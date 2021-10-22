using System.IO;
using UnityEngine;

namespace Eggland.Tools
{
    public class RepairManager : MonoBehaviour
    {
        private SerializedDictionary<ResourceType, int> repairRequirements;

        private void Awake()
        {
            repairRequirements = new SerializedDictionary<ResourceType, int>
            {
                [ResourceType.BRONZE] = 1,
                [ResourceType.IRON] = 1,
                [ResourceType.DIAMOND] = 1,
                [ResourceType.EMERALD] = 1,
                [ResourceType.RUBY] = 1
            };
        }

        public int GetRequirement(ResourceType type) => repairRequirements[type];
        public void IncrementRequirement(ResourceType type) => repairRequirements[type] += 1;
    }
}