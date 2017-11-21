//using System;
//using System.Collections.Generic;
//using Android.App;
//using Android.Widget;

//namespace FoosLiveAndroid
//{
//    public class MenuAdapter : BaseAdapter<MenuItem>
//    {
//        List<Button> items;
//        Activity context;
//        public MenuAdapter(Activity context, List<Button> items)
//            : base()
//        {
//            this.context = context;
//            this.items = items;
//        }
//        public override long GetItemId(int position)
//        {
//            return position;
//        }
//        public override TableItem this[int position]
//        {
//            get { return items[position]; }
//        }
//        public override int Count
//        {
//            get { return items.Count; }
//        }
//        public override View GetView(int position, View convertView, ViewGroup parent)
//        {
//            var item = items[position];
//            View view = convertView;
//            if (view == null) // no view to re-use, create new
//                view = context.LayoutInflater.Inflate(Resource.Layout.CustomView, null);
//            view.FindViewById<TextView>(Resource.Id.Text1).Text = item.Heading;
//            view.FindViewById<TextView>(Resource.Id.Text2).Text = item.SubHeading;
//            view.FindViewById<ImageView>(Resource.Id.Image).SetImageResource(item.ImageResourceId);
//            return view;
//        }
//    }
//}
