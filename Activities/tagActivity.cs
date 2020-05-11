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
using Android.Content.PM;

namespace Com.Android.Example.Beam.Activities {
	[IntentFilter(new[] {
		NfcAdapter.ActionNdefDiscovered,
		NfcAdapter.ActionTechDiscovered,
		NfcAdapter.ActionTagDiscovered
	}, Categories = new[] { Intent.CategoryDefault })]
	[Activity(Label = "TagScannerActivity", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, WindowSoftInputMode = SoftInput.StateHidden | SoftInput.AdjustResize)]
	[MetaData(NfcAdapter.ActionTechDiscovered, Resource = "@xml/nfc_tech_filter")]
	public class tagActivity : Activity {

		NfcAdapter mNfcAdapter;
		TextView mInfoText;


		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			HandleIntent(Intent);


			mInfoText = FindViewById<TextView>(Resource.Id.textview);
			// Check for available NFC Adapter
			mNfcAdapter = NfcAdapter.GetDefaultAdapter(this);

			if (mNfcAdapter == null) {
				mInfoText = FindViewById<TextView>(Resource.Id.textview2);
				mInfoText.Text = "NFC is not available on this device.";
			}
		}

		protected void showId(string id) {
			//mInfoText = FindViewById<TextView>(Resource.Id.textview);
			//mInfoText.Text = $"Tag ID: {id}";
		}
		protected void showData(string data) {
			//mInfoText = FindViewById<TextView>(Resource.Id.textview2);
			//mInfoText.Text = $"Data: {data}";
		}
		protected void showTech(string tech) {
			//mInfoText = FindViewById<TextView>(Resource.Id.textview3);
			vibrate();
			soundNotificationDefault();



			//mInfoText.Text = $"Techs: {tech}";
		}

		public async Task createDialogAsync(string tagId, string TagTechs, string ndefMessage) {
			//make dialog accept the id and data
			var dialog = new CustomDialog(this, tagId, TagTechs, ndefMessage);
			dialog.Show();
			await Task.Delay(2000);
			dialog.Dismiss();
			this.Finish();
		}

		protected override void OnResume() {
			base.OnResume();
			if (mNfcAdapter != null && mNfcAdapter.IsEnabled) {

				var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
				var defDetected = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
				var techDetected = new IntentFilter(NfcAdapter.ActionTechDiscovered);

				var filters = new[] { tagDetected, defDetected, techDetected };


				var intent = new Intent(this, this.GetType()).AddFlags(ActivityFlags.SingleTop);
				var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);
				mNfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
			}
		}

		protected override void OnPause() {
			base.OnPause();
			mNfcAdapter.DisableForegroundDispatch(this);
		}

		protected void vibrate() {
			var vibrator = (Vibrator)this.GetSystemService(Context.VibratorService);
			vibrator.Vibrate(VibrationEffect.CreateOneShot(150, VibrationEffect.DefaultAmplitude));
		}

		protected void soundNotificationCustom(string tech) {
			bool yeet = true;
			MediaPlayer player;
			if (yeet) {
				if (tech == "IsoDep, NfcA, NdefFormatable") {
					Vibration.Vibrate(TimeSpan.FromSeconds(1));
					player = MediaPlayer.Create(this, Resource.Raw.yeah_1);
					player.Start();
				}
				else if (tech == "NfcA, MifareClassic, Ndef") {
					player = MediaPlayer.Create(this, Resource.Raw.yeet_1);
					player.Start();
				}
				else {
					player = MediaPlayer.Create(this, Resource.Raw.yeet_3);
					player.Start();
				}
			}
		}

		protected void soundNotificationDefault() {
			try {
				var notification = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
				var r = RingtoneManager.GetRingtone(this, notification);
				r.Play();
			}
			catch (FeatureNotSupportedException e) {
				Console.WriteLine(e);
			}
		}

		protected override void OnNewIntent(Intent intent) {
			HandleIntent(intent);
		}

		protected void HandleIntent(Intent intent) {
			var action = intent.Action;
			if (NfcAdapter.ActionNdefDiscovered.Equals(action)
				|| NfcAdapter.ActionTechDiscovered.Equals(action)
				|| NfcAdapter.ActionTagDiscovered.Equals(action)) {

				var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;
				string[] tagTechs = tag.GetTechList();
				var ndefMessages = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
				var ndefMessagesList = new List<string>();



				if (tagTechs.Length != 0) {
					showTech(string.Join(", ", tagTechs.Select(b => b.Replace("android.nfc.tech.", ""))));
				}

				if (tag != null) {
					var tagId = string.Join("", tag.GetId().Select(b => b.ToString("X2")));
					showId(tagId);
				}

				if (ndefMessages != null) {
					foreach (var msg in ndefMessages.Cast<NdefMessage>()) {
						foreach (var record in msg.GetRecords()) {
							//check if the record type is T (Text)
							if (System.Text.Encoding.UTF8.GetString(record.GetTypeInfo()) == "T") {
								var payloadBytes = record.GetPayload();
								var languageLength = payloadBytes[0];
								var language = string.Join("", payloadBytes.Take(languageLength));

								var payload = System.Text.Encoding.UTF8.GetString(payloadBytes.Skip(3).ToArray());
								ndefMessagesList.Add(payload);

							}
							else {
								showData("Unsupported ndef record type");
							}
						}
					}
					showData(string.Join("\n", ndefMessagesList.ToArray()));
				}
				else {
					showData("No extra data");
				}
				var t = string.Join("", tag.GetId().Select(b => b.ToString("X2")));
				var tt = string.Join(", ", tagTechs.Select(b => b.Replace("android.nfc.tech.", "")));
				var m = string.Join("\n", ndefMessagesList.ToArray());
				createDialogAsync(t, tt, m);
			}
		}
		/*
		var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;
		if (tag != null) {
			string[] techs = tag.GetTechList();
		}

		var ndefMessages = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
		if(ndefMessages != null) {
			foreach (var msg in ndefMessages.Cast<NdefMessage>()) {
				foreach (var rcd in msg.GetRecords()) {
					byte[] typeBytes = rcd.GetTypeInfo();
					byte[] payloadBytes = rcd.GetPayload();
					var type = Encoding.UTF8.GetString(typeBytes);
					var payload = Encoding.UTF8.GetString(payloadBytes);
				}
			}
		}
		*/


		// onResume gets called after this to handle the intent
		//Intent = intent;
	}
}