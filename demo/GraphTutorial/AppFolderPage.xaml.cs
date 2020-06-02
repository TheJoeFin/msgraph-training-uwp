using Microsoft.Graph;
using Microsoft.Toolkit.Graph.Providers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GraphTutorial
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppFolderPage : Page
    {
        public AppFolderPage()
        {
            this.InitializeComponent();
        }

        private void ShowNotification(string message)
        {
            // Get the main page that contains the InAppNotification
            var mainPage = (Window.Current.Content as Frame).Content as MainPage;

            // Get the notification control
            var notification = mainPage.FindName("Notification") as InAppNotification;

            notification.Show(message);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get the Graph client from the provider
            var graphClient = ProviderManager.Instance.GlobalProvider.Graph;

            try
            {
                // Get the events
                DriveItem drive = await graphClient.Me.Drive.Special.AppRoot
                    .Request().GetAsync();

                IDriveItemChildrenCollectionPage files = await graphClient.Me.Drive.Special.AppRoot.Children
                    .Request().GetAsync();

                foreach (DriveItem item in files)
                {
                    if (item.Folder != null)
                    {
                        OutputText.Text += $"📁 Name: {item.Name} Description: {item.Description} \n";

                    }
                    else
                        OutputText.Text += "📄 Name: {item.Name} Description: {item.Description} \n";

                }

            }
            catch (ServiceException ex)
            {
                ShowNotification($"Exception getting events: {ex.Message}");
            }

            base.OnNavigatedTo(e);
        }

        private void GetFilesBTN_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
