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
            ReadFromFile();
        }

        private void OnApplicationQuit()
        {
            WriteToFile();
        }

        public int GetRequirement(ResourceType type) => repairRequirements[type];
        public void IncrementRequirement(ResourceType type) => repairRequirements[type] += 1;

        private void ReadFromFile()
        {
            var path = $"{Application.persistentDataPath}/repairs.json";
            if (!File.Exists(path)) return;

            var raw = File.ReadAllText(path);
            var obj = JsonUtility.FromJson<SerializedDictionary<ResourceType, int>>(raw);

            foreach (var pair in obj)
            {
                repairRequirements[pair.Key] = pair.Value;
            }
        }

        private void WriteToFile()
        {
            var path = $"{Application.persistentDataPath}/repairs.json";
            var raw = JsonUtility.ToJson(repairRequirements, true);
            
            File.WriteAllText(path, raw);
        }
    }
}