using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise.Graphics.Input.Keyboard
{
    public interface IKeyboard
    {

        /// <summary>
        /// Name of the Keyboard, Typically matches the class name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Display friendly Keyboard name
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Generate a character based on the selected  keyboard input.
        /// </summary>
        /// <param name="Down">List of keys pressed</param>
        /// <returns></returns>
        char GetCharacterCode(Key[] Down);
    }
}
