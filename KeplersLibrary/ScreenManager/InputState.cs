#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace KeplersLibrary
{
    /// <summary>
    /// Helper for reading input from keyboard and gamepad. This class tracks both
    /// the current and previous state of both input devices, and implements query
    /// methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        #region Fields

        public const int MaxInputs = 4;

        public KeyboardState CurrentKeyboardState;
        public KeyboardState LastKeyboardState;

        public readonly GamePadState[] CurrentGamePadStates;
        public readonly GamePadState[] LastGamePadStates;

        public readonly bool[] GamePadWasConnected;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            CurrentGamePadStates = new GamePadState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];

            GamePadWasConnected = new bool[MaxInputs];
        }


        #endregion

        #region Public Methods


        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            for (int i = 0; i < MaxInputs; i++)
            {                
                LastGamePadStates[i] = CurrentGamePadStates[i];                
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (CurrentGamePadStates[i].IsConnected)
                {
                    GamePadWasConnected[i] = true;
                }
            }
        }


        /// <summary>
        /// Helper for checking if a key was newly pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer,
                                            out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardState.IsKeyDown(key) &&
                        LastKeyboardState.IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Convenience wrapper for IsNewKeyPress when the controlling player
        /// does not matter. Tests only if any player is pressing the given key.
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
            PlayerIndex dummy;
            return IsNewKeyPress(key, null, out dummy);
        }

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer,
                                                     out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonDown(button) &&
                        LastGamePadStates[i].IsButtonUp(button));
            }
            else
            {
                // Accept input from any player.
                return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Convenience wrapper for IsNewButtonPress when the controlling player
        /// does not matter. Tests only if any player is pressing the given button.
        /// </summary>
        public bool IsNewButtonPress(Buttons button)
        {
            PlayerIndex dummy;
            return IsNewButtonPress(button, null, out dummy);
        }

        /// <summary>
        /// Checks for a "menu select" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuSelect(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu cancel" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuCancel(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu up" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu down" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "pause the game" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsPauseGame(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }


        #endregion
    }
}
