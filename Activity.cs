using System;
using System.Text;
using Android.App;
using Android.Nfc;
using Android.Content;
using Android.Provider;
using Android.Runtime;
using Android.OS;
using Android.Text.Format;
using Android.Views;
using Android.Widget;
using System.Linq;
using Android.Nfc.Tech;
using System.Collections.Generic;
using Xamarin.Essentials;
using Android.Media;
using System.Threading.Tasks;

namespace Com.Android.Example.Beam {
	[Activity(Label = "MonoDroid BeamDemo", MainLauncher = true)]
	public class Beam : Activity {

		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
		}
	}
}