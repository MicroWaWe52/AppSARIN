using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.IO;


namespace GestioneSarin2.Adapter_and_Single_class
{
    public class MyAdapter : RecyclerView.Adapter
    {

        private List<CreateList> galleryList;
        private Context context;

        public MyAdapter(Context context, List<CreateList> galleryList)
        {
            this.galleryList = galleryList;
            this.context = context;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup viewGroup, int i)
        {
            View view = LayoutInflater.From(viewGroup.Context).Inflate(Resource.Layout.RowModelCata, viewGroup, false);
            return new ViewHolder(view);
        }
       

        public class ViewHolder : RecyclerView.ViewHolder
        {
            public ImageView Img { get; }

            public ViewHolder(View view) : base(view)
            {
                Img = view.FindViewById<ImageView>(Resource.Id.imgCata);
            }

        }

       
        public  override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

            ViewHolder vh = holder as ViewHolder;
            vh.Img.SetScaleType(ImageView.ScaleType.FitStart);
            using (Stream stream = new FileStream(galleryList[position].ImageTitle, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                vh.Img.SetImageBitmap(BitmapFactory.DecodeStream(stream));
            }
        }

        public override int ItemCount => galleryList.Count;
    }
}

