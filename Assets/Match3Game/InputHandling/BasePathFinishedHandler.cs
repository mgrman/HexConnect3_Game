using UnityEngine;

namespace Match3Game.InputHandling
{
    /// <summary>
    ///     Base class for implementing path finished handlers.
    ///     Inherits from <see cref="MonoBehaviour" /> to allow compositing in Unity Scenes.
    /// </summary>
    public abstract class BasePathFinishedHandler : MonoBehaviour
    {
        public abstract void ProcessFinalPath(CellPath fullPath, ref bool isValid);
    }
}
