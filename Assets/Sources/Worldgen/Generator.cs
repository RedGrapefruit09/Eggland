using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eggland.Worldgen
{
    public class Generator : MonoBehaviour
    {
        public int layerSize;
        public Biome biome;
        [SerializeField] private GameObject emptyPrefab;
        
        private readonly List<Feature> features = new List<Feature>();
        private readonly List<Placement> placements = new List<Placement>();

        private void Awake()
        {
            RegisterFeatures();
        }

        private void Start()
        {
            StartCoroutine(InfGen());
        }

        private IEnumerator InfGen()
        {
            while (true)
            {
                Generate();
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.G));
                Clean();
            }
        }

        #region Registering

        public void RegisterPlacement(Placement placement)
        {
            placements.Add(placement);
        }
        
        private void RegisterFeatures()
        {
            Register(GetComponent<GrassFeature>());
            Register(GetComponents<RandomFeature>());
            
            Debug.Log($"{features.Count} features registered.");
        }

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

        private void Generate()
        {
            Debug.Log($"World generation started. Layer size - {layerSize}, biome - {biome}");

            foreach (var feature in features)
            {
                feature.Generate(this);
            }
            
            Debug.Log("Finished world generation.");
            
            CreateWorldBoundaries();
            
            Debug.Log("Created world boundaries.");
        }

        private void Clean()
        {
            foreach (var placement in placements)
            {
                if (placement.GameObject != null)
                {
                    Destroy(placement.GameObject);
                }
            }

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
    }
}