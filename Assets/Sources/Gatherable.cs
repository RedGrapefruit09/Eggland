using System.Collections;
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
            if (tool.type == type)
            {
                StartCoroutine(StartGathering(tool));
            }
        }

        private IEnumerator StartGathering(Tool tool)
        {
            foreach (var overlay in overlays)
            {
                yield return new WaitForSeconds(0.3333333333333333333f);
                overlayRenderer.sprite = overlay;
            }
            
            Destroy(gameObject);
        }
    }
}