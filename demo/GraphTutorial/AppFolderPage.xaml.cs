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


            if (string.IsNullOrWhiteSpace(FileTitleTXBX.Tag.ToString()))
            {
                // save a new DriveItem
                Microsoft.Graph.File newFile = new Microsoft.Graph.File();
                var fileContents= "";
                FileBodyREB.TextDocument.GetText(Windows.UI.Text.TextGetOptions.FormatRtf, out fileContents);

                DriveItem newDI = new DriveItem
                {
                    Name = FileTitleTXBX.Text,
                };

                using(StreamWriter sw = new StreamWriter(FileTitleTXBX.Text + ".txt"))
                {
                    sw.Write(newDI.Content);
                }

                var response = await graphClient.Me.Drive.Special.AppRoot
                    .Request().CreateAsync(newDI);


                //  // get reference to stream of file in OneDrive
                //  var fileName = "myNewSmallFile.txt";
                //  var currentFolder = System.IO.Directory.GetCurrentDirectory();
                //  var filePath = Path.Combine(currentFolder, fileName);
                    
                //  // get a stream of the local file
                //  FileStream fileStream = new FileStream(filePath, FileMode.Open);
                    
                //  // upload the file to OneDrive
                //  GraphServiceClient graphClient = GetAuthenticatedGraphClient(...);
                //  var uploadedFile = graphClient.Me.Drive.Root
                //                                .ItemWithPath(fileName)
                //                                .Content
                //                                .Request()
                //                                .PutAsync<DriveItem>(fileStream)
                //                                .Result;

            }
            else
            {
                // Update the drive item
            }
        }
    }
}
