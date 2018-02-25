using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.Xbox.Services.Client
{
    public class FilterSelect : MonoBehaviour
    {
        public Image SelectedLineImage;
        public Text FilterText;
        public bool isEnabled = false;

        private Color selectedColor;
        private Color defaultColor;

        private void Awake()
        {
            selectedColor = new Color(206, 61, 54);
            defaultColor = new Color(97, 108, 108);
        }
        public void SelectFilter() {
            UpdateStatus(true);
        }

        public void UpdateStatus(bool enable) {
            isEnabled = enable;
            SelectedLineImage.enabled = isEnabled;
            FilterText.color = isEnabled ? selectedColor : defaultColor;
        }
    }
}