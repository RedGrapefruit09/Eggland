using UnityEngine;

namespace Eggland
{
    /// <summary>
    /// A <see cref="CollisionDetector"/> is a script that tracks trigger collisions within its bounds.
    /// </summary>
    public class CollisionDetector : MonoBehaviour
    {
        // Whether the object is currently colliding
        [HideInInspector] public bool colliding;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("GatherZone")) return; // ignore gatherzones
            
            colliding = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("GatherZone")) return; // ignore gatherzones
            
            colliding = false;
        }
    }
}
