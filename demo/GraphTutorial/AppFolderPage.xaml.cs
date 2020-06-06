using Microsoft.Graph;
using Microsoft.Toolkit.Graph.Providers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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

        public ObservableCollection<DriveItem> FileItems { get; set; } = new ObservableCollection<DriveItem>();

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
                        //OutputText.Text += $"📁 Name: {item.Name} Description: {item.Description} \n";

                    }
                    else
                        FileItems.Add(item);

                }

            }
            catch (ServiceException ex)
            {
                ShowNotification($"Exception getting events: {ex.Message}");
            }

            base.OnNavigatedTo(e);
        }

        private async void GetFilesBTN_Click(object sender, RoutedEventArgs e)
        {
            var graphClient = ProviderManager.Instance.GlobalProvider.Graph;


            if (FileTitleTXBX.Tag == null)
            {
                // save a new DriveItem
                Microsoft.Graph.File newFile = new Microsoft.Graph.File();
                var fileContents= "";
                FileBodyREB.TextDocument.GetText(Windows.UI.Text.TextGetOptions.UseObjectText, out fileContents);

                DriveItem newDI = new DriveItem
                {
                    Name = FileTitleTXBX.Text,
                };

                StorageFolder localFolder = ApplicationData.Current.LocalCacheFolder;
                var fileName = $"{FileTitleTXBX.Text}.txt";
                StorageFile sf = await localFolder.CreateFileAsync(fileName);
                var filePath = Path.Combine(sf.Path, fileName);

                await FileIO.WriteTextAsync(sf, fileContents);
                FileStream fileStream = new FileStream(sf.Path, FileMode.Open);
                
                DriveItem uploadedFile = await graphClient.Me.Drive.Special.AppRoot
                                               .ItemWithPath(fileName)
                                               .Content
                                               .Request()
                                               .PutAsync<DriveItem>(fileStream);
                fileStream.Close();
                fileStream.Dispose();
                

                if (uploadedFile != null)
                    FileItems.Add(uploadedFile);
            }
            else
            {
                // Update the drive item
            }
        }

        private void FileTitleTXBX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FileTitleTXBX.Text))
                GetFilesBTN.IsEnabled = false;
            else
                GetFilesBTN.IsEnabled = true;


        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
