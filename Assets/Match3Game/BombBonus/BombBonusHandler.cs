using System.Linq;
using Match3Game.InputHandling;
using UnityEngine;

namespace Match3Game.BombBonus
{
    /// <summary>
    ///     Handler for adding neighbours of Bomb bonus cells to the evaluated path.
    /// </summary>
    public class BombBonusHandler : BasePathFinishedHandler
    {
        [SerializeField]
        private MainHexGrid grid;

        [SerializeField]
        private int bombBonusType = 1;

        public override void ProcessFinalPath(CellPath fullPath, ref bool isValid)
        {
            if (!isValid)
            {
                return;
            }

            for (var i = 0; i < fullPath.Path.Count; i++)
            {
                var cell = fullPath.Path[i];
                var cellValue = this.grid.GetValue(cell);
                if (cellValue.BonusType != this.bombBonusType)
                {
                    continue;
                }

                foreach (var neighbour in cell.GetNeighbours())
                {
                    if (!this.grid.IsCellContained(neighbour))
                    {
                        continue;
                    }

                    if (fullPath.Path.Contains(neighbour))
                    {
                        continue;
                    }

                    fullPath.AddFreePlaceCell(neighbour);
                }
            }
        }
    }
}
