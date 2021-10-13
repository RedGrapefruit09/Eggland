using UnityEngine;
using Random = UnityEngine.Random;

namespace Eggland.Worldgen
{
    public abstract class Feature : MonoBehaviour
    {
        public abstract string ID { get; }

        public abstract void Generate(Generator generator);

        protected static Vector3 CalculateTilePosition(int x, int y, Generator generator, float z = 0f)
        {
            return new Vector3(x - (float)generator.layerSize / 2 + 0.5f, y - (float)generator.layerSize / 2 + 0.5f, z);
        }

        protected static void RegisterPlacement(Generator generator, GameObject placement, Vector3 pos, bool isGrass = false, bool isDoubleTile = false)
        {
            generator.RegisterPlacement(new Placement(placement, pos, isGrass, isDoubleTile));
        }

        protected static Sprite RandomSprite(Sprite[] array)
        {
            return array[(int)Random.Range(0f, array.Length)];
        }
    }
}