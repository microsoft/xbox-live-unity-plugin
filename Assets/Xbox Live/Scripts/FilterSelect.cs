using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.Xbox.Services.Client
{
    public class FilterSelect : MonoBehaviour
    {
        [Header("Filter Setup")]
        public bool isEnabled = false;
        public int FilterId;

        [Header("UI Components")]
        public Image SelectedLineImage;
        public Text FilterText;

        [Header("Color Choices")]
        private Color selectedColor;
        private Color defaultColor;

        private void Awake()
        {
            selectedColor = new Color(206.0f/255.0f, 61.0f/255.0f, 54.0f/255.0f);
            defaultColor = new Color(97.0f/255.0f, 108.0f/255.0f, 108.0f/255.0f);
        }

        public void UpdateStatus(bool enable) {
            isEnabled = enable;
            SelectedLineImage.GetComponent<Image>().enabled = isEnabled;
            FilterText.color = isEnabled ? selectedColor : defaultColor;
        }
    }
}