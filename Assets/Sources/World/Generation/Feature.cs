using UnityEngine;
using Random = UnityEngine.Random;

namespace Eggland.World.Generation
{
    /// <summary>
    /// A <see cref="Feature"/> represents any pattern that can generate inside the game world.
    /// </summary>
    public abstract class Feature : MonoBehaviour
    {
        /// <summary>
        /// Generate the pattern into the scene.
        /// </summary>
        /// <param name="generator">Generator instance.</param>
        public abstract void Generate(Generator generator);

        /// <summary>
        /// A utility provided to implementors for quickly calculating an absolute tile position from
        /// a relative grid position.
        /// </summary>
        /// <param name="x">Grid X</param>
        /// <param name="y">Grid Y</param>
        /// <param name="generator">Generator instance, passed in from <see cref="Generate"/></param>
        /// <param name="z">Z value (optional)</param>
        /// <returns>A <see cref="Vector3"/> representing the absolute tile position</returns>
        protected static Vector3 CalculateTilePosition(int x, int y, Generator generator, float z = 0f)
        {
            return new Vector3(x - (float)generator.layerSize / 2 + 0.5f, y - (float)generator.layerSize / 2 + 0.5f, z);
        }

        /// <summary>
        /// Registers a placement into the <see cref="Generator"/> for later cleanup
        /// </summary>
        /// <param name="generator">Generator instance.</param>
        /// <param name="placement">Placed <see cref="GameObject"/></param>
        protected static void RegisterPlacement(Generator generator, GameObject placement)
        {
            generator.RegisterPlacement(placement);
        }

        /// <summary>
        /// A simple utility picking out a random <see cref="Sprite"/> out of an array.
        /// </summary>
        /// <param name="array">The <see cref="Sprite"/> array</param>
        /// <returns>The picked out <see cref="Sprite"/></returns>
        protected static Sprite RandomSprite(Sprite[] array)
        {
            return array[(int)Random.Range(0f, array.Length)];
        }
    }
}