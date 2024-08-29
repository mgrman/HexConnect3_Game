using UnityEngine;
using UnityEngine.UI;

namespace Match3Game.Score
{
    /// <summary>
    ///     Manager for score feature. Keeps score and resets it when the game resets.
    ///     Updates to the score are reflected in the associated UI text.
    /// </summary>
    public class ScoreboardManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerSessionManager playerSessionManager;

        [SerializeField]
        private Text text;

        [SerializeField]
        private string Format = "{0}";

        private int score;

        protected void Awake()
        {
            this.UpdateScore();
            this.playerSessionManager.GameReset += this.ResetScore;
        }

        protected void OnDestroy()
        {
            this.playerSessionManager.GameReset -= this.ResetScore;
        }

        public void ResetScore()
        {
            this.score = 0;
            this.UpdateScore();
        }

        public void AddScore(int value)
        {
            this.score += value;
            this.UpdateScore();
        }

        private void UpdateScore()
        {
            this.text.text = string.Format(this.Format, this.score);
        }
    }
}
