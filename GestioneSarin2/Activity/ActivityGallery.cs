using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GestioneSarin2;
using GestioneSarin2.Adapter_and_Single_class;
using Java.IO;
using Void = Java.Lang.Void;

namespace GestioneSarin2.Activity
{
    [Activity(Label = "ActivityGallery", Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
    public class ActivityGallery :Android.App.Activity
    {
        private RecyclerView recyclerView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutGalleryG);
            var galleryList = Helper.GetImgList();
            Gallery layout = FindViewById<Gallery>(Resource.Id.galleryCata);
            layout.Adapter=new ImageAdapter(this,galleryList);
           

        }

    }
}

public class CreateList
{
    public string ImageTitle { get; set; }

    public int ImageId { get; set; }

    public static List<CreateList> prepareData()
    {
        var imageList = Helper.GetImgList();

        List<CreateList> theimage = new List<CreateList>();

        foreach (var t in imageList)
        {
            CreateList createList = new CreateList();
            createList.ImageTitle = t;
            theimage.Add(createList);
        }

        return theimage;
    }
}

