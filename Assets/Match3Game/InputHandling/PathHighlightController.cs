using System.Collections.Generic;
using System.Linq;
using Match3Game.DataTypes;
using UnityEngine;

namespace Match3Game.InputHandling
{
    /// <summary>
    ///     Controller for creating path highlight based on user selection in <see cref="GridInputHandler" />.
    /// </summary>
    public class PathHighlightController : MonoBehaviour
    {
        [SerializeField]
        private GameObject edgeHighlightPrefab;

        [SerializeField]
        private GameObject centerHighlightPrefab;

        [SerializeField]
        private MainHexGrid grid;

        [SerializeField]
        private GridInputHandler gridInputHandler;

        private readonly List<GameObject> centerHighlightsActive = new List<GameObject>();
        private readonly List<GameObject> centerHighlightsPool = new List<GameObject>();
        private readonly List<GameObject> edgeHighlightsActive = new List<GameObject>();
        private readonly List<GameObject> edgeHighlightsPool = new List<GameObject>();

        protected void Awake()
        {
            this.gridInputHandler.PathChanged += this.UpdateHighlightedPath;
        }

        protected void OnDestroy()
        {
            this.gridInputHandler.PathChanged -= this.UpdateHighlightedPath;
        }

        private void UpdateHighlightedPath()
        {
            this.MoveActiveToPool(this.centerHighlightsActive, this.centerHighlightsPool);
            this.MoveActiveToPool(this.edgeHighlightsActive, this.edgeHighlightsPool);

            var path = this.gridInputHandler.Path;
            if (path.IsEmpty)
            {
                return;
            }

            this.CreateCenterHighlight(path.FirstCell);

            if (path.Count == 1)
            {
                return;
            }

            var previousCell = path.FirstCell;
            foreach (var cell in path.Path.Skip(1))
            {
                this.CreateCenterHighlight(cell);
                this.CreateEdgeHighlight(previousCell, cell);

                previousCell = cell;
            }
        }

        private void MoveActiveToPool(List<GameObject> activeList, List<GameObject> pool)
        {
            foreach (var highlight in activeList)
            {
                highlight.SetActive(false);
            }

            pool.AddRange(activeList);
            activeList.Clear();
        }

        private void CreateCenterHighlight(CellCoordinate cell)
        {
            var highlight = this.GetItemFromPool(this.centerHighlightsPool, this.centerHighlightsActive, this.centerHighlightPrefab);
            highlight.transform.position = this.GetCellCenter(cell);
        }

        private void CreateEdgeHighlight(CellCoordinate fromCell, CellCoordinate toCell)
        {
            var fromPoint = this.GetCellCenter(fromCell);
            var toPoint = this.GetCellCenter(toCell);

            var center = (fromPoint + toPoint) / 2f;
            var orientation = Quaternion.FromToRotation(this.grid.transform.forward, toPoint - fromPoint);

            var highlight = this.GetItemFromPool(this.edgeHighlightsPool, this.edgeHighlightsActive, this.edgeHighlightPrefab);
            highlight.transform.position = center;
            highlight.transform.rotation = orientation;
        }

        private GameObject GetItemFromPool(List<GameObject> pool, List<GameObject> activeList, GameObject prefab)
        {
            GameObject highlight;
            if (pool.Count > 0)
            {
                var lastItemIndex = pool.Count - 1;
                highlight = pool[lastItemIndex];
                pool.RemoveAt(lastItemIndex);
                highlight.SetActive(true);
            }
            else
            {
                highlight = Instantiate(prefab, this.transform);
            }

            activeList.Add(highlight);
            return highlight;
        }

        private Vector3 GetCellCenter(CellCoordinate coords)
        {
            var gridPosition = this.grid.GetPosition(coords);
            var worldPosition = this.grid.transform.TransformPoint(gridPosition);
            return worldPosition;
        }
    }
}
