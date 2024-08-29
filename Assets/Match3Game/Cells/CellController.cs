using Match3Game.DataTypes;
using UnityEngine;

namespace Match3Game.Cells
{
    /// <summary>
    ///     Controller for updating the main cell UX. Needs to be applied to the prefab linked in <see cref="MainHexGrid" />.
    /// </summary>
    [ExecuteInEditMode]
    public class CellController : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer meshRenderer;

        [SerializeField]
        private TextMesh text;

        [SerializeField]
        private CellConfig cellConfig;

        private CellValue value;

        public CellValue Value
        {
            get => this.value;
            set
            {
                if (this.Value == value)
                {
                    return;
                }

                this.value = value;
                this.UpdateCellValue();
            }
        }

        protected void Awake()
        {
            this.UpdateCellValue();
        }

#if UNITY_EDITOR
        protected void Update()
        {
            this.UpdateCellValue();
        }
#endif

        private void UpdateCellValue()
        {
            this.meshRenderer.material = this.cellConfig.GetMaterial(this.value.CellType);
            this.text.text = this.cellConfig.GetBonusIcon(this.Value.BonusType);
        }
    }
}
