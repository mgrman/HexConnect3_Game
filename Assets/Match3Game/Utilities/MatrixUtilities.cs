using Match3Game.DataTypes;

namespace Match3Game.Utilities
{
    public static class MatrixUtilities
    {
        public static int GetSizeX<T>(this T[,] matrix) => matrix.GetSizeDimension(0);

        public static int GetSizeY<T>(this T[,] matrix) => matrix.GetSizeDimension(1);

        public static int GetSizeDimension<T>(this T[,] matrix, int dimension) => matrix.GetUpperBound(dimension) + 1;

        public static bool ContainsIndex<T>(this T[,] matrix, CellCoordinate coordinate) => (coordinate.X < matrix.GetSizeX()) && (coordinate.Y < matrix.GetSizeY());
    }
}
