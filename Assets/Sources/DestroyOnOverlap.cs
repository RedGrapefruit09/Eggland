using UnityEngine;

namespace Eggland
{
    public class DestroyOnOverlap : MonoBehaviour
    {
        [HideInInspector] public bool immune;
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") || immune) return;
            
            // If there's another prop, set it to be immune, so only one tile is removed instead of two
            var otherComponent = other.gameObject.GetComponent<DestroyOnOverlap>();
            if (otherComponent)
            {
                otherComponent.immune = true;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Self-destruct
            Destroy(gameObject);
        }
    }
}