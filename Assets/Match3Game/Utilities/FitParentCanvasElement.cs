using System;
using UnityEngine;

namespace Match3Game.Utilities
{
    /// <summary>
    ///     Helper class to scale content to fit as square in the parent <see cref="RectTransform" />
    /// </summary>
    [ExecuteInEditMode]
    public class FitParentCanvasElement : MonoBehaviour
    {
        [SerializeField]
        private float ScaleFactor = 0.001f;

        private RectTransform parentRectTransform;

        protected void Awake()
        {
            this.parentRectTransform = this.transform.parent as RectTransform;
            this.UpdatePosition(this.parentRectTransform);
        }

        // Update is called once per frame
        protected void Update()
        {
#if UNITY_EDITOR
            this.parentRectTransform = this.transform.parent as RectTransform;
#endif
            if (!this.parentRectTransform.hasChanged)
            {
                return;
            }

            this.UpdatePosition(this.parentRectTransform);
        }

        private void UpdatePosition(RectTransform parent)
        {
            this.transform.localPosition = Vector3.zero;

            var scale = parent.rect;

            var uniformScale = Math.Min(scale.width, scale.height) * this.ScaleFactor;

            this.transform.localScale = new Vector3(uniformScale, uniformScale, uniformScale);
        }
    }
}
