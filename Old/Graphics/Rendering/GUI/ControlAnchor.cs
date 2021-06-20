using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise.Graphics.Rendering.GUI
{
    [Flags]
    public enum ControlAnchor
    {
        Default = Top | Left,
        Top = 0x01,
        Right = 0x02,
        Bottom = 0x04,
        Left = 0x08,
    }
}
