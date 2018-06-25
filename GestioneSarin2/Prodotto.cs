using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GestioneSarin2
{
    class Prodotto:Java.Lang.Object
    {
        public string Name { get; set; }

        public string QuantityPrice { get; set; }
        public string ImageUrl { get; set; }
        public string Grouop { get; set; }
        public string SubGroup { get; set; }
    }
}