using System;
using UnityEngine;

namespace Match3Game.Cells
{
    /// <summary>
    ///     Shared configuration for <see cref="CellController" /> to not have to copy configuration around.
    /// </summary>
    [CreateAssetMenu(menuName = "Create CellConfig", fileName = "CellConfig", order = 0)]
    public class CellConfig : ScriptableObject
    {
        [SerializeField]
        private Material[] materialPerValue;

        [SerializeField]
        private string[] bonusIcons;

        public Material GetMaterial(int value)
        {
            if (value >= this.materialPerValue.Length)
            {
                throw new InvalidOperationException("Materials were not defined for this value!");
            }

            return this.materialPerValue[value];
        }

        public string GetBonusIcon(int value)
        {
            if (value >= this.materialPerValue.Length)
            {
                throw new InvalidOperationException("Materials were not defined for this value!");
            }

            return this.bonusIcons[value];
        }
    }
}
