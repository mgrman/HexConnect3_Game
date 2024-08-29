using Match3Game.InputHandling;
using UnityEngine;

namespace Match3Game.MoveLimitation
{
    /// <summary>
    ///     Handler for limiting finishing of drawing if the user ran out of moves.
    /// </summary>
    public class SubtractMoveOnPathFinishHandler : BasePathFinishedHandler
    {
        [SerializeField]
        private MoveLimitationManager moveLimiter;

        public override void ProcessFinalPath(CellPath fullPath, ref bool isValid)
        {
            isValid = isValid && this.moveLimiter.AnyMovesLeft;
            if (isValid)
            {
                this.moveLimiter.SubtractMove();
            }
        }
    }
}
