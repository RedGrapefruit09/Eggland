using UnityEngine;

namespace Eggland
{
    public class Gatherable : MonoBehaviour
    {
        [SerializeField] private Sprite[] overlays;
        [SerializeField] private ToolType type;

        private SpriteRenderer overlayRenderer;

        private void Awake()
        {
            overlayRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void Gather(Tool tool)
        {
            if (tool.type == type) Destroy(gameObject);
        }
    }
}