using System.Collections;
using UnityEngine;
using Eggland.Tools;

namespace Eggland.Gathering
{
    /// <summary>
    /// A gatherable is a prop that can be mined/chopped down using a tool.
    ///
    /// This class is extensible, due to some types of gatherables requiring custom graphical behavior.
    /// </summary>
    public class Gatherable : MonoBehaviour
    {
        [SerializeField] private Sprite[] overlays; // list of gathering overlays
        public ToolType type; // the type of tool that is required to gather this
        [SerializeField] private SpriteRenderer overlayRenderer; // a reference to a sprite renderer of the overlay object
        [SerializeField] private SimplePair<ResourceType, Range> drop;
        [SerializeField] private SerializedDictionary<string, SimplePair<ResourceType, Range>> dependentDrops;
        private ResourceStorage storage;

        private void Awake()
        {
            storage = FindObjectOfType<ResourceStorage>();
        }

        /// <summary>
        /// Trigger the gathering operation if that operation is feasible (the tool is of the correct type).
        /// </summary>
        /// <param name="tool">Tool to gather with</param>
        /// <param name="player">Player script</param>
        public virtual void Gather(Tool tool, Player player)
        {
            if (tool.type == type) // check if type matches
            {
                player.IsGathering = true;
                StartCoroutine(OverlayAnimation(overlays, player, tool));
            }
        }

        protected IEnumerator OverlayAnimation(Sprite[] overlayCollection, Player player, Tool tool)
        {
            // Use every overlay with a delay of a third of a second
            foreach (var overlay in overlayCollection)
            {
                yield return new WaitForSeconds(GetAnimationDelay(tool));
                overlayRenderer.sprite = overlay;
            }
            
            player.IsGathering = false; // trigger the end of the gathering operation for the player
            Destroy(gameObject); // commit suicide
            SendGatheredResources(); // send resources
        }

        protected static float GetAnimationDelay(Tool tool) => 3.5f / tool.efficiency;

        protected void SendGatheredResources()
        {
            if (dependentDrops.Count == 0)
            {
                var amount = Random.Range(drop.value.min, drop.value.max + 1);
                storage.Add(drop.key, amount);
            }
            else
            {
                foreach (var pair in dependentDrops)
                {
                    if (!GetComponent<SpriteRenderer>().sprite.name.StartsWith(pair.Key)) continue;
                    
                    var amount = Random.Range(pair.Value.value.min, pair.Value.value.max + 1);
                    storage.Add(pair.Value.key, amount);
                    
                    break;
                }
            }
        }
    }
}