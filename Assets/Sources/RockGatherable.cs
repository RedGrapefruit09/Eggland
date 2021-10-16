using System.Collections.Generic;
using UnityEngine;

namespace Eggland
{
    public class RockGatherable : Gatherable
    {
        [SerializeField] private Sprite[] smallOverlays;
        [SerializeField] private Sprite[] mediumOverlays;
        [SerializeField] private Sprite[] largeOverlays;
        
        public override void Gather(Tool tool)
        {
            if (tool.type == type) StartCoroutine(OverlayAnimation(GetOverlays()));
        }

        private Sprite[] GetOverlays()
        {
            var spriteName = GetComponent<SpriteRenderer>().sprite.name;

            if (spriteName.StartsWith("small")) return smallOverlays;
            if (spriteName.StartsWith("medium")) return mediumOverlays;
            if (spriteName.StartsWith("large")) return largeOverlays;

            throw new KeyNotFoundException();
        }
    }
}