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
            selectedColor = new Color(206.0f/255.0f, 61.0f/255.0f, 54.0f/255.0f);
            defaultColor = new Color(97.0f/255.0f, 108.0f/255.0f, 108.0f/255.0f);
        }
        public void SelectFilter() {
            var filterList = this.transform.parent.GetComponentsInChildren<FilterSelect>();
            foreach (var filter in filterList)
            {
                if (filter.Equals(this))
                {
                    UpdateStatus(true);
                }
                else {
                    filter.UpdateStatus(false);
                }
            }
        }


        public void UpdateStatus(bool enable) {
            print("SelectedLineImage.enabled before? " + SelectedLineImage.enabled);
            isEnabled = enable;
            SelectedLineImage.GetComponent<Image>().enabled = isEnabled;
            FilterText.color = isEnabled ? selectedColor : defaultColor;
            print("SelectedLineImage.enabled after? " + SelectedLineImage.enabled);
        }
    }
}