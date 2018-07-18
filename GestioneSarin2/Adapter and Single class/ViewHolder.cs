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
    class ViewHolderProdotto : Java.Lang.Object
    {
        public ImageView Photo { get; set; }
        public TextView Name { get; set; }
        public TextView QuantPrice { get; set; }
    }
    class ViewHolderOrdine : Java.Lang.Object
    {
        public TextView Name { get; set; }
        public TextView Data { get; set; }
        public TextView Price { get; set; }
        public TextView CodCli { get; set; }
        public TextView Type { get; set; }
    }
}