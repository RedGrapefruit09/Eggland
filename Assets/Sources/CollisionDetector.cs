using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public bool colliding;

    private void OnTriggerEnter2D(Collider2D other)
    {
        colliding = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        colliding = false;
    }
}