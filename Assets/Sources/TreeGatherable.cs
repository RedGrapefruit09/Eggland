using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eggland
{
    public class TreeGatherable : Gatherable
    {
        [SerializeField] private SerializedDictionary<string, Sprite[]> treeAnimations;

        public override void Gather(Tool tool, Player player)
        {
            if (tool.type == type)
            {
                player.IsGathering = true;
                StartCoroutine(TreeAnimation(GetAnimation(), player));
            }
        }

        private IEnumerator TreeAnimation(IEnumerable<Sprite> treeAnimation, Player player)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            
            foreach (var sprite in treeAnimation)
            {
                yield return new WaitForSeconds(0.3f);
                spriteRenderer.sprite = sprite;
            }
            
            player.IsGathering = false;
            Destroy(gameObject);
        }
        
        private IEnumerable<Sprite> GetAnimation()
        {
            var spriteName = GetComponent<SpriteRenderer>().sprite.name;
            
            foreach (var anim in treeAnimations)
            {
                if (anim.Key == spriteName) return anim.Value;
            }
            
            throw new KeyNotFoundException();
        }
    }
}