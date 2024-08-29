using System;
using UnityEngine;

namespace Match3Game
{
    /// <summary>
    ///     Class for handling player sessions. Acts as a central place for resetting game.
    /// </summary>
    public class PlayerSessionManager : MonoBehaviour
    {
        public event Action GameReset;

        public void ResetGame()
        {
            this.GameReset?.Invoke();
        }
    }
}
