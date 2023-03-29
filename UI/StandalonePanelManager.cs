﻿// <copyright file="StandalonePanelManager.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace AlgernonCommons.UI
{
    using System;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Management of standalone in-game panels.
    /// </summary>
    /// <typeparam name="TPanel">Panel type.</typeparam>
    public static class StandalonePanelManager<TPanel>
        where TPanel : StandalonePanelBase
    {
        // Instance references.
        private static GameObject s_gameObject;
        private static TPanel s_panel;

        // Last saved position.
        private static Vector2 s_savedPosition = Vector2.left;

        /// <summary>
        /// Gets the active panel instance.
        /// </summary>
        public static TPanel Panel => s_panel;

        /// <summary>
        /// Creates the panel object in-game and displays it.
        /// </summary>
        public static void Create()
        {
            try
            {
                // If no GameObject instance already set, create one.
                if (s_gameObject == null)
                {
                    // Give it a unique name for easy finding with ModTools.
                    s_gameObject = new GameObject(typeof(TPanel).Name);
                    UIView view = UIView.GetAView();
                    s_gameObject.transform.parent = view.transform;

                    // Create new panel instance and add it to GameObject.
                    s_panel = s_gameObject.AddComponent<TPanel>();

                    // Set panel relative position if saved.
                    if (s_savedPosition.x >= 0f && s_panel.RememberPosition)
                    {
                        s_panel.absolutePosition = s_savedPosition;
                    }

                    // Ensure panel is fully visible on screen (in case of e.g. UI scaling changes).
                    float clampedXpos = Mathf.Clamp(s_panel.absolutePosition.x, 0f, view.fixedWidth - s_panel.PanelWidth);
                    float clampedYpos = Mathf.Clamp(s_panel.absolutePosition.y, 0f, view.fixedHeight - s_panel.PanelHeight);
                    s_panel.absolutePosition = new Vector2(clampedXpos, clampedYpos);

                    // Create panel close event handler.
                    s_panel.EventClose += DestroyPanel;
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception creating standalone panel of type ", typeof(TPanel).Name);
            }
        }

        /// <summary>
        /// Destroys the panel and containing GameObject (removing any ongoing UI overhead).
        /// </summary>
        private static void DestroyPanel()
        {
            // Don't do anything if no panel.
            if (s_panel == null)
            {
                return;
            }

            // Record previous position if we're doing so.
            if (s_panel.RememberPosition)
            {
                s_savedPosition = s_panel.absolutePosition;
            }

            // Destroy game objects.
            GameObject.Destroy(s_panel);
            GameObject.Destroy(s_gameObject);

            // Let the garbage collector do its work (and also let us know that we've closed the object).
            s_panel = null;
            s_gameObject = null;
        }
    }
}