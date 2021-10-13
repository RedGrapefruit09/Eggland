using UnityEngine;

namespace Eggland
{
    /// <summary>
    /// A small script that updates the camera's position according to the player's position.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        private Transform player;

        private void Awake()
        {
            player = FindObjectOfType<Player>().transform;
        }

        private void LateUpdate()
        {
            // The Z coordinate mustn't be affected
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
    }
}
