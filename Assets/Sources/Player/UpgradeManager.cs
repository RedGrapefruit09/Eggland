using Eggland.Gathering;
using UnityEngine;

namespace Eggland
{
    public class UpgradeManager : MonoBehaviour
    {
        [SerializeField] private SimplePair<ResourceType, int>[] pickaxeUpgrades;
        [SerializeField] private SimplePair<ResourceType, int>[] axeUpgrades;
        [SerializeField] private GameObject[] axes;
        [SerializeField] private GameObject[] pickaxes;

        private ResourceStorage storage;

        private void Awake()
        {
            storage = FindObjectOfType<ResourceStorage>();
        }
        
        public GameObject GetAxePrefab(int index) => axes[index];
        public GameObject GetPickaxePrefab(int index) => pickaxes[index];
        public GameObject[] GetPrefabArray(ToolType type) => type == ToolType.AXE ? axes : pickaxes;

        public bool CanUpgrade(int index, ToolType type)
        {
            var upgrade = type == ToolType.AXE ? axeUpgrades[index] : pickaxeUpgrades[index];
            var storedAmount = storage.Get(upgrade.key);

            if (storedAmount < upgrade.value) return false;

            storage.Use(upgrade.key, upgrade.value);
            
            return true;
        }
    }
}