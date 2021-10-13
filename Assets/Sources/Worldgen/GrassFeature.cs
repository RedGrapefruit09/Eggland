using UnityEngine;

namespace Eggland.Worldgen
{
    public class GrassFeature : Feature
    {
        // Base grass prefab
        [SerializeField] private GameObject grassPrefab;
        // A dictionary of grass sprites for every biome
        [SerializeField] private SerializedDictionary<Biome, Sprite> grassSprites;

        public override string ID => "grass";
        
        public override void Generate(Generator generator)
        {
            // For every tile
            for (var y = 0; y < generator.layerSize; ++y)
            {
                for (var x = 0; x < generator.layerSize; ++x)
                {
                    // Calculate position and instantiate
                    var pos = CalculateTilePosition(x, y, generator);
                    var clone = Instantiate(grassPrefab, pos, Quaternion.identity);
                    // Assign sprite
                    clone.GetComponent<SpriteRenderer>().sprite = grassSprites[generator.biome];
                    // Register placement for removal on next generation
                    RegisterPlacement(generator, clone, pos, true);
                }
            }
        }
    }
}