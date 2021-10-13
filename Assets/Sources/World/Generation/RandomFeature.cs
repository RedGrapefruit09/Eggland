using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Eggland.World.Generation
{
    /// <summary>
    /// A complex, biome-dependent <see cref="Feature"/> for generating random objects across the game world.
    /// </summary>
    public class RandomFeature : Feature
    {
        [SerializeField] private GameObject mainPrefab; // barebones prefab to instantiate
        // Sprite collections
        [SerializeField] private Sprite[] regularSprites;
        [SerializeField] private Sprite[] autumnSprites;
        [SerializeField] private Sprite[] winterSprites;
        [SerializeField] private SerializedDictionary<Biome, int> chance; // a dictionary of chances to generate the object for each biome

        public override void Generate(Generator generator)
        {
            // For every tile
            for (var y = 1; y < generator.layerSize - 1; ++y)
            {
                for (var x = 1; x < generator.layerSize - 1; ++x)
                {
                    if (!ShouldGenerate(generator)) continue;

                    // Calculate position and check if something is at the position
                    var pos = CalculateTilePosition(x, y, generator, -1f);
                    
                    // Pick sprite
                    var sprite = RandomSprite(CurrentSpriteCollection(generator));
                    var doubleTile = sprite.bounds.size.y > 1f;
                    // Instantiate
                    if (doubleTile)
                    {
                        pos = new Vector3(pos.x, pos.y - 0.5f, pos.z); // also offset double tiles down a bit
                    }
                    var clone = Instantiate(mainPrefab, pos, Quaternion.identity);
                    clone.GetComponent<SpriteRenderer>().sprite = sprite;

                    // Register placement for later cleanup
                    RegisterPlacement(generator, clone);
                }
            }
        }

        // Randomizer
        private bool ShouldGenerate(Generator generator)
        {
            var r = (int)Random.Range(0f, 1000f);

            return r <= chance[generator.biome];
        }

        // Picks a Sprite[] for the current generator biome using a switch statement
        private Sprite[] CurrentSpriteCollection(Generator generator)
        {
            return generator.biome switch
            {
                Biome.SPRING => regularSprites,
                Biome.SUMMER => regularSprites,
                Biome.AUTUMN => autumnSprites,
                Biome.WINTER => winterSprites,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}