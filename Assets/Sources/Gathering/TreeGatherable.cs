using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eggland.Gathering
{
    /// <summary>
    /// A custom gatherable for trees, with normal animations instead of overlays.
    /// </summary>
    public class TreeGatherable : Gatherable
    {
        // A dictionary of animation sprites for every type of tree. This boi is chunky
        [SerializeField] private SerializedDictionary<string, Sprite[]> treeAnimations;

        public override void Gather(Tool tool, Player player)
        {
            if (tool.type == type)
            {
                player.IsGathering = true;
                StartCoroutine(TreeAnimation(GetAnimation(), player, tool)); // start a custom coroutine
            }
        }

        private IEnumerator TreeAnimation(IEnumerable<Sprite> treeAnimation, Player player, Tool tool)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>(); // get a reference to this object's renderer
            // Loop over the animation frames with a delay of a third of a second
            foreach (var sprite in treeAnimation)
            {
                yield return new WaitForSeconds(GetAnimationDelay(tool));
                spriteRenderer.sprite = sprite;
            }
            
            player.IsGathering = false;
            Destroy(gameObject);
        }
        
        private IEnumerable<Sprite> GetAnimation()
        {
            // Try to find a Sprite[] using the sprite name
            // The correct dictionary key must match the sprite name
            var spriteName = GetComponent<SpriteRenderer>().sprite.name;
            
            foreach (var anim in treeAnimations)
            {
                if (anim.Key == spriteName) return anim.Value;
            }
            
            throw new KeyNotFoundException(); // if we're here, the dictionary is incomplete or something went wrong
        }
    }
}