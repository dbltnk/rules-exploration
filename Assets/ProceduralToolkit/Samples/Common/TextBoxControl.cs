using UnityEngine;
using UnityEngine.UI;

namespace ProceduralToolkit.Samples.UI
{
    public class TextBoxControl : MonoBehaviour
    {
        public Text headerText;

        public void Initialize(string header)
        {
            name = header;
            headerText.text = header;
        }
    }
}
