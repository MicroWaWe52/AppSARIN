using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GestioneSarin2
{
    public static class ImageManager
    {
        static Dictionary<string,Drawable> cache = new Dictionary<string, Drawable>();

        public static Drawable Get(Context context, string url)
        {
            if (!cache.ContainsKey(url))
            {
                var drawable = Drawable.CreateFromStream(new FileStream(url,FileMode.Open), null);

                cache.Add(url, drawable);
            }

            return cache[url];
        }
    }
}