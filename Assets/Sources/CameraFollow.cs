using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform _player;
    
    private void Awake()
    {
        _player = FindObjectOfType<Player>().transform;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(_player.position.x, _player.position.y, transform.position.z);
    }
}
