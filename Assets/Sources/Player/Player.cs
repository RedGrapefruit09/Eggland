using UnityEngine;

namespace Eggland
{
    /// <summary>
    /// The main player script
    /// </summary>
    public class Player : MonoBehaviour
    {
        #region Serialized properties

        // Health-related settings
        [Header("Health")]
        [SerializeField] private GameObject cracksObject;
        [SerializeField] private int health = 7;
        [SerializeField] private SerializedDictionary<int, Sprite> cracks;

        // Movement
        [Header("Movement")]
        [SerializeField] private float movementSpeed; // the speed multiplier of each movement
        // References to all collision detectors in the right order
        [SerializeField] private CollisionDetector upDetector; 
        [SerializeField] private CollisionDetector downDetector;
        [SerializeField] private CollisionDetector leftDetector;
        [SerializeField] private CollisionDetector rightDetector;

        // Sprint
        [Header("Sprinting")]
        [SerializeField] private float sprintMultiplier; // by how much sprinting multiplies the base speed
        [SerializeField] private float sprintUsage; // how much sprint is used per tick
        [SerializeField] private float sprintRegain; // how much sprint can be regained per tick when not sprinting
        [SerializeField] private float minimalSprint; // the minimal sprint boundary
        [SerializeField] private float maximalSprint; // the maximal sprint boundary. the player starts with this value

        // Crouch
        [Header("Crouching")]
        [SerializeField] private float crouchMultiplier; // by how much crouching multiplies the base speed

        #endregion

        #region Runtime properties

        private float sprint; // current sprint value

        #endregion

        private void Awake()
        {
            sprint = maximalSprint; // initialize sprint
        }

        private void Update()
        {
            UpdateCracks();
            ControlMovement();
        }

        #region Health

        private void UpdateCracks()
        {
            // Ignore if health is at its normal value
            if (health >= 7) return;

            // If there's no health left, exit out of the game
            // TODO: Later replace this with a retry screen
            if (health == 0)
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                return;
            }

            // In the case if health is in the range between 1 to 6, set the crack overlay to the necessary sprite
            // This will emulate the effect that the egg cracks, but it's actually an overlay, so extra animations are
            // unnecessary
            cracksObject.GetComponent<SpriteRenderer>().sprite = cracks[health];
        }

        #endregion

        #region Movement

        private void ControlMovement()
        {
            var sprinted = false; // store for sprint regain

            // Check for crouching
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                Move(crouchMultiplier);
            }
            else
            {
                // Check for sprinting AND the ability to currently sprint
                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
                    sprint >= minimalSprint)
                {
                    // Sprint, count in the usage and mark not to regain any sprint later on
                    Move(sprintMultiplier);
                    sprint -= sprintUsage;
                    sprinted = true;
                }
                else
                {
                    // If nothing of the above matched, move normally
                    Move();
                }
            }

            if (sprinted) return;
            
            sprint += sprintRegain; // regain sprint
            if (sprint > maximalSprint) sprint = maximalSprint; // clamp the value afterwards
        }

        private void Move(float m = 1f)
        {
            // Standard movement pattern with WASD, but with an extra m multiplier passed in
            
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
}
