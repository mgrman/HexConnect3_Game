using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Match3Game.Cells;
using Match3Game.DataTypes;
using Match3Game.InputHandling;
using Match3Game.Utilities;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3Game
{
    /// <summary>
    ///     This class stores the values of the hex grid and does operations on them. Any changes to the hex grid are reflected
    ///     in the cells that are managed here (storing values, instantiating and moving prefabs).
    /// </summary>
    public class MainHexGrid : MonoBehaviour
    {
        private const float OuterToInnerRadiusMultiplier = 0.866025404f; // sqrt(3)/2
        private const float CellOffsetXRelativeToInnerRadius = 1.732050807568877f; // sqrt(3)

        [SerializeField]
        private PlayerSessionManager playerSessionManager;

        [SerializeField]
        [Range(1, 20)]
        private int sizeX = 5;

        [SerializeField]
        [Range(1, 20)]
        private int sizeY = 6;

        [SerializeField]
        [Range(0, 8)]
        private int maxCellValue = 4;

        [SerializeField]
        [Range(0, 2)]
        private int bonusCount = 1;

        [SerializeField]
        private CellController cellPrefab;

        [SerializeField]
        private float cellPrefabOuterRadius = 0.5f;

        [SerializeField]
        private GridAlignment alignment = GridAlignment.Center;

        [SerializeField]
        private float movementTimeSeconds;

        private readonly List<CellController> controllerCache = new List<CellController>();

        private Vector2 alignmentOffset;

        private CellController[,] cellControllers;
        private float cellOddColumnOffsetY;
        private float cellSizeInX;
        private float cellSizeInY;

        public float CellPrefabOuterRadius => this.cellPrefabOuterRadius;

        protected void Awake()
        {
            this.Initialize();

            this.playerSessionManager.GameReset += this.ResetValues;
        }

        protected void OnDestroy()
        {
            this.playerSessionManager.GameReset -= this.ResetValues;
        }

#if UNITY_EDITOR
        protected void OnDrawGizmos()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            this.InitializeHelperFields();

            var sphereRadius = this.transform.TransformVector(this.cellPrefabOuterRadius, 0, 0)
                .magnitude;

            for (var ix = 0; ix < this.sizeX; ix++)
            {
                for (var iy = 0; iy < this.sizeY; iy++)
                {
                    var worldPosition = this.transform.TransformPoint(this.GetPosition(new CellCoordinate(ix, iy)));

                    Gizmos.DrawWireSphere(worldPosition, sphereRadius);
                }
            }
        }
#endif

        public bool RaycastCell(Vector3 screenPosition, out CellCoordinate hitResult)
        {
            hitResult = new CellCoordinate();
            var rayWorld = Camera.main.ScreenPointToRay(screenPosition);
            var rayLocal = new Ray(this.transform.InverseTransformPoint(rayWorld.origin), this.transform.InverseTransformVector(rayWorld.direction));

            var plane = new Plane(Vector3.back, Vector3.zero);
            if (!plane.Raycast(rayLocal, out var hitDistance))
            {
                return false;
            }

            var hitLocal = rayLocal.origin + (rayLocal.direction * hitDistance);

            const int range = 1;

            var cellX = Mathf.Clamp((int)((hitLocal.x - this.alignmentOffset.x) / this.cellSizeInX), range, this.sizeX - 1 - range);
            var cellY = Mathf.Clamp((int)((hitLocal.y - this.alignmentOffset.y) / this.cellSizeInY), range, this.sizeY - 1 - range);

            var probableMiddleCell = new CellCoordinate(cellX, cellY);
            var cellRange = new CellCoordinateRange(probableMiddleCell, probableMiddleCell);
            cellRange = cellRange.Extend(range);

            var minDistance = float.MaxValue;
            for (var ix = cellRange.Min.X; ix <= cellRange.Max.X; ix++)
            {
                for (var iy = cellRange.Min.Y; iy <= cellRange.Max.Y; iy++)
                {
                    var coords = new CellCoordinate(ix, iy);
                    var distance = Vector3.Distance(this.GetPosition(coords), hitLocal);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        hitResult = new CellCoordinate(ix, iy);
                    }
                }
            }

            return minDistance <= this.cellPrefabOuterRadius;
        }

        public CellValue GetValue(CellCoordinate cellCoordinate)
        {
            if (!this.cellControllers.ContainsIndex(cellCoordinate))
            {
                throw new ArgumentOutOfRangeException(nameof(cellCoordinate));
            }

            return this.cellControllers[cellCoordinate.X, cellCoordinate.Y]
                .Value;
        }

        public Task RemovePath(CellPath pathToRemove)
        {
            if (pathToRemove.IsEmpty)
            {
                return Task.CompletedTask;
            }

            var range = pathToRemove.Range;

            var maxHoleCount = 0;
            for (var ix = range.Min.X; ix <= range.Max.X; ix++)
            {
                var holeCount = 0;
                for (var iy = range.Min.Y; iy < this.sizeY; iy++)
                {
                    var controller = this.cellControllers[ix, iy];
                    var coord = new CellCoordinate(ix, iy);
                    var isHole = pathToRemove.Path.Contains(coord);

                    if (isHole)
                    {
                        holeCount++;
                        this.controllerCache.Add(controller);
                        this.cellControllers[ix, iy] = null;
                    }
                    else
                    {
                        if (holeCount == 0)
                        {
                            continue;
                        }

                        this.cellControllers[ix, iy - holeCount] = controller;
                        this.StartCoroutine(this.MoveDown(controller, coord, holeCount));
                    }
                }

                for (var iy = this.sizeY - holeCount; iy < this.sizeY; iy++)
                {
                    var controller = this.controllerCache.First();
                    this.controllerCache.RemoveAt(0);
                    controller.Value = this.GetNewValue();
                    this.cellControllers[ix, iy] = controller;

                    var startCell = new CellCoordinate(ix, iy + holeCount);
                    this.StartCoroutine(this.MoveDown(controller, startCell, holeCount));
                }

                if (holeCount > maxHoleCount)
                {
                    maxHoleCount = holeCount;
                }
            }

            this.controllerCache.Clear();

            return Task.Delay(TimeSpan.FromSeconds(this.movementTimeSeconds * maxHoleCount));
        }

        private IEnumerator MoveDown(CellController controller, CellCoordinate startCell, int offsetDown)
        {
            var startTime = Time.time;
            var timeLenght = this.movementTimeSeconds * offsetDown;
            var endTime = startTime + timeLenght;
            var startPosition = this.GetPosition(startCell);
            var endPosition = this.GetPosition(new CellCoordinate(startCell.X, startCell.Y - offsetDown));

            while (Time.time < endTime)
            {
                var alpha = (Time.time - startTime) / timeLenght;
                controller.transform.localPosition = Vector3.Lerp(startPosition, endPosition, alpha);
                yield return null;
            }

            controller.transform.localPosition = endPosition;
        }

        private void Initialize()
        {
            if ((this.sizeX < 0) || (this.sizeX > int.MaxValue))
            {
                throw new InvalidOperationException($"{nameof(this.sizeX)} must be between {0} and {int.MaxValue}!");
            }

            if ((this.sizeY < 0) || (this.sizeY > int.MaxValue))
            {
                throw new InvalidOperationException($"{nameof(this.sizeY)} must be between {0} and {int.MaxValue}!");
            }

            if ((this.maxCellValue < 0) || (this.maxCellValue > int.MaxValue))
            {
                throw new InvalidOperationException($"{nameof(this.maxCellValue)} must be between {0} and {int.MaxValue}!");
            }

            if (this.cellPrefab == null)
            {
                throw new InvalidOperationException($"{nameof(this.cellPrefab)} is not defined!");
            }

            this.InitializeHelperFields();

            for (var ix = 0; ix < this.sizeX; ix++)
            {
                for (var iy = 0; iy < this.sizeY; iy++)
                {
                    var cellGameObject = Instantiate(this.cellPrefab.gameObject, this.transform)
                        .GetComponent<CellController>();

                    var coords = new CellCoordinate(ix, iy);

                    cellGameObject.name = $"cell_{coords}";
                    cellGameObject.transform.localPosition = this.GetPosition(coords);
                    cellGameObject.Value = this.GetNewValue();
                    cellGameObject.hideFlags = HideFlags.DontSave;
                    this.cellControllers[ix, iy] = cellGameObject;
                }
            }
        }

        private void ResetValues()
        {
            for (var ix = 0; ix < this.sizeX; ix++)
            {
                for (var iy = 0; iy < this.sizeY; iy++)
                {
                    var controller = this.cellControllers[ix, iy];
                    controller.Value = this.GetNewValue();
                }
            }
        }

        private void InitializeHelperFields()
        {
            this.cellOddColumnOffsetY = this.cellPrefabOuterRadius * OuterToInnerRadiusMultiplier;
            this.cellSizeInY = this.cellOddColumnOffsetY * 2f;
            this.cellSizeInX = this.cellOddColumnOffsetY * CellOffsetXRelativeToInnerRadius;
            this.cellControllers = new CellController[this.sizeX, this.sizeY];

            switch (this.alignment)
            {
                case GridAlignment.Center:
                    this.alignmentOffset = new Vector2((-this.cellSizeInX * (this.sizeX - 1)) / 2f, (-this.cellSizeInY * (this.sizeY - 0.5f)) / 2f);
                    break;
                case GridAlignment.StartAtBottomLeft:
                    this.alignmentOffset = Vector2.zero;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unsupported {nameof(this.alignment)} value {this.alignment}!");
            }
        }

        private CellValue GetNewValue()
        {
            var mainValue = Random.Range(0, this.maxCellValue);
            var bonusType = Random.value > 0.9f ? Random.Range(1, this.bonusCount + 1) : 0;
            return new CellValue(mainValue, bonusType);
        }

        public Vector3 GetPosition(CellCoordinate coordinate)
        {
            var x = (coordinate.X * this.cellSizeInX) + this.alignmentOffset.x;
            var y = (coordinate.Y * this.cellSizeInY) + ((coordinate.X % 2) * this.cellOddColumnOffsetY) + this.alignmentOffset.y;

            return new Vector3(x, y, 0);
        }

        public bool IsCellContained(CellCoordinate cell) => (cell.X >= 0) && (cell.Y >= 0) && (cell.X < this.sizeX) && (cell.Y < this.sizeY);
    }
}
