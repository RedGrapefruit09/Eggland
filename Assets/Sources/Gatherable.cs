using System.Collections;
using UnityEngine;

namespace Eggland
{
    public class Gatherable : MonoBehaviour
    {
        [SerializeField] private Sprite[] overlays;
        [SerializeField] protected ToolType type;
        [SerializeField] private SpriteRenderer overlayRenderer;

        public virtual void Gather(Tool tool, Player player)
        {
            if (tool.type == type)
            {
                player.IsGathering = true;
                StartCoroutine(OverlayAnimation(overlays, player));
            }
        }

        protected IEnumerator OverlayAnimation(Sprite[] overlayCollection, Player player)
        {
            foreach (var overlay in overlayCollection)
            {
                yield return new WaitForSeconds(0.3f);
                overlayRenderer.sprite = overlay;
            }

            player.IsGathering = false;
            Destroy(gameObject);
        }
    }
}