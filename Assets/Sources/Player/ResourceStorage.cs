using System;
using System.IO;
using UnityEngine;

namespace Eggland
{
    public class ResourceStorage : MonoBehaviour
    {
        public SerializedDictionary<ResourceType, int> state;

        private void Awake()
        {
            // Initialize
            state = new SerializedDictionary<ResourceType, int>
            {
                [ResourceType.WOOD] = 0,
                [ResourceType.COAL] = 0,
                [ResourceType.BRONZE] = 0,
                [ResourceType.IRON] = 0,
                [ResourceType.DIAMOND] = 0,
                [ResourceType.EMERALD] = 0,
                [ResourceType.BUSH_LEAF] = 0,
                [ResourceType.RUBY] = 0,
                [ResourceType.ROCK] = 0
            };
            
            ReadFromFile();
        }

        private void OnApplicationQuit()
        {
            WriteToFile();
        }

        public int Get(ResourceType type) => state[type];

        public void Add(ResourceType type, int amount) => state[type] += amount;

        public void Synchronize()
        {
            foreach (var obj in FindObjectsOfType<ResourceUICount>())
            {
                obj.Sync(state[obj.type]);
            }
        }

        private void ReadFromFile()
        {
            var path = $"{Application.persistentDataPath}/storage.json";
            if (!File.Exists(path)) return;
            
            var raw = File.ReadAllText(path);
            var obj = JsonUtility.FromJson<SerializedDictionary<ResourceType, int>>(raw);

            foreach (var pair in obj)
            {
                state[pair.Key] = pair.Value;
            }
        }

        private void WriteToFile()
        {
            var path = $"{Application.persistentDataPath}/storage.json";
            var raw = JsonUtility.ToJson(state, true);
            
            File.WriteAllText(path, raw);
        }
    }

    public enum ResourceType
    {
        WOOD,
        COAL,
        BRONZE,
        IRON,
        DIAMOND,
        EMERALD,
        RUBY,
        BUSH_LEAF,
        ROCK
    }

    [Serializable]
    public struct Range
    {
        public int min;
        public int max;
    }

    [Serializable]
    public struct SimplePair<TKey, TValue>
    {
        public TKey key;
        public TValue value;
    }
}