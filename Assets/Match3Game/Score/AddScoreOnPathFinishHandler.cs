using System.Linq;
using Match3Game.InputHandling;
using UnityEngine;

namespace Match3Game.Score
{
    /// <summary>
    ///     Handler for updating score when path drawing by user is finished.
    /// </summary>
    public class AddScoreOnPathFinishHandler : BasePathFinishedHandler
    {
        [SerializeField]
        private ScoreboardManager scoreboard;

        [SerializeField]
        private MainHexGrid grid;

        [SerializeField]
        private int multiplierBonusType = 2;

        public override void ProcessFinalPath(CellPath fullPath, ref bool isValid)
        {
            if (!isValid)
            {
                return;
            }

            var score = Mathf.RoundToInt(((1 + fullPath.Count) / 2f) * fullPath.Count);

            var bonusMultiplier = 1 + fullPath.Path.Count(o => this.grid.GetValue(o)
                .BonusType == this.multiplierBonusType);

            this.scoreboard.AddScore(score * bonusMultiplier);
        }
    }
}
