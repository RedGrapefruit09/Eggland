using System.Collections.Generic;
using UnityEngine;

namespace Eggland
{
    /// <summary>
    /// A custom gatherable script for rocks, supporting overlays for small, medium and large types of rocks.
    /// </summary>
    public class RockGatherable : Gatherable
    {
        // Overlay sprites for every type of a rock
        [SerializeField] private Sprite[] smallOverlays;
        [SerializeField] private Sprite[] mediumOverlays;
        [SerializeField] private Sprite[] largeOverlays;
        
        public override void Gather(Tool tool, Player player)
        {
            // Do the same as a normal gatherable, but pass a custom Sprite[] into the coroutine
            if (tool.type == type)
            {
                player.IsGathering = true;
                StartCoroutine(OverlayAnimation(GetOverlays(), player, tool));
            }
        }

        private Sprite[] GetOverlays()
        {
            // Check, which type of rock is this by looking at the prefix of the sprite's name
            var spriteName = GetComponent<SpriteRenderer>().sprite.name;
            
            if (spriteName.StartsWith("small")) return smallOverlays;
            if (spriteName.StartsWith("medium")) return mediumOverlays;
            if (spriteName.StartsWith("large")) return largeOverlays;
            
            throw new KeyNotFoundException(); // if no pattern matches, something clearly went wrong
        }
    }
}