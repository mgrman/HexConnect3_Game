using UnityEngine;

namespace Match3Game.Utilities
{
    public class ToggleVisibilityHelper : MonoBehaviour
    {
        public void Toggle()
        {
            this.gameObject.SetActive(!this.gameObject.activeSelf);
        }
    }
}