// -----------------------------------------------------------------------
//  <copyright file="UIMonoBehavior.cs" company="Microsoft">
//      Copyright (c) Microsoft. All rights reserved.
//      Licensed under the MIT license. See LICENSE file in the project root for full license information.
//  </copyright>
// -----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A base class to provide some general helper
/// </summary>
public abstract class UIMonoBehaviour : MonoBehaviour
{
    /// <summary>
    /// Ensures that this GameObject is parented under a Canvas so that it will be rendered properly.
    /// </summary>
    /// <remarks>
    /// All UI elements must be children of a GameObject that has a Canvas component attached.  This
    /// method will cause an error to be written to the console but should not have any other detrimental
    /// effect. See https://docs.unity3d.com/Manual/comp-CanvasComponents.html for more detail.
    /// </remarks>
    protected void EnsureInCanvas()
    {
        if (this.GetComponentInParent<Canvas>() == null)
        {
            Debug.LogErrorFormat("UI Object '{0}' ({1}) is not a child of a Canvas so it will not render.  Create a Canvas element and place the element inside it.", this.name, this.GetType().Name);
        }
    }

    /// <summary>
    /// Ensures that this scene contains an EventSystem which is required for interactive UI elements to function..
    /// </summary>
    /// <remarks>
    /// All interactive UI elements need an EventSystem to be able to react to user input.  This method will cause
    /// an error to be written to the console but should not have any other detrimental.
    /// See https://docs.unity3d.com/Manual/EventSystem.html for more detail.
    /// </remarks>
    protected void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogErrorFormat("Interactive UI element '{0}' ({1}) requires an EventSystem component to function.  Create an EventSystem using 'GameObject > UI > EventSystem'.  A temporary event system will be created in the mean time.", this.name, this.GetType().Name);

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }
}