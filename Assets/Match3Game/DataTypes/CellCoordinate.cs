using System;
using System.Collections.Generic;

namespace Match3Game.DataTypes
{
    /// <summary>
    ///     Struct holding coordinates of a cell in the hex grid (the value is not stored here).
    /// </summary>
    public readonly struct CellCoordinate : IEquatable<CellCoordinate>
    {
        public readonly int X;
        public readonly int Y;

        public CellCoordinate(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public bool IsNeighbour(CellCoordinate that)
        {
            var diffX = that.X - this.X;
            var diffY = that.Y - this.Y;

            if ((diffX == 1) || (diffX == -1))
            {
                if (diffY == 0)
                {
                    return true;
                }

                if ((this.X % 2) == 0)
                {
                    return diffY == -1;
                }

                return diffY == 1;
            }

            if (diffX == 0)
            {
                return (diffY == 1) || (diffY == -1);
            }

            return false;
        }

        public IEnumerable<CellCoordinate> GetNeighbours()
        {
            yield return new CellCoordinate(this.X, this.Y + 1);
            yield return new CellCoordinate(this.X, this.Y - 1);

            if ((this.X % 2) == 1)
            {
                yield return new CellCoordinate(this.X + 1, this.Y);
                yield return new CellCoordinate(this.X + 1, this.Y + 1);
                yield return new CellCoordinate(this.X - 1, this.Y);
                yield return new CellCoordinate(this.X - 1, this.Y + 1);
            }
            else
            {
                yield return new CellCoordinate(this.X + 1, this.Y - 1);
                yield return new CellCoordinate(this.X + 1, this.Y);
                yield return new CellCoordinate(this.X - 1, this.Y - 1);
                yield return new CellCoordinate(this.X - 1, this.Y);
            }
        }

        public static CellCoordinate Min(CellCoordinate a, CellCoordinate b) => new CellCoordinate(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));

        public static CellCoordinate Max(CellCoordinate a, CellCoordinate b) => new CellCoordinate(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

        public override string ToString() => $"({this.X}, {this.Y})";

        public static CellCoordinate operator +(CellCoordinate @this, int val) => new CellCoordinate(@this.X + val, @this.Y + val);

        public static CellCoordinate operator -(CellCoordinate @this, int val) => new CellCoordinate(@this.X - val, @this.Y - val);

        public static bool operator ==(CellCoordinate left, CellCoordinate right) => left.Equals(right);

        public static bool operator !=(CellCoordinate left, CellCoordinate right) => !left.Equals(right);

        public bool Equals(CellCoordinate other) => (this.X == other.X) && (this.Y == other.Y);

        public override bool Equals(object obj) => obj is CellCoordinate other && this.Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.X.GetHashCode() * 397) ^ this.Y.GetHashCode();
            }
        }
    }
}
