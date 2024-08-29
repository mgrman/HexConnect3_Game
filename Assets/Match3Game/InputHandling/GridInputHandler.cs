using System;
using System.Linq;
using Match3Game.DataTypes;
using UnityEngine;

namespace Match3Game.InputHandling
{
    /// <summary>
    ///     Input handler to draw on the hex grid using mouse.
    /// </summary>
    public class GridInputHandler : MonoBehaviour
    {
        [SerializeField]
        private MainHexGrid grid;

        [SerializeField]
        private BaseCellSelectionConstraint[] contstraints;

        [SerializeField]
        private BasePathFinishedHandler[] pathFinishedHandlers;

        private bool isAnimating;

        public CellPath Path { get; } = new CellPath();

        public event Action PathChanged;

        private void Awake()
        {
            this.Path.CellPathChanged += this.InvokePathChanged;
        }

        protected void Update()
        {
            if (!Input.mousePresent)
            {
                return;
            }

            if (this.isAnimating)
            {
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                this.HandlePathEnd();
                return;
            }

            if (!Input.GetMouseButton(0))
            {
                return;
            }

            this.HandlePathUpdate();
        }

        protected void OnDestroy()
        {
            this.Path.CellPathChanged -= this.InvokePathChanged;
        }

        private void HandlePathUpdate()
        {
            if (!this.grid.RaycastCell(Input.mousePosition, out var cell))
            {
                return;
            }

            if (this.Path.IsNotEmpty && (cell == this.Path.LastCell))
            {
                return;
            }

            if (!this.IsNewCellValid(cell))
            {
                return;
            }

            this.Path.AddContinuousCell(cell);
        }

        private bool IsNewCellValid(CellCoordinate cell)
        {
            if (this.contstraints != null)
            {
                foreach (var constraint in this.contstraints)
                {
                    if (!constraint.CheckCell(this.Path, cell))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private async void HandlePathEnd()
        {
            if (this.Path.IsEmpty)
            {
                return;
            }

            this.isAnimating = true;
            try
            {
                if (this.IsFinalPathValid())
                {
                    await this.grid.RemovePath(this.Path);
                }
            }
            finally
            {
                this.isAnimating = false;
                this.Path.Clear();
            }
        }

        private bool IsFinalPathValid()
        {
            var isValid = true;
            foreach (var pathFinishedHandler in this.pathFinishedHandlers)
            {
                pathFinishedHandler.ProcessFinalPath(this.Path, ref isValid);
            }

            return isValid;
        }

        private void InvokePathChanged()
        {
            this.PathChanged?.Invoke();
        }
    }
}
