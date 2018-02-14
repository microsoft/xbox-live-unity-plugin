// Copyright (c) Microsoft Corporation
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// 
using System;

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.Xbox.Services.Client
{
    /// <summary>
    /// Logic for the StatsPanel prefab that displays name and value of a <see cref="StatBase"/>.
    /// </summary>
    [Serializable]
    public class StatPanel : MonoBehaviour
    {
        [HideInInspector]
        public Text StatLabelText;

        [HideInInspector]
        public Text StatValueText;

        [Tooltip("The stat object that this panel renders.")]
        public StatBase Stat;

        public bool ShowStatLabel;

        private void Awake()
        {
            this.EnsureEventSystem();
            XboxLiveServicesSettings.EnsureXboxLiveServicesSettings();
            this.StatLabelText.text = string.Empty;
            this.StatValueText.text = string.Empty;
        }

        /// <summary>
        /// OnGUI is called for rendering and handling GUI events
        /// </summary>
        private void OnGUI()
        {
            if (this.ShowStatLabel)
            {
                Vector3 position = this.StatValueText.rectTransform.position;
                position.x = this.StatLabelText.rectTransform.position.x + this.StatLabelText.rectTransform.rect.xMax + 10;
                this.StatValueText.rectTransform.position = position;
                this.StatLabelText.text = string.IsNullOrEmpty(this.Stat.DisplayName) ? this.Stat.ID : this.Stat.DisplayName;
                this.StatLabelText.gameObject.SetActive(true);
            }
            else
            {
                this.StatLabelText.gameObject.SetActive(false);
                Vector3 position = this.StatValueText.rectTransform.position;
                position.x = this.StatLabelText.rectTransform.position.x;
                this.StatValueText.rectTransform.position = position;
            }

            if (this.Stat != null)
            {
                this.StatValueText.text = this.Stat.ToString();
            }
        }
    }
}