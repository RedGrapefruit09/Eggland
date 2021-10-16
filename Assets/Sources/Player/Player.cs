using System;
using System.Collections.Generic;
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

        // Look animations
        [Header("Animation")]
        [SerializeField] private Sprite upLookSprite;
        [SerializeField] private Sprite downLookSprite;
        [SerializeField] private Sprite rightLookSprite;
        [SerializeField] private Sprite leftLookSprite;
        
        // Tools
        [Header("Tools")]
        [SerializeField] private GameObject[] pickaxes;
        [SerializeField] private GameObject[] axes;

        #endregion

        #region Runtime properties

        private SpriteRenderer spriteRenderer;
        
        private float sprint; // current sprint value
        private Facing facing = Facing.DOWN;

        // Tools
        private GameObject currentPickaxePrefab;
        private GameObject currentAxePrefab;
        private int currentTool;
        private GameObject currentPickaxe;
        private GameObject currentAxe;
        
        // Gathering
        private GameObject gatherSelection;
        [HideInInspector] public bool IsGathering { get; set; }

        #endregion

        private void Awake()
        {
            sprint = maximalSprint; // initialize sprint
            spriteRenderer = GetComponent<SpriteRenderer>();

            currentPickaxePrefab = pickaxes[0];
            currentAxePrefab = axes[0];
            SetupTools();
        }

        private void Start()
        {
            ApplyFacing();
        }

        private void Update()
        {
            UpdateCracks();
            ControlMovement();
            
            ApplyToolFacing();
            SwitchTools();
            DisplayActiveTool();
            
            Gather();
        }

        #region Tools & Gathering

        private void SetupTools()
        {
            var axe = Instantiate(currentAxePrefab, transform);
            axe.SetActive(false);
            currentAxe = axe;

            var pickaxe = Instantiate(currentPickaxePrefab, transform);
            pickaxe.SetActive(false);
            currentPickaxe = pickaxe;

            currentTool = 0;
        }

        private void ApplyToolFacing()
        {
            if (facing == Facing.LEFT)
            {
                currentAxe.transform.localPosition = new Vector3(-0.2f, -0.1f, -5f);
                currentAxe.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 100f));
                currentAxe.GetComponent<SpriteRenderer>().flipY = false;
                
                currentPickaxe.transform.localPosition = new Vector3(-0.2f, -0.1f, -5f);
                currentPickaxe.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 100f));
                currentPickaxe.GetComponent<SpriteRenderer>().flipY = false;
            }
            else
            {
                currentAxe.transform.localPosition = new Vector3(0.15f, -0.115f, -5f);
                currentAxe.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 100f));
                currentAxe.GetComponent<SpriteRenderer>().flipY = true;
                
                currentPickaxe.transform.localPosition = new Vector3(0.15f, -0.115f, -5f);
                currentPickaxe.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 100f));
                currentPickaxe.GetComponent<SpriteRenderer>().flipY = true;
            }
        }
        
        private void DisplayActiveTool()
        {
            switch (currentTool)
            {
                case 0:
                    currentAxe.SetActive(false);
                    currentPickaxe.SetActive(false);
                    break;
                case 1:
                    currentAxe.SetActive(true);
                    currentPickaxe.SetActive(false);
                    break;
                default:
                    currentAxe.SetActive(false);
                    currentPickaxe.SetActive(true);
                    break;
            }
        }

        private void SwitchTools()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                currentTool = 0;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currentTool = 1;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currentTool = 2;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("GatherZone")) return;

            if (gatherSelection == null)
            {
                gatherSelection = other.gameObject.transform.parent.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("GatherZone")) return;

            if (gatherSelection != null) gatherSelection = null;
        }

        private Tool GetActiveTool()
        {
            return currentTool switch
            {
                1 => currentAxe.GetComponent<Tool>(),
                2 => currentPickaxe.GetComponent<Tool>(),
                _ => null
            };
        }
        
        private void Gather()
        {
            if (Input.GetKeyDown(KeyCode.G) && GetActiveTool() != null && gatherSelection != null)
            {
                var gatherable = gatherSelection.GetComponent<Gatherable>();

                if (gatherable != null)
                {
                    gatherable.Gather(GetActiveTool(), this);
                    gatherSelection = null;
                }
            }
        }
        
        #endregion

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
            // You can't move while gathering
            if (IsGathering) return;
            
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
                facing = Facing.UP;
            }

            if (Input.GetKey(KeyCode.S) && !downDetector.colliding)
            {
                transform.position += Vector3.down * movementSpeed * m * Time.deltaTime;
                facing = Facing.DOWN;
            }

            if (Input.GetKey(KeyCode.D) && !rightDetector.colliding)
            {
                transform.position += Vector3.right * movementSpeed * m * Time.deltaTime;
                facing = Facing.RIGHT;
            }

            if (Input.GetKey(KeyCode.A) && !leftDetector.colliding)
            {
                transform.position += Vector3.left * movementSpeed * m * Time.deltaTime;
                facing = Facing.LEFT;
            }
            
            ApplyFacing();
        }

        private void ApplyFacing()
        {
            // Apply to player
            var sprite = facing switch
            {
                Facing.UP => upLookSprite,
                Facing.DOWN => downLookSprite,
                Facing.LEFT => leftLookSprite,
                Facing.RIGHT => rightLookSprite,
                _ => throw new ArgumentOutOfRangeException()
            };

            spriteRenderer.sprite = sprite;
        }

        #endregion
    }

    public enum Facing
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
}
