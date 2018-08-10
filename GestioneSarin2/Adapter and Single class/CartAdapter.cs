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
using Object = Java.Lang.Object;

namespace GestioneSarin2.Adapter_and_Single_class
{
    class CartAdapter : BaseExpandableListAdapter
    {
        public CartAdapter(Context context, List<string> listGroup, Dictionary<string, List<string>> listChild)
        {
            Context = context;
            ListGroup = listGroup;
            ListChild = listChild;
        }

        public override Object GetChild(int groupPosition, int childPosition)
        {
            var result = new List<string>();
            ListChild.TryGetValue(ListGroup[groupPosition], out result);
            return result[childPosition];


        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            var result = new List<string>();
            ListChild.TryGetValue(ListGroup[groupPosition], out result);
            return result.Count;

        }

        public override int GetChildrenCount(int groupPosition)
        {
            var result = new List<string>();
            ListChild.TryGetValue(ListGroup[groupPosition], out result);
            return result.Count;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                var inf = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                convertView = inf.Inflate(Resource.Layout.itemLayout, null);
            }

            var itemView = convertView.FindViewById<TextView>(Resource.Id.textItem);
            var buttItem = convertView.FindViewById<Button>(Resource.Id.buttonItem);
            buttItem.Text = "Bella ciao pisello-kun";
            buttItem.Click += ButtItem_Click;
            var content = (string)GetChild(groupPosition, childPosition);
            itemView.Text = content;
            return convertView;
        }

        private void ButtItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public override Object GetGroup(int groupPosition)
        {
            return ListGroup[groupPosition];
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                var inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.groupLayout, null);
                
            }
            var textGroup = (string)GetGroup(groupPosition);
            var textGroupV = convertView.FindViewById<TextView>(Resource.Id.textGroup);
            textGroupV.Text = textGroup;
            return convertView;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }

        public override int GroupCount => ListGroup.Count;
        public override bool HasStableIds => false;
        public Context Context { get; set; }

        public List<string> ListGroup { get; set; }

        public Dictionary<string, List<string>> ListChild { get; set; }
    }
}