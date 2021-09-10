using Plugin.Fingerprint.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamBiometric
{
	public partial class MainPage : ContentPage
	{
		private CancellationTokenSource _cancel;
		private bool _initialized;

		public MainPage()
		{
			InitializeComponent();
		}
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!_initialized)
            {
                _initialized = true;
                lblAuthenticationType.Text = "Auth Type: " + await Plugin.Fingerprint.CrossFingerprint.Current.GetAuthenticationTypeAsync();
            }
        }

        private async void OnAuthenticate(object sender, EventArgs e)
        {
            await AuthenticateAsync("Prove you have fingers!");
        }

        private async void OnAuthenticateLocalized(object sender, EventArgs e)
        {
            await AuthenticateAsync("Beweise, dass du Finger hast!", "Abbrechen", "Anders!", "Viel zu schnell!");
        }

        private async Task AuthenticateAsync(string reason, string cancel = null, string fallback = null, string tooFast = null)
        {
            _cancel = swAutoCancel.IsToggled ? new CancellationTokenSource(TimeSpan.FromSeconds(10)) : new CancellationTokenSource();
            lblStatus.Text = "";

            var dialogConfig = new AuthenticationRequestConfiguration("XamBiometric", reason)
            { // all optional
                CancelTitle = cancel,
                FallbackTitle = fallback,
                AllowAlternativeAuthentication = swAllowAlternative.IsToggled,
                ConfirmationRequired = swConfirmationRequired.IsToggled
            };

            // optional
            dialogConfig.HelpTexts.MovedTooFast = tooFast;

            var result = await Plugin.Fingerprint.CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);

            SetResult(result);
        }

        private void SetResult(FingerprintAuthenticationResult result)
        {
            if (result.Authenticated)
            {
                var navPage = new NavigationPage(new SecretPage());
                Application.Current.MainPage = navPage;
            }
            else
            {
                lblStatus.Text = $"{result.Status}: {result.ErrorMessage}";
            }
        }
    }
}
