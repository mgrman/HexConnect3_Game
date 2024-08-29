using System;
using System.Collections.Generic;
using Match3Game.DataTypes;

namespace Match3Game.InputHandling
{
    /// <summary>
    ///     Class holding the path through the cells as user is drawing it on the hex board. Handles drawing backward.
    /// </summary>
    public class CellPath
    {
        private readonly List<CellCoordinate> path = new List<CellCoordinate>(20);
        private CellCoordinateRange range;

        public event Action CellPathChanged;

        public IReadOnlyList<CellCoordinate> Path => this.path;

        public bool IsEmpty => this.path.Count == 0;

        public bool IsNotEmpty => this.path.Count > 0;

        public int Count => this.path.Count;

        public CellCoordinateRange Range
        {
            get
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException($"Cannot call {nameof(this.LastCell)} when Empty!");
                }

                return this.range;
            }
        }

        public CellCoordinate LastCell
        {
            get
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException($"Cannot call {nameof(this.LastCell)} when Empty!");
                }

                return this.path[this.path.Count - 1];
            }
        }

        public CellCoordinate FirstCell
        {
            get
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException($"Cannot call {nameof(this.FirstCell)} when Empty!");
                }

                return this.path[0];
            }
        }

        public void Clear()
        {
            if (this.path.Count == 0)
            {
                return;
            }

            this.path.Clear();
            this.CellPathChanged?.Invoke();
            this.range = new CellCoordinateRange();
        }

        public void AddContinuousCell(CellCoordinate cell)
        {
            var indexOfExistingItem = this.path.IndexOf(cell);

            if (indexOfExistingItem >= 0)
            {
                var indexOfNextItem = indexOfExistingItem + 1;

                this.path.RemoveRange(indexOfNextItem, this.Path.Count - indexOfNextItem);

                //remove at from the end to not copy the list, only move the count
                for (var i = this.Path.Count - 1; i >= indexOfNextItem; i--)
                {
                    this.path.RemoveAt(i);
                }

                if (this.IsEmpty)
                {
                    this.range = new CellCoordinateRange();
                }
                else
                {
                    this.range = CellCoordinateRange.FromCellRange(this.path);
                }
            }
            else
            {
                if (this.IsEmpty)
                {
                    this.range = new CellCoordinateRange(cell, cell);
                }
                else
                {
                    this.range = this.range.Encapsulate(cell);
                }

                this.path.Add(cell);
            }

            this.CellPathChanged?.Invoke();
        }

        public void AddFreePlaceCell(CellCoordinate cell)
        {
            if (this.IsEmpty)
            {
                this.range = new CellCoordinateRange(cell, cell);
            }
            else
            {
                this.range = this.range.Encapsulate(cell);
            }

            this.path.Add(cell);

            this.CellPathChanged?.Invoke();
        }
    }
}
