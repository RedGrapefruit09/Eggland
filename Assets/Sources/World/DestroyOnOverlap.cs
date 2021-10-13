using UnityEngine;

namespace Eggland.World
{
    /// <summary>
    /// This script avoids tiles that go in each other's bounds by destroying one of the tiles.
    /// </summary>
    public class DestroyOnOverlap : MonoBehaviour
    {
        // The immunity parameter is needed, so only one tile is removed instead of two
        [HideInInspector] public bool immune;
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            // Skip if colliding with the player or if the script has been given immunity
            if (other.gameObject.CompareTag("Player") || immune) return;
            
            // If there's another prop, set it to be immune, so only one tile is removed instead of two
            var otherComponent = other.gameObject.GetComponent<DestroyOnOverlap>();
            if (otherComponent)
            {
                otherComponent.immune = true;
            }

            // Self-destruct
            Destroy(gameObject);
        }
    }
}