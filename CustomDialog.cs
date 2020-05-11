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

namespace Com.Android.Example.Beam {

    public class CustomDialog : Dialog {
        string data;
        string id;
        string tagTech;
        public CustomDialog(Activity activity, string id, string data, string tagTech) : base(activity) {
            this.id = id;
            this.data = data;
            this.tagTech = tagTech;
        }

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature((int)WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.CustomDialog);


            FindViewById<TextView>(Resource.Id.textview_id).Text = this.id;
            FindViewById<TextView>(Resource.Id.textview_data).Text = this.data;
        }
    }
}