using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise.Graphics.Input.Keyboard
{
    class US_English_103 : IKeyboard
    {

        public static Type Type
        {
            get
            {
                return typeof(US_English_103);
            }
        }

        public string DisplayName
        {
            get
            {
                return "English 103 (US)";
            }
        }

        public string Name
        {
            get
            {
                return "English_103";
            }
        }

        public char GetCharacterCode(Key[] Down)
        {
            bool isShift = false;
            if (Down.Contains(Key.Shift) || Down.Contains(Key.LeftShift) || Down.Contains(Key.RightShift))
                isShift = true;

            char c = char.MinValue;

            foreach (Key key in Down)
                switch (key)
                {
                    case Key.Space:
                        c = ' ';
                        break;
                    case Key.N0:
                        c = '0';
                        if (isShift) c = ')';
                        break;
                    case Key.N1:
                        c = '1';
                        if (isShift) c = '!';
                        break;
                    case Key.N2:
                        c = '2';
                        if (isShift) c = '@';
                        break;
                    case Key.N3:
                        c = '3';
                        if (isShift) c = '#';
                        break;
                    case Key.N4:
                        c = '4';
                        if (isShift) c = '$';
                        break;
                    case Key.N5:
                        c = '5';
                        if (isShift) c = '%';
                        break;
                    case Key.N6:
                        c = '6';
                        if (isShift) c = '^';
                        break;
                    case Key.N7:
                        c = '7';
                        if (isShift) c = '&';
                        break;
                    case Key.N8:
                        c = '8';
                        if (isShift) c = '*';
                        break;
                    case Key.N9:
                        c = '9';
                        if (isShift) c = '(';
                        break;
                    case Key.A:
                        c = 'a';
                        break;
                    case Key.B:
                        c = 'b';
                        break;
                    case Key.C:
                        c = 'c';
                        break;
                    case Key.D:
                        c = 'd';
                        break;
                    case Key.E:
                        c = 'e';
                        break;
                    case Key.F:
                        c = 'f';
                        break;
                    case Key.G:
                        c = 'g';
                        break;
                    case Key.H:
                        c = 'h';
                        break;
                    case Key.I:
                        c = 'i';
                        break;
                    case Key.J:
                        c = 'j';
                        break;
                    case Key.K:
                        c = 'k';
                        break;
                    case Key.L:
                        c = 'l';
                        break;
                    case Key.M:
                        c = 'm';
                        break;
                    case Key.N:
                        c = 'n';
                        break;
                    case Key.O:
                        c = 'o';
                        break;
                    case Key.P:
                        c = 'p';
                        break;
                    case Key.Q:
                        c = 'q';
                        break;
                    case Key.R:
                        c = 'r';
                        break;
                    case Key.S:
                        c = 's';
                        break;
                    case Key.T:
                        c = 't';
                        break;
                    case Key.U:
                        c = 'u';
                        break;
                    case Key.V:
                        c = 'v';
                        break;
                    case Key.W:
                        c = 'w';
                        break;
                    case Key.X:
                        c = 'x';
                        break;
                    case Key.Y:
                        c = 'y';
                        break;
                    case Key.Z:
                        c = 'z';
                        break;
                        //Start Numpad
                    case Key.Numpad0:
                        c = '0';
                        break;
                    case Key.Numpad1:
                        c = '1';
                        break;
                    case Key.Numpad2:
                        c = '2';
                        break;
                    case Key.Numpad3:
                        c = '3';
                        break;
                    case Key.Numpad4:
                        c = '4';
                        break;
                    case Key.Numpad5:
                        c = '5';
                        break;
                    case Key.Numpad6:
                        c = '6';
                        break;
                    case Key.Numpad7:
                        c = '7';
                        break;
                    case Key.Numpad8:
                        c = '8';
                        break;
                    case Key.Numpad9:
                        c = '9';
                        break;
                    case Key.Multiply:
                        c = '*';
                        break;
                    case Key.Add:
                        c = '+';
                        break;
                    case Key.Separator:
                        c = '_';
                        break;
                    case Key.Subtract:
                        c = '-';
                        break;
                    case Key.Decimal:
                        c = '.';
                        break;
                    case Key.Divide:
                        c = '/';
                        break;
                        //End Numpad
                    case Key.NEC_Equal:
                        c = '=';
                        if (isShift) c = '+';
                        break;
                    case Key.Semicolon:
                        c = ';';
                        if (isShift) c = ':';
                        break;
                    case Key.OEMPlus:
                        c = '+';
                        break;
                    case Key.OEMComma:
                        c = ',';
                        if (isShift) c = '<';
                        break;
                    case Key.OEMMinus:
                        c = '-';
                        if (isShift) c = '_';
                        break;
                    case Key.OEMPeriod:
                        c = '.';
                        if (isShift) c = '>';
                        break;
                    case Key.OEM2:
                        c = '/';
                        if (isShift) c = '?';
                        break;
                    case Key.Tilde:
                        c = '`';
                        if (isShift) c = '~';
                        break;
                    case Key.OpenBracket:
                        c = '[';
                        if (isShift) c = '{';
                        break;
                    case Key.BackSlash:
                        c = '\\';
                        if (isShift) c = '|';
                        break;
                    case Key.CloseBracket:
                        c = ']';
                        if (isShift) c = '}';
                        break;
                    case Key.Apostrophe:
                        c = '\'';
                        if (isShift) c = '"';
                        break;
                    case Key.OEM8:
                        c = '\\';
                        if (isShift) c = '|';
                        break;
     
                }


            if (c == char.MinValue) return c;

            if (isShift)
                c = char.ToUpper(c);

            return c;
        }

    }
}
