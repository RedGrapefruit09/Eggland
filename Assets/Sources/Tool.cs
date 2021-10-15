using System;
using UnityEngine;

namespace Eggland
{
    public class Tool : MonoBehaviour
    {
        public ToolType type;
        public int efficiency;
        public int durability;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public Sprite GetSprite() => spriteRenderer.sprite;
    }

    [Serializable]
    public enum ToolType
    {
        PICKAXE,
        AXE
    }
}