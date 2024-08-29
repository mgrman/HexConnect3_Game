using System;
using System.Collections.Generic;

namespace Match3Game.DataTypes
{
    /// <summary>
    ///     Struct holding cell coordinate ranges, to allow easier operations with them.
    /// </summary>
    public readonly struct CellCoordinateRange : IEquatable<CellCoordinateRange>
    {
        public readonly CellCoordinate Min;

        /// <summary>
        ///     Inclusive Max
        /// </summary>
        public readonly CellCoordinate Max;

        public CellCoordinateRange(CellCoordinate min, CellCoordinate max)
        {
            this.Min = min;
            this.Max = max;
        }

        public CellCoordinateRange Extend(int i) => new CellCoordinateRange(this.Min - i, this.Max + i);

        public CellCoordinateRange Encapsulate(CellCoordinate cell) => new CellCoordinateRange(CellCoordinate.Min(this.Min, cell), CellCoordinate.Max(this.Max, cell));

        public override string ToString() => $"{this.Min}-{this.Max}";

        public bool Equals(CellCoordinateRange other) => this.Min.Equals(other.Min) && this.Max.Equals(other.Max);

        public override bool Equals(object obj) => obj is CellCoordinateRange other && this.Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Min.GetHashCode() * 397) ^ this.Max.GetHashCode();
            }
        }

        public static bool operator ==(CellCoordinateRange left, CellCoordinateRange right) => left.Equals(right);

        public static bool operator !=(CellCoordinateRange left, CellCoordinateRange right) => !left.Equals(right);

        public static CellCoordinateRange FromCellRange(IEnumerable<CellCoordinate> path)
        {
            var min = new CellCoordinate(int.MaxValue, int.MaxValue);
            var max = new CellCoordinate(int.MinValue, int.MinValue);
            foreach (var cell in path)
            {
                min = CellCoordinate.Min(min, cell);
                max = CellCoordinate.Min(max, cell);
            }

            return new CellCoordinateRange(min, max);
        }
    }
}
