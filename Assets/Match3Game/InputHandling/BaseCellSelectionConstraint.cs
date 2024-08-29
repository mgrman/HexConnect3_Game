using Match3Game.DataTypes;
using UnityEngine;

namespace Match3Game.InputHandling
{
    /// <summary>
    ///     Base class for implementing cell selection constraints.
    ///     Inherits from <see cref="MonoBehaviour" /> to allow compositing in Unity Scenes.
    /// </summary>
    public abstract class BaseCellSelectionConstraint : MonoBehaviour
    {
        public abstract bool CheckCell(CellPath fullPath, CellCoordinate cell);
    }
}
