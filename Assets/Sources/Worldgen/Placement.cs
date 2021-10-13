using UnityEngine;

namespace Eggland.Worldgen
{
    public readonly struct Placement
    {
        public readonly GameObject GameObject;
        public readonly Vector3 Coordinate;
        public readonly bool IsGrass;
        public readonly bool IsDoubleTile;

        public Placement(GameObject gameObject, Vector3 coordinate, bool isGrass, bool isDoubleTile)
        {
            GameObject = gameObject;
            Coordinate = coordinate;
            IsGrass = isGrass;
            IsDoubleTile = isDoubleTile;
        }
    }
}