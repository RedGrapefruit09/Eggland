using UnityEngine;
using UnityEngine.UI;

namespace Eggland
{
    public class ResourceUICount : MonoBehaviour
    {
        public ResourceType type;
        private Text text;

        private void Awake()
        {
            text = GetComponent<Text>();
        }

        public void Sync(int amount)
        {
            text.text = amount.ToString();
        }
    }
}