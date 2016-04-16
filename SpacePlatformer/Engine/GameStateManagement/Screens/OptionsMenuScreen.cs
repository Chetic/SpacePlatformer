#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace GameStateManagement.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry multisamplingMenuEntry;

        public static bool enableMultisampling = false;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            multisamplingMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            multisamplingMenuEntry.Selected += MultisamplingMenuEntrySelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(multisamplingMenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            multisamplingMenuEntry.Text = "Multisampling: " + (enableMultisampling ? "Enabled" : "Disabled");
        }


        #endregion

        #region Handle Input
        
        /// <summary>
        /// Event handler for when the multisampling setting menu entry is selected.
        /// </summary>
        void MultisamplingMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            enableMultisampling = !enableMultisampling;

            SpacePlatformer.SpacePlatformer.graphics.PreferMultiSampling = enableMultisampling;
            SpacePlatformer.SpacePlatformer.graphics.ApplyChanges();

            SetMenuEntryText();
        }

        #endregion
    }
}
