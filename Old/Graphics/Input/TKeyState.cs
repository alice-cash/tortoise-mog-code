/*
 * Copyright 2016 Matthew Cash. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *    1. Redistributions of source code must retain the above copyright notice, this list of
 *       conditions and the following disclaimer.
 * 
 *    2. Redistributions in binary form must reproduce the above copyright notice, this list
 *       of conditions and the following disclaimer in the documentation and/or other materials
 *       provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY Matthew Cash ``AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Matthew Cash OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Matthew Cash.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using Microsoft.Xna.Framework.Input;
using Control = System.Windows.Forms.Control;
using Keys = System.Windows.Forms.Keys;

namespace Tortoise.Graphics.Input
{
    public class TKeyState: InputState
    {
        private Key[] _keyStateArray;
        public event Action<KeyEventArgs> KeyboardEvent;
        public event Action<KeyEventArgs> KeyboardKeyPressEvent;
        public event Action<KeyEventArgs> KeyboardKeyReleaseEvent;
        private TGraphics _graphics;

        private List<KeyStateEventData> _keyEventData;

        private struct KeyStateEventData
        {
            public Key Data;
            public KeyStateEventType Type;

            public KeyStateEventData(System.Windows.Forms.KeyEventArgs e, KeyStateEventType t)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                Type = t;
                Data = TranslateKey(e.KeyCode);
            }
        }

        private enum KeyStateEventType
        {
            KeyDown,
            KeyUp,
        }


        public TKeyState(TGraphics graphics)
        {
            _graphics = graphics;
            _keyEventData = new List<KeyStateEventData>();
            _keyStateArray = new Key[0];

            graphics.Control.KeyDown += Control_KeyDown;
            graphics.Control.KeyUp += Control_KeyUp;
        }

        private void Control_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            lock (_keyEventData)
            {
                _keyEventData.Add(new KeyStateEventData(e, KeyStateEventType.KeyUp));
            }
        }

        private void Control_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            lock (_keyEventData)
            {
                _keyEventData.Add(new KeyStateEventData(e, KeyStateEventType.KeyDown));
            }
        }

        public override bool Poll()
        {
            bool returnStatus = false;

            KeyStateEventData[] eventDataArray;

            lock (_keyEventData)
            {
                eventDataArray = _keyEventData.ToArray();
                _keyEventData.Clear();
            }

            if (eventDataArray.Length == 0) { return returnStatus; }

            Key[] currentstate = GetKeyState(_keyStateArray, GetPressedKeys(eventDataArray), GetRelasedKeys(eventDataArray));
            Key[] pressedKeys = GetNewItems(_keyStateArray, currentstate);
            Key[] releasedKeys = GetMissingItems(_keyStateArray, currentstate);


            _keyStateArray = currentstate;

            KeyEventData eventData = new KeyEventData(currentstate, pressedKeys, releasedKeys);

            if (pressedKeys.Length != 0 || releasedKeys.Length !=0 || currentstate.Length !=0)
            {
                KeyboardEvent?.Invoke(new KeyEventArgs(eventData));
                returnStatus = true;
            }

            if (pressedKeys.Length != 0)
            {
                KeyboardKeyPressEvent?.Invoke(new KeyEventArgs(eventData));
            }

            if (releasedKeys.Length != 0)
            {
                KeyboardKeyReleaseEvent?.Invoke(new KeyEventArgs(eventData));
            }

            return returnStatus;
        }

        private Key[] GetKeyState(Key[] state, Key[] pressed, Key[] released)
        {
            List<Key> returndata = new List<Key>(pressed);
            foreach(Key k in state)
            {
                if (pressed.Contains(k)) continue;
                if (released.Contains(k)) continue;
                if (returndata.Contains(k)) continue;
                returndata.Add(k);
            }
            return returndata.ToArray();
        }

        private Key[] GetPressedKeys(KeyStateEventData[] data)
        {
            List<Key> ButtonList = new List<Key>();
            foreach (KeyStateEventData state in data)
            {
                if (state.Type == KeyStateEventType.KeyDown)
                {
                    ButtonList.Add(state.Data);
                }
            }
            return ButtonList.ToArray();
        }

        private Key[] GetRelasedKeys(KeyStateEventData[] data)
        {
            List<Key> ButtonList = new List<Key>();
            foreach (KeyStateEventData state in data)
            {
                if (state.Type == KeyStateEventType.KeyUp)
                {
                    ButtonList.Add(state.Data);
                }
            }
            return ButtonList.ToArray();
        }


        private static Key TranslateKey(Keys key)
        {

            switch (key)
            {
                case Keys.None: return Key.None;
                case Keys.LButton: return Key.LeftButton;
                case Keys.RButton: return Key.RightButton;
                case Keys.Cancel: return Key.Cancel;
                case Keys.MButton: return Key.MiddleButton;
                case Keys.XButton1: return Key.ExtraButton1;
                case Keys.XButton2: return Key.ExtraButton2;
                case Keys.Back: return Key.Back;
                case Keys.Tab: return Key.Tab;
                case Keys.Clear: return Key.Clear;
                case Keys.Enter: return Key.Return;
                case Keys.ShiftKey: return Key.Shift;
                case Keys.ControlKey: return Key.Control;
                case Keys.Menu: return Key.Menu;
                case Keys.Pause: return Key.Pause;
                case Keys.CapsLock: return Key.Capital;
                case Keys.KanaMode: return Key.Kana;
                //case Keys.Hangeul: return Key.Hangeul;
                //case Keys.HangulMode: return Key.Hangul;
                case Keys.JunjaMode: return Key.Junja;
                case Keys.FinalMode: return Key.Final;
                //case Keys.HanjaMode: return Key.Hanja;
                case Keys.KanjiMode: return Key.Kanji;
                case Keys.Escape: return Key.Escape;
                case Keys.IMEConvert: return Key.Convert;
                case Keys.IMENonconvert: return Key.NonConvert;
                case Keys.IMEAccept: return Key.Accept;
                case Keys.IMEModeChange: return Key.ModeChange;
                case Keys.Space: return Key.Space;
                case Keys.PageUp: return Key.Prior;
                case Keys.PageDown: return Key.Next;
                case Keys.End: return Key.End;
                case Keys.Home: return Key.Home;
                case Keys.Left: return Key.Left;
                case Keys.Up: return Key.Up;
                case Keys.Right: return Key.Right;
                case Keys.Down: return Key.Down;
                case Keys.Select: return Key.Select;
                case Keys.Print: return Key.Print;
                case Keys.Execute: return Key.Execute;
                case Keys.PrintScreen: return Key.Snapshot;
                case Keys.Insert: return Key.Insert;
                case Keys.Delete: return Key.Delete;
                case Keys.Help: return Key.Help;
                case Keys.D0: return Key.N0;
                case Keys.D1: return Key.N1;
                case Keys.D2: return Key.N2;
                case Keys.D3: return Key.N3;
                case Keys.D4: return Key.N4;
                case Keys.D5: return Key.N5;
                case Keys.D6: return Key.N6;
                case Keys.D7: return Key.N7;
                case Keys.D8: return Key.N8;
                case Keys.D9: return Key.N9;
                case Keys.A: return Key.A;
                case Keys.B: return Key.B;
                case Keys.C: return Key.C;
                case Keys.D: return Key.D;
                case Keys.E: return Key.E;
                case Keys.F: return Key.F;
                case Keys.G: return Key.G;
                case Keys.H: return Key.H;
                case Keys.I: return Key.I;
                case Keys.J: return Key.J;
                case Keys.K: return Key.K;
                case Keys.L: return Key.L;
                case Keys.M: return Key.M;
                case Keys.N: return Key.N;
                case Keys.O: return Key.O;
                case Keys.P: return Key.P;
                case Keys.Q: return Key.Q;
                case Keys.R: return Key.R;
                case Keys.S: return Key.S;
                case Keys.T: return Key.T;
                case Keys.U: return Key.U;
                case Keys.V: return Key.V;
                case Keys.W: return Key.W;
                case Keys.X: return Key.X;
                case Keys.Y: return Key.Y;
                case Keys.Z: return Key.Z;
                case Keys.LWin: return Key.LeftWindows;
                case Keys.RWin: return Key.RightWindows;
                case Keys.Apps: return Key.Application;
                case Keys.Sleep: return Key.Sleep;
                case Keys.NumPad0: return Key.Numpad0;
                case Keys.NumPad1: return Key.Numpad1;
                case Keys.NumPad2: return Key.Numpad2;
                case Keys.NumPad3: return Key.Numpad3;
                case Keys.NumPad4: return Key.Numpad4;
                case Keys.NumPad5: return Key.Numpad5;
                case Keys.NumPad6: return Key.Numpad6;
                case Keys.NumPad7: return Key.Numpad7;
                case Keys.NumPad8: return Key.Numpad8;
                case Keys.NumPad9: return Key.Numpad9;
                case Keys.Multiply: return Key.Multiply;
                case Keys.Add: return Key.Add;
                case Keys.Separator: return Key.Separator;
                case Keys.Subtract: return Key.Subtract;
                case Keys.Decimal: return Key.Decimal;
                case Keys.Divide: return Key.Divide;
                case Keys.F1: return Key.F1;
                case Keys.F2: return Key.F2;
                case Keys.F3: return Key.F3;
                case Keys.F4: return Key.F4;
                case Keys.F5: return Key.F5;
                case Keys.F6: return Key.F6;
                case Keys.F7: return Key.F7;
                case Keys.F8: return Key.F8;
                case Keys.F9: return Key.F9;
                case Keys.F10: return Key.F10;
                case Keys.F11: return Key.F11;
                case Keys.F12: return Key.F12;
                case Keys.F13: return Key.F13;
                case Keys.F14: return Key.F14;
                case Keys.F15: return Key.F15;
                case Keys.F16: return Key.F16;
                case Keys.F17: return Key.F17;
                case Keys.F18: return Key.F18;
                case Keys.F19: return Key.F19;
                case Keys.F20: return Key.F20;
                case Keys.F21: return Key.F21;
                case Keys.F22: return Key.F22;
                case Keys.F23: return Key.F23;
                case Keys.F24: return Key.F24;
                case Keys.NumLock: return Key.NumLock;
                case Keys.Scroll: return Key.ScrollLock;
                //case Keys.NEC_Equal: return Key.NEC_Equal;
                //case Keys.Fujitsu_Jisho: return Key.Fujitsu_Jisho;
                //case Keys.Fujitsu_Masshou: return Key.Fujitsu_Masshou;
                //case Keys.Fujitsu_Touroku: return Key.Fujitsu_Touroku;
                //case Keys.Fujitsu_Loya: return Key.Fujitsu_Loya;
                //case Keys.Fujitsu_Roya: return Key.Fujitsu_Roya;
                case Keys.LShiftKey: return Key.LeftShift;
                case Keys.RShiftKey: return Key.RightShift;
                case Keys.LControlKey: return Key.LeftControl;
                case Keys.RControlKey: return Key.RightControl;
                case Keys.LMenu: return Key.LeftMenu;
                case Keys.RMenu: return Key.RightMenu;
                case Keys.BrowserBack: return Key.BrowserBack;
                case Keys.BrowserForward: return Key.BrowserForward;
                case Keys.BrowserRefresh: return Key.BrowserRefresh;
                case Keys.BrowserStop: return Key.BrowserStop;
                case Keys.BrowserSearch: return Key.BrowserSearch;
                case Keys.BrowserFavorites: return Key.BrowserFavorites;
                case Keys.BrowserHome: return Key.BrowserHome;
                case Keys.VolumeMute: return Key.VolumeMute;
                case Keys.VolumeDown: return Key.VolumeDown;
                case Keys.VolumeUp: return Key.VolumeUp;
                case Keys.MediaNextTrack: return Key.MediaNextTrack;
                case Keys.MediaPreviousTrack: return Key.MediaPrevTrack;
                case Keys.MediaStop: return Key.MediaStop;
                case Keys.MediaPlayPause: return Key.MediaPlayPause;
                case Keys.LaunchMail: return Key.LaunchMail;
                case Keys.SelectMedia: return Key.LaunchMediaSelect;
                case Keys.LaunchApplication1: return Key.LaunchApplication1;
                case Keys.LaunchApplication2: return Key.LaunchApplication2;
                //case Keys.OEM1: return Key.OEM1;
                case Keys.OemSemicolon: return Key.Semicolon;
                case Keys.Oemplus: return Key.OEMPlus;
                case Keys.Oemcomma: return Key.OEMComma;
                case Keys.OemMinus: return Key.OEMMinus;
                case Keys.OemPeriod: return Key.OEMPeriod;
                //case Keys.OEM2: return Key.OEM2;
                case Keys.OemQuestion: return Key.Question;
                //case Keys.OEM3: return Key.OEM3;
                case Keys.Oemtilde: return Key.Tilde;
                //case Keys.OEM4: return Key.OEM4;
                case Keys.OemOpenBrackets: return Key.OpenBracket;
                //case Keys.OEM5: return Key.OEM5;
                case Keys.OemBackslash: return Key.BackSlash;
                //case Keys.OEM6: return Key.OEM6;
                case Keys.OemCloseBrackets: return Key.CloseBracket;
                //case Keys.OEM7: return Key.OEM7;
                case Keys.OemQuotes: return Key.Apostrophe;
                case Keys.Oem8: return Key.OEM8;
                //case Keys.OEMAX: return Key.OEMAX;
                //case Keys.OEM102: return Key.OEM102;
                //case Keys.ICOHelp: return Key.ICOHelp;
                //case Keys.ICO00: return Key.ICO00;
                case Keys.ProcessKey: return Key.ProcessKey;
                //case Keys.ICOClear: return Key.ICOClear;
                case Keys.Packet: return Key.Packet;
                //case Keys.OEMReset: return Key.OEMReset;
                //case Keys.OEMJump: return Key.OEMJump;
                //case Keys.OEMPA1: return Key.OEMPA1;
                //case Keys.OEMPA2: return Key.OEMPA2;
                //case Keys.OEMPA3: return Key.OEMPA3;
                //case Keys.OEMWSCtrl: return Key.OEMWSCtrl;
                //case Keys.OEMCUSel: return Key.OEMCUSel;
                //case Keys.OEMATTN: return Key.OEMATTN;
                //case Keys.OEMFinish: return Key.OEMFinish;
                //case Keys.OemCopy: return Key.OEMCopy;
                //case Keys.OemAuto: return Key.OEMAuto;
                //case Keys.OemEnlW: return Key.OEMENLW;
                //case Keys.OEMBackTab: return Key.OEMBackTab;
                case Keys.Attn: return Key.ATTN;
                case Keys.Crsel: return Key.CRSel;
                case Keys.Exsel: return Key.EXSel;
                case Keys.EraseEof: return Key.EREOF;
                case Keys.Play: return Key.Play;
                case Keys.Zoom: return Key.Zoom;
                case Keys.NoName: return Key.Noname;
                case Keys.Pa1: return Key.PA1;
                case Keys.OemClear: return Key.OEMClear;
                    
                //No idea what to return for these
                //case Keys.OemPipe:
                //case Keys.LineFeed:
                case Keys.OemPipe: return Key.BackSlash;

                default:
                    System.Diagnostics.Trace.WriteLine(key.ToString());
                    System.Diagnostics.Trace.WriteLine((int)key);
                    return Key.Noname;


            }

        }
    }
}
