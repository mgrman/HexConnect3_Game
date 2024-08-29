using Match3Game.DataTypes;
using Match3Game.InputHandling;
using UnityEngine;

namespace Match3Game.MainLogic
{
    /// <summary>
    ///     Constraint limiting cell selection to only neighbours of the last cell in the path, if it exists.
    /// </summary>
    public class NextCellMustBeNeighbourOfPreviousOneConstraint : BaseCellSelectionConstraint
    {
        [SerializeField]
        private MainHexGrid grid;

        public override bool CheckCell(CellPath fullPath, CellCoordinate cell)
        {
            if (fullPath.IsEmpty)
            {
                return true;
            }

            var lastCell = fullPath.LastCell;
            return lastCell.IsNeighbour(cell);
        }
    }
}
