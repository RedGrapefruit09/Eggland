using System;
using UnityEngine;

namespace Eggland.Tools
{
    /// <summary>
    /// A tool is required to gather a gatherable.
    ///
    /// The actual script is only used for storing data with no extra functionality being added.
    /// </summary>
    public class Tool : MonoBehaviour
    {
        public ToolType type;
        public int efficiency;
        public int durability;

        [SerializeField] private int currentDurability;
        private Player player;

        private void Awake()
        {
            currentDurability = durability;
            player = FindObjectOfType<Player>();
        }

        private void Update()
        {
            if (currentDurability >= 5) return;
            
            player.NotifyDestroyed(gameObject);
            Destroy(gameObject);
        }
        
        public void OnUse()
        {
            currentDurability -= 25;
        }

        public void Repair()
        {
            currentDurability += 50;
            if (currentDurability > durability) currentDurability = durability;
        }

        public int CurrentDurability() => currentDurability;
    }

    /// <summary>
    /// ToolTypes determine whether the tool is an example of an axe or a pickaxe.
    /// </summary>
    [Serializable]
    public enum ToolType
    {
        PICKAXE,
        AXE
    }
}