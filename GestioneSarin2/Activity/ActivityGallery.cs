using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.IO;

namespace GestioneSarin2.Activity
{
    [Activity(Label = "ActivityGallery",Theme = "@style/AppTheme")]
    public class ActivityGallery : AppCompatActivity
    {
        private Gallery gallery;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutGallery);
            // Create your application here
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment
                           .DirectoryDownloads).AbsolutePath + "/Sarin/imacat";
            gallery = FindViewById<Gallery>(Resource.Id.galleryAdd);
        }
    }
}