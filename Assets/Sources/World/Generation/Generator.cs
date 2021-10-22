using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Eggland.World.Generation
{
    /// <summary>
    /// A generator is the central place for worldgen.
    /// The generator manages features, current biome and layer size, cleanup and world boundaries.
    /// Though it is designed to be lightweight, so most things can be separated into <see cref="Feature"/>s
    /// </summary>
    public class Generator : MonoBehaviour
    {
        public int layerSize; // the size of a generated layer. exposed to features
        public Biome biome; // the current biome to generate. exposed to features
        [SerializeField] private GameObject emptyPrefab; // an empty prefab for setting up world boundaries
        
        private readonly List<Feature> features = new List<Feature>(); // all registered features
        private readonly List<GameObject> placements = new List<GameObject>(); // all collected placements

        private void Awake()
        {
            RegisterFeatures();
        }

        private void Start()
        {
            Generate();
        }

        #region Registering

        /// <summary>
        /// Registers a placement <see cref="GameObject"/> for later recycling.
        /// <see cref="Feature"/> implementors can use <see cref="Feature"/>'s shortcut with the same name.
        /// </summary>
        /// <param name="placement">Placement <see cref="GameObject"/></param>
        public void RegisterPlacement(GameObject placement)
        {
            placements.Add(placement);
        }
        
        private void RegisterFeatures()
        {
            // Register all active features from code to avoid Unity serialization hell
            
            Register(GetComponent<GrassFeature>());
            Register(GetComponents<RandomFeature>());
            
            Debug.Log($"{features.Count} features registered.");
        }

        // Utils
        
        private void Register(Feature feature)
        {
            if (feature == null)
            {
                Debug.LogError("Missing feature. Worldgen might break!");
                return;
            }
            
            features.Add(feature);
        }

        private void Register(IEnumerable<Feature> list)
        {
            foreach (var feature in list)
            {
                Register(feature);
            }
        }

        #endregion

        #region Generation & Cleanup

        public void Generate()
        {
            Debug.Log($"World generation started. Layer size - {layerSize}, biome - {biome}");

            // Call all features
            foreach (var feature in features)
            {
                feature.Generate(this);
            }
            
            Debug.Log("Finished world generation.");
            CreateWorldBoundaries(); // afterwards, create world boundary objects preventing the player from going off-map
            Debug.Log("Created world boundaries.");
        }

        public void Clean()
        {
            // Destroy all placements in the list
            foreach (var placement in placements.Where(placement => placement != null)) Destroy(placement);
            placements.Clear();

            Debug.Log("Cleanup successful");
        }

        #endregion
        
        private void CreateWorldBoundaries()
        {
            // Left boundary
            CreateBoundary(new Vector2(-((float)layerSize / 2 + 0.5f), 0f), new Vector2(1f, layerSize));
            // Right boundary
            CreateBoundary(new Vector2((float)layerSize / 2 + 0.5f, 0f), new Vector2(1f, layerSize));
            // Top boundary
            CreateBoundary(new Vector2(0f, (float)layerSize / 2 + 0.5f), new Vector2(layerSize, 1f));
            // Bottom boundary
            CreateBoundary(new Vector2(0f, -((float)layerSize / 2 + 0.5f)), new Vector2(layerSize, 1f));
        }

        private void CreateBoundary(Vector2 pos, Vector2 colliderSize)
        {
            // Instantiate a blank GameObject and add a BoxCollider2D
            var clone = Instantiate(emptyPrefab, pos, Quaternion.identity);
            clone.AddComponent<BoxCollider2D>();
            // Eliminate the shape offset and set the correct shape size
            var collider = clone.GetComponent<BoxCollider2D>();
            collider.offset = Vector2.zero;
            collider.size = colliderSize;
        }

        public bool ShouldGameOver() => biome == Biome.WINTER;

        public Biome NextBiome()
        {
            return biome switch
            {
                Biome.SPRING => Biome.SUMMER,
                Biome.SUMMER => Biome.AUTUMN,
                Biome.AUTUMN => Biome.WINTER,
                Biome.WINTER => throw new ApplicationException(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}