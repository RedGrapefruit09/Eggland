using System;
using UnityEngine;

namespace Eggland
{
    public class ResourceStorage : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<ResourceType, int> state;

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
        }

        public int Get(ResourceType type) => state[type];
        public void Add(ResourceType type, int amount) => state[type] += amount;
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