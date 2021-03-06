﻿using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Webkit;

namespace Plugin.OAuth2.Components
{
    [Activity(Label = "WebViewActivity")]
    internal class WebViewActivity : Activity
    {
        private const string UserAgentString = "Mozilla/5.0 (Android 4.4; Mobile; rv:41.0) Gecko/41.0 Firefox/41.0";

        public delegate void OnNavigatingDelegate(string Uri);

        public delegate void OnCancelingDelegate();

        private class BrowserViewClient : WebViewClient
        {
            private readonly OnNavigatingDelegate OnNavigating;

            public override void OnPageStarted(WebView view, string url, Bitmap favicon)
            {
                OnNavigating(url);
            }

            public BrowserViewClient(OnNavigatingDelegate onNavigating)
            {
                OnNavigating = onNavigating;
            }
        };

        public event OnNavigatingDelegate OnNavigating;
        public event OnCancelingDelegate OnCanceling;

        private WebView BrowserView { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetTheme(Android.Resource.Style.ThemeDeviceDefaultDialogNoActionBar);

            BrowserView = new WebView(this);
            var client = new BrowserViewClient(d => OnNavigating?.Invoke(d));
            BrowserView.Settings.UserAgentString = UserAgentString;
            BrowserView.Settings.JavaScriptEnabled = true;
            BrowserView.SetWebViewClient(client);
            SetContentView(BrowserView);

            var startUri = Intent.GetStringExtra(AuthorizationUriAcquirer.IntentStartUriKey);
            BrowserView.LoadUrl(startUri);
        }

        protected override void OnStop()
        {
            OnCanceling?.Invoke();

            base.OnStop();
        }
    }
}