using Match3Game.DataTypes;
using Match3Game.InputHandling;
using UnityEngine;

namespace Match3Game.MainLogic
{
    /// <summary>
    ///     Constraint limiting cell selection during path input to only those that have the same value as the first one, if it
    ///     exists.
    /// </summary>
    public class SimpleValueCellSelectionConstraint : BaseCellSelectionConstraint
    {
        [SerializeField]
        private MainHexGrid grid;

        public override bool CheckCell(CellPath fullPath, CellCoordinate cell)
        {
            if (fullPath.IsEmpty)
            {
                return true;
            }

            var mainCellType = this.grid.GetValue(fullPath.FirstCell)
                .CellType;

            var newValue = this.grid.GetValue(cell);
            return newValue.CellType == mainCellType;
        }
    }
}
