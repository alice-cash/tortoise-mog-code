using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XColor = Microsoft.Xna.Framework.Color;
using Color = System.Drawing.Color;

namespace Tortoise.Shared.Drawing
{
    public static class ColorTools
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static XColor ToXNAColor(Color color)
        {
            return new XColor(color.R, color.G, color.B, color.A);
        }
    }
}
