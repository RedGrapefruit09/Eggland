using UnityEngine;
using UnityEngine.UI;

namespace Eggland.Tools
{
    public class ToolDurabilitySync : MonoBehaviour
    {
        private Player player;
        private Text text;

        private void Awake()
        {
            player = FindObjectOfType<Player>();
            text = GetComponent<Text>();
        }
        
        private void Update()
        {
            if (player.GetActiveTool() == null)
            {
                text.text = "";
                return;
            }

            var tool = player.GetActiveTool();
            var percentage = tool.CurrentDurability() * 100 / tool.durability;
            text.text = $"{percentage}%";
        }
    }
}