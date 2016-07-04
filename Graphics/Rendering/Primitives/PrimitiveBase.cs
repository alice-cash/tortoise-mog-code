using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise.Graphics.Rendering.Primitives
{
    abstract class PrimitiveBase
    {

        protected static List<Surface> pixleSet;

        protected static Surface getPixel(TGraphics tgraphics)
        {
            if (pixleSet == null) pixleSet = new List<Surface>();
            foreach (Surface pb in pixleSet)
            {
                if (pb.Graphics == tgraphics) return pb;
            }
            Surface newPixle = _createPixle(tgraphics);
            pixleSet.Add(newPixle);
            return newPixle;
        }

        private static Surface _createPixle(TGraphics tgraphics)
        {
            return Surface.CreateBlankSurface(tgraphics, 1, 1);
        }


    }
}
