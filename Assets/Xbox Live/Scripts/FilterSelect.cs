// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        public Theme Theme;

        [Header("UI Components")]
        public Image SelectedLineImage;
        public Text FilterText;
        
        public void UpdateStatus(bool enable) {
            isEnabled = enable;
            SelectedLineImage.GetComponent<Image>().enabled = isEnabled;
            FilterText.color = isEnabled ? 
                ThemeHelper.GetThemeHighlightColor(this.Theme) : 
                ThemeHelper.GetThemeBaseFontColor(this.Theme);
            this.SelectedLineImage.sprite = ThemeHelper.LoadSprite(this.Theme, "RowBackground-Highlighted");
        }
    }
}