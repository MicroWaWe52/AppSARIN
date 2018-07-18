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
    class Ordine
    {
        public string Name { get; set; }
        public string CodCli { get; set; }
        public string Date { get; set; }
        public string Tot { get; set; }
        public string Type { get; set; }
    }
}