using Match3Game.InputHandling;
using UnityEngine;

namespace Match3Game.MainLogic
{
    /// <summary>
    ///     Handler to limit valid paths only to those that are longer than the path limit.
    /// </summary>
    public class RemoveIfPathLengthAboveLimitHandler : BasePathFinishedHandler
    {
        [SerializeField]
        [Range(2, 10)]
        private int pathLimit = 3;

        public override void ProcessFinalPath(CellPath fullPath, ref bool isValid)
        {
            isValid = fullPath.Count >= this.pathLimit;
        }
    }
}
