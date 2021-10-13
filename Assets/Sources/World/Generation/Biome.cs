using System;

namespace Eggland.World.Generation
{
    /// <summary>
    /// The current in-game biome.
    ///
    /// You start in the <see cref="SPRING"/> biome, and move downwards.
    /// </summary>
    [Serializable]
    public enum Biome
    {
        SPRING,
        SUMMER,
        AUTUMN,
        WINTER
    }
}