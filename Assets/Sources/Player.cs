using UnityEngine;

public class Player : MonoBehaviour
{
    #region Settings

    [Header("Health")]
    [SerializeField] private GameObject cracksObject;
    [SerializeField] private int health = 7;
    [SerializeField] private SerializedDictionary<int, Sprite> cracks;

    [Header("Movement")]
    [SerializeField] private float movementSpeed;

    [SerializeField] private CollisionDetector upDetector;
    [SerializeField] private CollisionDetector downDetector;
    [SerializeField] private CollisionDetector leftDetector;
    [SerializeField] private CollisionDetector rightDetector;

    [Header("Sprinting")]
    [SerializeField] private float sprintMultiplier;
    [SerializeField] private float sprintUsage;
    [SerializeField] private float sprintRegain;
    [SerializeField] private float minimalSprint;
    [SerializeField] private float maximalSprint;
    
    [Header("Crouching")]
    [SerializeField] private float crouchMultiplier;

    #endregion

    private float sprint;

    private void Awake()
    {
        sprint = maximalSprint;
    }

    private void Update()
    {
        UpdateCracks();
        ControlMovement();
    }

    #region Health

    private void UpdateCracks()
    {
        if (health >= 7) return;
        
        if (health == 0)
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            return;
        }

        cracksObject.GetComponent<SpriteRenderer>().sprite = cracks[health];
    }

    #endregion

    #region Movement

    private void ControlMovement()
    {
        var sprinted = false;
        
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            Move(crouchMultiplier);
        }
        else
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && sprint >= minimalSprint)
            {
                Move(sprintMultiplier);
                sprint -= sprintUsage;
                sprinted = true;
            }
            else
            {
                Move();
            }
        }

        if (!sprinted)
        {
            sprint += sprintRegain;

            if (sprint > maximalSprint) sprint = maximalSprint;
        }
    }
    
    private void Move(float m = 1f)
    {
        if (Input.GetKey(KeyCode.W) && !upDetector.colliding)
        {
            transform.position += Vector3.up * movementSpeed * m * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S) && !downDetector.colliding)
        {
            transform.position += Vector3.down * movementSpeed * m * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D) && !rightDetector.colliding)
        {
            transform.position += Vector3.right * movementSpeed * m * Time.deltaTime;
        }
        
        if (Input.GetKey(KeyCode.A) && !leftDetector.colliding)
        {
            transform.position += Vector3.left * movementSpeed * m * Time.deltaTime;
        }
    }

    #endregion
}

public enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}
