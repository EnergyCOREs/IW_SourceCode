using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.SimpleLocalization
{
    /// <summary>
    /// Localize text component.
    /// </summary> 
    public class LocalizedText : MonoBehaviour
    {
        //public bool GetKeyFtext=false;
        string LocalizationKey;

        public void Start()
        {
            //if (GetKeyFtext)
            // {
            if (GetComponent<Text>())
            {
                LocalizationKey = GetComponent<Text>().text;
            }

            if (GetComponent<TMP_Text>())
            {
                LocalizationKey = GetComponent<TMP_Text>().text;
            }

            // }
            if (LocalizationKey != "")
            {

                Localize();
                LocalizationManager.LocalizationChanged += Localize;
            }
        }

        public void OnDestroy()
        {
            if (LocalizationKey != "")
            {

                LocalizationManager.LocalizationChanged -= Localize;
            }
        }

        private void Localize()
        {
            if (GetComponent<Text>())
            {
                GetComponent<Text>().text = LocalizationManager.Localize(LocalizationKey);
            }

            if (GetComponent<TMP_Text>())
            {
                GetComponent<TMP_Text>().text = LocalizationManager.Localize(LocalizationKey);
            }
        }
    }
}