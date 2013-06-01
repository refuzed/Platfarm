using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

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

        static public GraphicsDevice GetGraphicsDevice(this ContentManager content)
        {
            IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)content.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
            return graphicsDeviceService.GraphicsDevice;
        }

        public static Point ToPoint(this Vector2 v)
        {
            return new Point(Convert.ToInt32(v.X), Convert.ToInt32(v.Y));
        }
    }
}
