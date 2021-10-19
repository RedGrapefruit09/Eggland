using System;
using System.Collections;
using System.Collections.Generic;
using Eggland.Gathering;
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

        // UI
        [Header("UI")] 
        [SerializeField] private GameObject resourceStorageUi;
        [SerializeField] private GameObject upgradeButton;

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
        private UpgradeManager upgradeManager;
        
        // Gathering
        private GameObject gatherSelection;
        [HideInInspector] public bool IsGathering { get; set; }

        #endregion

        private void Awake()
        {
            cracksObject.GetComponent<SpriteRenderer>().sprite = null;
            upgradeManager = FindObjectOfType<UpgradeManager>();
            
            sprint = maximalSprint; // initialize sprint
            spriteRenderer = GetComponent<SpriteRenderer>();

            currentPickaxePrefab = upgradeManager.GetPickaxePrefab(0);
            currentAxePrefab = upgradeManager.GetAxePrefab(0);
            SetupTools();
        }

        private void Start()
        {
            ApplyFacing();
        }

        private void Update()
        {
            if (!resourceStorageUi.activeSelf)
            {
                UpdateCracks();
                ControlMovement();

                ApplyToolFacing();
                SwitchTools();
                DisplayActiveTool();

                Gather();
                
                DisplayUpgradeUI();
                UpgradeWithHotkey();
            }
            else
            {
                upgradeButton.SetActive(false); // upgrade button is unavailable while interacting with other UI
            }

            HandleResourceStorage();
        }

        #region Tools & Gathering

        // setup the tool prefabs as children of this object
        private void SetupTools(bool resetCurrentTool = true)
        {
            // instantiate the prefabs, deactivate them and assign references

            var axe = Instantiate(currentAxePrefab, transform);
            axe.SetActive(false);
            currentAxe = axe;

            var pickaxe = Instantiate(currentPickaxePrefab, transform);
            pickaxe.SetActive(false);
            currentPickaxe = pickaxe;

            if (resetCurrentTool) currentTool = 0; // set current tool to be none/0
        }

        // rotate and position the tools according to the player's facing
        private void ApplyToolFacing()
        {
            // usual tool facing mustn't be applied during gathering, since that'd break the gathering animation
            if (IsGathering) return;
            
            if (facing == Facing.LEFT)
            {
                if (currentAxe != null)
                {
                    currentAxe.transform.localPosition = new Vector3(-0.2f, -0.1f, -5f);
                    currentAxe.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 100f));
                    currentAxe.GetComponent<SpriteRenderer>().flipY = false;
                }

                if (currentPickaxe != null)
                {
                    currentPickaxe.transform.localPosition = new Vector3(-0.2f, -0.1f, -5f);
                    currentPickaxe.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 100f));
                    currentPickaxe.GetComponent<SpriteRenderer>().flipY = false;
                }
            }
            else
            {
                if (currentAxe != null)
                {
                    currentAxe.transform.localPosition = new Vector3(0.15f, -0.115f, -5f);
                    currentAxe.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 100f));
                    currentAxe.GetComponent<SpriteRenderer>().flipY = true;
                }

                if (currentPickaxe != null)
                {
                    currentPickaxe.transform.localPosition = new Vector3(0.15f, -0.115f, -5f);
                    currentPickaxe.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 100f));
                    currentPickaxe.GetComponent<SpriteRenderer>().flipY = true;
                }
            }
        }
        
        // activate the current active tool and deactivate all others in the scene
        private void DisplayActiveTool()
        {
            switch (currentTool)
            {
                case 0:
                    if (currentAxe != null) currentAxe.SetActive(false);
                    if (currentPickaxe != null) currentPickaxe.SetActive(false);
                    break;
                case 1:
                    if (currentAxe != null) currentAxe.SetActive(true);
                    if (currentPickaxe != null) currentPickaxe.SetActive(false);
                    break;
                default:
                    if (currentAxe != null) currentAxe.SetActive(false);
                    if (currentPickaxe != null) currentPickaxe.SetActive(true);
                    break;
            }
        }

        // provides keybinds that allow the player to switch their current tool
        // 0 => no tool
        // 1 => axe
        // 2 => pickaxe
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

            // include the zone's parent into the gathering selection
            if (gatherSelection == null)
            {
                gatherSelection = other.gameObject.transform.parent.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("GatherZone")) return;

            // exclude the zone's parent out of the gathering selection
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (gatherSelection != null) gatherSelection = null;
        }

        // returns the reference to the tool script of the current used tool
        // if the player hasn't a tool equipped, a null reference is returned
        private Tool GetActiveTool()
        {
            return currentTool switch
            {
                1 => currentAxe != null ? currentAxe.GetComponent<Tool>() : null,
                2 => currentPickaxe != null ? currentPickaxe.GetComponent<Tool>() : null,
                _ => null
            };
        }

        private bool HasTool(Gatherable gatherable)
        {
            var reference = gatherable.type == ToolType.AXE ? currentAxe : currentPickaxe;
            return reference != null;
        }

        // starts off the gathering mechanism on the G keybind
        private void Gather()
        {
            // also check if a tool is equipped and an object is in the gathering selection
            if (!Input.GetKeyDown(KeyCode.G) || GetActiveTool() == null || gatherSelection == null) return;
            
            var gatherable = gatherSelection.GetComponent<Gatherable>();
            if (!HasTool(gatherable) || gatherable == null) return;
            
            gatherable.Gather(GetActiveTool(), this); // trigger gathering
            gatherSelection = null; // empty the gathering selection
            StartCoroutine(PlayGatheringAnimation()); // trigger the animation coroutine
        }

        // a coroutine that plays a little animation for the current tool
        // the animation consists of the tool rotating up and down while the gathering is still in process
        private IEnumerator PlayGatheringAnimation()
        {
            var tool = GetActiveTool().gameObject; // get the tool
            var repeats = currentTool == 1 ? 35 : 20; // rotate the axe 35 degrees and the pickaxe 20 degrees
            
            while (IsGathering) // repeat the animation until the gathering process is finished
            {
                // rotate upwards by 2 degrees with a 1/100 second delay
                for (var i = 0; i < repeats; ++i)
                {
                    tool.transform.Rotate(0f, 0f, 2f);
                    yield return new WaitForSeconds(0.01f);
                }

                // then, rotate downwards with the same action pattern
                for (var i = 0; i < repeats; ++i)
                {
                    tool.transform.Rotate(0f, 0f, -2f);
                    yield return new WaitForSeconds(0.01f);
                }
            }
            
            GetActiveTool().OnUse();
        }

        #endregion

        #region Tool upgrades

        public void NotifyDestroyed(GameObject tool)
        {
            if (tool == currentAxe) currentAxe = null;
            if (tool == currentPickaxe) currentPickaxe = null;

            currentTool = 0;
        }
        
        public bool CanUpgrade(ToolType type)
        {
            return type == ToolType.AXE ? currentAxe != null : currentPickaxe != null;
        }

        private GameObject GetToolPrefabOfType(ToolType type) =>
            type == ToolType.AXE ? currentAxePrefab : currentPickaxePrefab;

        public void Upgrade() => UpgradeInternal(GetActiveTool().type);

        private void UpgradeInternal(ToolType type)
        {
            // Resolve the next index in the prefab array for upgrading
            var list = upgradeManager.GetPrefabArray(type);
            var prefab = GetToolPrefabOfType(type);
            var currentIndex = IndexOf(list, prefab);
            var nextIndex = currentIndex + 1;
            
            // Return if the tool is already maxed out or cannot upgrade
            if (nextIndex >= list.Length) return;
            if (!upgradeManager.CanUpgrade(currentIndex, type)) return;
            upgradeManager.CountUpgrade(currentIndex, type);

            // Get the next prefab and deactivate the current tool's GameObject
            var nextPrefab = list[nextIndex];
            Destroy(GetActiveTool().gameObject);

            // Refresh references
            if (type == ToolType.AXE)
            {
                currentAxe = null;
                currentAxePrefab = nextPrefab;
            }
            else
            {
                currentPickaxe = null;
                currentPickaxePrefab = nextPrefab;
            }
            
            // Call setup
            SetupTools(false);
        }

        private void DisplayUpgradeUI()
        {
            if (GetActiveTool() == null)
            {
                upgradeButton.SetActive(false);
                return;
            }
            
            var type = GetActiveTool().type;

            int ActiveToolIndex()
            {
                var list = upgradeManager.GetPrefabArray(type);
                var prefab = GetToolPrefabOfType(type);

                return IndexOf(list, prefab);
            }

            upgradeButton.SetActive(upgradeManager.CanUpgrade(ActiveToolIndex(), type));
        }

        private void UpgradeWithHotkey()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.U)) Upgrade();
        }

        public void InitToolsFromSerialized()
        {
            currentAxe = null;
            currentPickaxe = null;
            
            currentAxePrefab = upgradeManager.GetAxePrefab(upgradeManager.AxeLevel);
            currentPickaxePrefab = upgradeManager.GetPickaxePrefab(upgradeManager.PickaxeLevel);
            
            SetupTools(false);
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
            if (IsGathering) return; // block movement during gathering to avoid animation oddness
            
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

        // applies the correct facing sprite to the player's renderer
        private void ApplyFacing()
        {
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

        #region UI

        private void HandleResourceStorage()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            {
                resourceStorageUi.SetActive(!resourceStorageUi.activeSelf);
                
                if (resourceStorageUi.activeSelf) FindObjectOfType<ResourceStorage>().Synchronize();
            }
        }

        #endregion

        private static int IndexOf<T>(IReadOnlyList<T> array, T element)
        {
            for (var i = 0; i < array.Count; ++i)
            {
                if (array[i].Equals(element)) return i;
            }

            throw new KeyNotFoundException();
        }
    }

    public enum Facing
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
}
