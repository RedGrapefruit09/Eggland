using System.Collections;
using UnityEngine;

namespace Eggland
{
    public class Gatherable : MonoBehaviour
    {
        [SerializeField] private Sprite[] overlays;
        [SerializeField] protected ToolType type;
        [SerializeField] private SpriteRenderer overlayRenderer;

        public virtual void Gather(Tool tool)
        {
            if (tool.type == type) StartCoroutine(OverlayAnimation(overlays));
        }

        protected IEnumerator OverlayAnimation(Sprite[] overlayCollection)
        {
            foreach (var overlay in overlayCollection)
            {
                yield return new WaitForSeconds(0.3f);
                overlayRenderer.sprite = overlay;
            }
            
            Destroy(gameObject);
        }
    }
}