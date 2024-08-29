using System;

namespace Match3Game.DataTypes
{
    /// <summary>
    ///     Struct holding possible values of a cell. It is a combination of Cell Type which corresponds to color and BonusType
    ///     which the type of bonus on that cell.
    /// </summary>
    public readonly struct CellValue : IEquatable<CellValue>
    {
        public readonly int CellType;

        public readonly int BonusType;

        public CellValue(int cellType, int bonusType)
        {
            this.CellType = cellType;
            this.BonusType = bonusType;
        }

        public bool Equals(CellValue other) => (this.CellType == other.CellType) && (this.BonusType == other.BonusType);

        public override bool Equals(object obj) => obj is CellValue other && this.Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.CellType.GetHashCode() * 397) ^ this.BonusType.GetHashCode();
            }
        }

        public static bool operator ==(CellValue left, CellValue right) => left.Equals(right);

        public static bool operator !=(CellValue left, CellValue right) => !left.Equals(right);
    }
}
