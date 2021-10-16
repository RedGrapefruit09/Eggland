using System.Collections;
using UnityEngine;

namespace Eggland
{
    /// <summary>
    /// A gatherable is a prop that can be mined/chopped down using a tool.
    ///
    /// This class is extensible, due to some types of gatherables requiring custom graphical behavior.
    /// </summary>
    public class Gatherable : MonoBehaviour
    {
        [SerializeField] private Sprite[] overlays; // list of gathering overlays
        [SerializeField] protected ToolType type; // the type of tool that is required to gather this
        [SerializeField] private SpriteRenderer overlayRenderer; // a reference to a sprite renderer of the overlay object

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
                StartCoroutine(OverlayAnimation(overlays, player));
            }
        }

        protected IEnumerator OverlayAnimation(Sprite[] overlayCollection, Player player)
        {
            // Use every overlay with a delay of a third of a second
            foreach (var overlay in overlayCollection)
            {
                yield return new WaitForSeconds(0.3f);
                overlayRenderer.sprite = overlay;
            }
            
            player.IsGathering = false; // trigger the end of the gathering operation for the player
            Destroy(gameObject); // commit suicide
        }
    }
}