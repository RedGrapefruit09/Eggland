using System;
using UnityEngine;

namespace Eggland
{
    /// <summary>
    /// A tool is required to gather a gatherable.
    ///
    /// The actual script is only used for storing data with no extra functionality being added.
    /// </summary>
    public class Tool : MonoBehaviour
    {
        public ToolType type;
        public int efficiency;
        public int durability;
    }

    /// <summary>
    /// ToolTypes determine whether the tool is an example of an axe or a pickaxe.
    /// </summary>
    [Serializable]
    public enum ToolType
    {
        PICKAXE,
        AXE
    }
}