using System.Collections;
using Eggland.World.Generation;
using UnityEngine;

public class Grass : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<Biome, Sprite[]> falloutAnimations;
    [SerializeField] private float falloutSpeed;
    
    private Generator generator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        generator = FindObjectOfType<Generator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartFallout()
    {
        StartCoroutine(Fallout());
    }

    private IEnumerator Fallout()
    {
        var falloutAnimation = falloutAnimations[generator.biome];

        foreach (var frame in falloutAnimation)
        {
            spriteRenderer.sprite = frame;
            yield return new WaitForSeconds(falloutSpeed);
        }
        
        Destroy(gameObject);
    }
}
