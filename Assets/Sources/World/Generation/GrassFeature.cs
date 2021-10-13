using UnityEngine;

namespace Eggland.World.Generation
{
    /// <summary>
    /// A non-randomized <see cref="Feature"/> used to generate a flat plane of grass.
    ///
    /// The grass sprites vary between biomes.
    /// </summary>
    public class GrassFeature : Feature
    {
        [SerializeField] private GameObject grassPrefab; // grass prefab reference
        [SerializeField] private SerializedDictionary<Biome, Sprite> grassSprites; // a dictionary of grass sprites for every biome

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
                    // Register placement for removal
                    RegisterPlacement(generator, clone);
                }
            }
        }
    }
}