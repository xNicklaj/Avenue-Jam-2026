using UnityEngine;

namespace TabbyStudios
{
    public static class KeyUtil
    {
        public static bool IsTypingKey(KeyCode key)
        {
            if (key >= KeyCode.A && key <= KeyCode.Z)
                return true;
    
            if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
                return true;
    
            if (key >= KeyCode.Keypad0 && key <= KeyCode.Keypad9)
                return true;
    
            switch (key)
            {
                case KeyCode.Space:
                case KeyCode.Exclaim:
                case KeyCode.DoubleQuote:
                case KeyCode.Hash:
                case KeyCode.Dollar:
                case KeyCode.Percent:
                case KeyCode.Ampersand:
                case KeyCode.Quote:
                case KeyCode.LeftParen:
                case KeyCode.RightParen:
                case KeyCode.Asterisk:
                case KeyCode.Plus:
                case KeyCode.Comma:
                case KeyCode.Minus:
                case KeyCode.Period:
                case KeyCode.Slash:
                case KeyCode.Colon:
                case KeyCode.Semicolon:
                case KeyCode.Less:
                case KeyCode.Equals:
                case KeyCode.Greater:
                case KeyCode.Question:
                case KeyCode.At:
                case KeyCode.LeftBracket:
                case KeyCode.Backslash:
                case KeyCode.RightBracket:
                case KeyCode.Caret:
                case KeyCode.Underscore:
                case KeyCode.BackQuote:
                case KeyCode.KeypadPeriod:
                case KeyCode.KeypadDivide:
                case KeyCode.KeypadMultiply:
                case KeyCode.KeypadMinus:
                case KeyCode.KeypadPlus:
                case KeyCode.KeypadEquals:
                    return true;
            }
        
            return false;
        }
    }
}