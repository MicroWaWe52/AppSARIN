﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Object = Java.Lang.Object;

namespace GestioneSarin2.Adapter_and_Single_class
{
    class ImageAdapter:BaseAdapter
    {
        private Context context;

        public ImageAdapter(Context context)
        {
            this.context = context;
        }

        public override Object GetItem(int position)
        {
            throw new NotImplementedException();
        }

        public override long GetItemId(int position)
        {
            throw new NotImplementedException();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView i=new ImageView(context);
            i.setimage
        }

        public override int Count => 0;
    }
}