using System.Collections.Generic;
using UnityEngine;

namespace Eggland.Tools
{
    public class RepairManager : MonoBehaviour
    {
        private Dictionary<ResourceType, int> repairRequirements;

        private void Awake()
        {
            repairRequirements = new Dictionary<ResourceType, int>
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