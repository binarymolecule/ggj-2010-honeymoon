#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace KeplersLibrary
{
    /// <summary>
    /// Helper class for customization of input controls. Each control is identified
    /// by its name. For each logical player, the control is mapped onto a specific
    /// key and button. With each logical player, one player index defining the keyboard
    /// input device and one player index defining the gamepad input device associated
    /// with this player is stored within the control layout.
    /// </summary>
    public class ControlLayout
    {
        #region Fields

        readonly Dictionary<string, Keys>[] keyMap;
        readonly Dictionary<string, Buttons>[] buttonMap;
        readonly PlayerIndex[] keyboardIndex, gamepadIndex;

        public PlayerIndex KeyboardIndex(PlayerIndex index) { return keyboardIndex[(int)index]; }
        public PlayerIndex GamePadIndex(PlayerIndex index) { return gamepadIndex[(int)index]; }

        #endregion

        #region Initialization

        public static ControlLayout CreateDefaultLayout()
        {
            PlayerIndex[] keyboardIndex = { PlayerIndex.One, PlayerIndex.One };
            PlayerIndex[] gamepadIndex = { PlayerIndex.One, PlayerIndex.Two };
            ControlLayout layout = new ControlLayout(2, keyboardIndex, gamepadIndex);
            layout.SetKey("select", PlayerIndex.One, Keys.Space);
            layout.SetKey("select", PlayerIndex.Two, Keys.Enter);
            layout.SetKey("cancel", PlayerIndex.One, Keys.Escape);
            layout.SetKey("cancel", PlayerIndex.Two, Keys.Back);
            return layout;
        }

        public ControlLayout(int NumPlayers, PlayerIndex[] keyboardIndex, PlayerIndex[] gamepadIndex)
        {
            this.keyboardIndex = keyboardIndex;
            this.gamepadIndex = gamepadIndex;
            this.keyMap = new Dictionary<string, Keys>[NumPlayers];
            this.buttonMap = new Dictionary<string, Buttons>[NumPlayers];
            for (int i = 0; i < NumPlayers; i++)
            {
                this.keyMap[i] = new Dictionary<string, Keys>();
                this.buttonMap[i] = new Dictionary<string, Buttons>();
            }
        }

        #endregion

        #region Public Methods

        public Keys GetKey(string name, PlayerIndex player)
        {
            return keyMap[(int)player][name];
        }

        public Buttons GetButton(string name, PlayerIndex player)
        {
            return buttonMap[(int)player][name];
        }

        public void SetKey(string name, PlayerIndex player, Keys key)
        {
            if (keyMap[(int)player].ContainsKey(name))
                keyMap[(int)player][name] = key;
            else
                keyMap[(int)player].Add(name, key);
        }

        public void SetButton(string name, PlayerIndex player, Buttons button)
        {
            if (buttonMap[(int)player].ContainsKey(name))
                buttonMap[(int)player][name] = button;
            else
                buttonMap[(int)player].Add(name, button);
        }

        #endregion

    }
}