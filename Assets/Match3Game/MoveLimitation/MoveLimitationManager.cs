using System;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Game.MoveLimitation
{
    /// <summary>
    ///     Manager for move limitation feature. Keeps number of allowed moves and resets them when the game resets.
    ///     Updates to how many moves are left are reflected in the associated UI text.
    /// </summary>
    public class MoveLimitationManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerSessionManager playerSessionManager;

        [SerializeField]
        private Text text;

        [SerializeField]
        private int moveLimit;

        [SerializeField]
        private string Format = "{0}";

        private int movesLeft;

        public bool AnyMovesLeft => this.movesLeft > 0;

        protected void Awake()
        {
            this.movesLeft = this.moveLimit;
            this.UpdateMoveLimitUI();
            this.playerSessionManager.GameReset += this.ResetMoves;
        }

        protected void OnDestroy()
        {
            this.playerSessionManager.GameReset -= this.ResetMoves;
        }

        public void ResetMoves()
        {
            this.movesLeft = this.moveLimit;
            this.UpdateMoveLimitUI();
        }

        public void SubtractMove()
        {
            if (!this.AnyMovesLeft)
            {
                throw new InvalidOperationException("No moves left!");
            }

            this.movesLeft--;
            this.UpdateMoveLimitUI();
        }

        private void UpdateMoveLimitUI()
        {
            this.text.text = string.Format(this.Format, this.movesLeft);
        }
    }
}
