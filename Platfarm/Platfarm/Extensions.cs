using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Platfarm
{
    public static class Extensions
    {
        public static string ToHex(this Color color)
        {
            string[] rgb = {
                color.R.ToString("X2"),
                color.G.ToString("X2"),
                color.B.ToString("X2"),
            };
            return "#" + string.Join(string.Empty, rgb);
        }
    }
}
