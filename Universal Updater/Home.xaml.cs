using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System.Profile;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinUniversalTool.Models;
using Windows.System.Display;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading;
using Telnet;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Universal_Updater
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        ulong build = (ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion) & 0x00000000FFFF0000L) >> 16;
        string InstalledLocation = Windows.Application­Model.Package.Current.Installed­Location.Path;
        StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;
        static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        TelnetClient tClient = new TelnetClient(TimeSpan.FromSeconds(1), cancellationTokenSource.Token);
        RadioButton SelectedUpdate;
        IReadOnlyList<StorageFile> UpdateFiles;
        StorageFolder UpdateFolder;
        StorageFolder Packages;
        StorageFolder DownPkgFold;
        StorageFolder filteredFolder;
        string[] InstalledPackages;
        string[] LinkFile;
        string[] DownloadLinks;
        string GetUpdateFiles;
        string packageName;
        string[] knownPackages = { "MICROSOFT.MS_BOOTSEQUENCE_RETAIL.EFIESP", "MICROSOFT.MS_BOOTSEQUENCE_RETAIL.MAINOS", "MICROSOFT.MS_COMMSENHANCEMENTGLOBAL.MAINOS", "MICROSOFT.MS_COMMSMESSAGINGGLOBAL.MAINOS", "MICROSOFT.MS_PROJECTA.MAINOS", "MICROSOFT.MICROSOFTPHONEFM.PLATFORMMANIFEST.EFIESP", "MICROSOFT.MICROSOFTPHONEFM.PLATFORMMANIFEST.MAINOS", "MICROSOFT.MICROSOFTPHONEFM.PLATFORMMANIFEST.UPDATEOS", "MICROSOFT.USERINSTALLABLEFM.PLATFORMMANIFEST.MAINOS" };
        int State = 0;
        int flag = 0;
        private DisplayRequest _displayRequest;

        public Home()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                if (File.Exists(@"C:\Windows\System32\cmd.exe") && File.Exists(@"C:\Windows\System32\telnetd.exe"))
                {
                    _ = tClient.Connect();
                    long i = 0;
                    while (tClient.IsConnected == false && i < 1000000)
                    {
                        i++;
                    }
                    if (!tClient.IsConnected)
                    {
                        NextBtn.IsEnabled = false;
                        await new MessageDialog("Make sure you have injected CMD using CMD Injector v2.4.7.0 or higher.", "Required CMD").ShowAsync();
                    }
                    await Task.Delay(200);
                    _ = sendCommand("\"" + InstalledLocation + "\\Contents\\BatchScripts\\GetDeviceInfo.bat\" \"" + LocalFolder.Path + "\" \"" + InstalledLocation + "\"");
                    while (File.Exists($"{LocalFolder.Path}\\End1.txt") == false)
                    {
                        await Task.Delay(200);
                    }
                    DevInfoBox.Text = await FileIO.ReadTextAsync(await LocalFolder.GetFileAsync("DeviceInfo.txt"));
                    NextBtn.IsEnabled = true;
                    _ = sendCommand("\"" + InstalledLocation + "\\Contents\\BatchScripts\\GetInstalledPackages.bat\" \"" + LocalFolder.Path + "\" \"" + InstalledLocation + "\"");
                    while (File.Exists($"{LocalFolder.Path}\\End2.txt") == false)
                    {
                        await Task.Delay(200);
                    }
                    InstPkgBox.Text = await FileIO.ReadTextAsync(await LocalFolder.GetFileAsync("FilteredPackages.txt"));
                    InstalledPackages = await GetLines($"{LocalFolder.Path}\\FilteredPackages.txt");
                    NextBtn.IsEnabled = true;
                }
                else
                {
                    NextBtn.IsEnabled = false;
                    _ = new MessageDialog("Make sure you have injected CMD using CMD Injector v2.4.7.0 or higher.", "Required CMD").ShowAsync();
                }
            }
            catch (Exception ex) { _ = new MessageDialog(ex.Message + "\n" + ex.StackTrace).ShowAsync(); }
        }

        private async Task<string[]> GetLines(string textFile)
        {
            using (var reader = File.OpenText(textFile))
            {
                var Objects = await reader.ReadToEndAsync();
                return Objects.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            }
        }

        private async Task sendCommand(string command)
        {
            await Task.Run(() =>
            {
                _ = tClient.Send(command);
            });
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UpdtPivot.SelectedIndex != 0)
            {
                UpdtPivot.SelectedIndex = UpdtPivot.SelectedIndex - 1;
                NextBtn.IsEnabled = true;
                if (UpdtPivot.SelectedIndex == 0)
                {
                    BackBtn.IsEnabled = false;
                }
            }
        }

        private async void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UpdtPivot.SelectedIndex != UpdtPivot.Items.Count - 1)
            {
                UpdtPivot.SelectedIndex = UpdtPivot.SelectedIndex + 1;
                BackBtn.IsEnabled = true;
                if (UpdtPivot.SelectedIndex == UpdtPivot.Items.Count - 1)
                {
                    NextBtn.IsEnabled = false;
                }
            }
            if (UpdtPivot.SelectedIndex == 1 && DevInfoBox.Text == "Getting device info . . ." || UpdtPivot.SelectedIndex == 2 && InstalledPackages == null)
            {
                NextBtn.IsEnabled = false;
            }
            if (UpdtPivot.SelectedIndex == 3)
            {
                if (build <= 10240) BuildOne.IsEnabled = true;
                else if (build == 10549) BuildTwo.IsEnabled = true;
                else if (build == 10570) BuildFour.IsEnabled = true;
                else if (build == 10586) BuildFour.IsEnabled = true;
                else if (build == 14393)
                {
                    BuildFive.IsEnabled = true;
                    BuildSix.IsEnabled = true;
                }
                else if (build == 15063) BuildSix.IsEnabled = true;
                if (BuildOne.IsChecked == false && BuildTwo.IsChecked == false && BuildThree.IsChecked == false && BuildFour.IsChecked == false && BuildFive.IsChecked == false && BuildSix.IsChecked == false && UpdateFiles == null && UpdateFolder == null)
                {
                    NextBtn.IsEnabled = false;
                }
            }
            if (UpdtPivot.SelectedIndex == 4)
            {
                if (UpdtFileBox.Text != "Completed.")
                {
                    NextBtn.IsEnabled = false;
                }
                flag = State;
                if (State == 0)
                {
                    flag = 3;
                }
                else if (State == 1 || State == 2)
                {
                    DownPkgBtn.Content = "Load";
                }
                else if (State == 3)
                {
                    PkgTitleBox.Text = "Download Links:";
                    DownPkgBox.Text = "Getting Packages Link . . .";
                    DownTitleBox.Text = "Download Packages:";
                    DownPkgBtn.Content = "Download";
                    /*_ = sendCommand("\"" + InstalledLocation + "\\Contents\\BatchScripts\\GetDownloadLinks.bat\" \"" + SelectedUpdate.Content.ToString() + "\" \"" + LocalFolder.Path + "\" \"" + InstalledLocation + "\"");
                    while (File.Exists($"{LocalFolder.Path}\\End3.txt") == false)
                    {
                        await Task.Delay(200);
                    }
                    DownPkgBox.Text = await FileIO.ReadTextAsync(await LocalFolder.GetFileAsync("Downloads.txt"));*/
                    if (SelectedUpdate.Content.ToString() == "10.0.10549.4") LinkFile = await GetLines($"{InstalledLocation}\\Contents\\Updates\\13016.108.txt");
                    else if (SelectedUpdate.Content.ToString() == "10.0.10570.0") LinkFile = await GetLines($"{InstalledLocation}\\Contents\\Updates\\13037.0.txt");
                    else if (SelectedUpdate.Content.ToString() == "10.0.10586.107") LinkFile = await GetLines($"{InstalledLocation}\\Contents\\Updates\\13080.107.txt");
                    else if (SelectedUpdate.Content.ToString() == "10.0.14393.1066") LinkFile = await GetLines($"{InstalledLocation}\\Contents\\Updates\\14393.1066.txt");
                    else if (SelectedUpdate.Content.ToString() == "10.0.15063.297") LinkFile = await GetLines($"{InstalledLocation}\\Contents\\Updates\\15063.297.txt");
                    else if (SelectedUpdate.Content.ToString() == "10.0.15254.603") LinkFile = await GetLines($"{InstalledLocation}\\Contents\\Updates\\15254.603.txt");
                    await LocalFolder.CreateFileAsync("DownloadLinks.txt", CreationCollisionOption.ReplaceExisting);
                    await Task.Run(() =>
                    {
                        for (int i = 0; i < InstalledPackages.Length; i++)
                        {
                            for (int j = 0; j < LinkFile.Length; j++)
                            {
                                if (LinkFile[j].ToUpper().Contains(InstalledPackages[i].ToUpper() + ".CAB")) CheckLinkExist(LinkFile[j], InstalledPackages[i].ToUpper() + ".CAB");
                                else if (LinkFile[j].ToUpper().Contains(InstalledPackages[i].ToUpper() + ".SPKG")) CheckLinkExist(LinkFile[j], InstalledPackages[i].ToUpper() + ".SPKG");
                                else if (LinkFile[j].ToUpper().Contains(InstalledPackages[i].ToUpper() + ".CBS_")) CheckLinkExist(LinkFile[j], InstalledPackages[i].ToUpper() + ".CBS_");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[0] + ".CAB")) CheckLinkExist(LinkFile[j], knownPackages[0] + ".CAB");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[1] + ".CAB")) CheckLinkExist(LinkFile[j], knownPackages[1] + ".CAB");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[2] + ".CAB")) CheckLinkExist(LinkFile[j], knownPackages[2] + ".CAB");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[3] + ".CAB")) CheckLinkExist(LinkFile[j], knownPackages[3] + ".CAB");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[4] + ".CAB")) CheckLinkExist(LinkFile[j], knownPackages[4] + ".CAB");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[5] + ".CAB")) CheckLinkExist(LinkFile[j], knownPackages[5] + ".CAB");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[6] + ".CAB")) CheckLinkExist(LinkFile[j], knownPackages[6] + ".CAB");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[7] + ".CAB")) CheckLinkExist(LinkFile[j], knownPackages[7] + ".CAB");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[8] + ".CAB")) CheckLinkExist(LinkFile[j], knownPackages[8] + ".CAB");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[0] + ".SPKG")) CheckLinkExist(LinkFile[j], knownPackages[0] + ".SPKG");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[1] + ".SPKG")) CheckLinkExist(LinkFile[j], knownPackages[1] + ".SPKG");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[2] + ".SPKG")) CheckLinkExist(LinkFile[j], knownPackages[2] + ".SPKG");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[3] + ".SPKG")) CheckLinkExist(LinkFile[j], knownPackages[3] + ".SPKG");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[4] + ".SPKG")) CheckLinkExist(LinkFile[j], knownPackages[4] + ".SPKG");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[4] + ".CBS_")) CheckLinkExist(LinkFile[j], knownPackages[4] + ".CBS_");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[5] + ".CBS_")) CheckLinkExist(LinkFile[j], knownPackages[5] + ".CBS_");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[6] + ".CBS_")) CheckLinkExist(LinkFile[j], knownPackages[6] + ".CBS_");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[7] + ".CBS_")) CheckLinkExist(LinkFile[j], knownPackages[7] + ".CBS_");
                                else if (LinkFile[j].ToUpper().Contains(knownPackages[8] + ".CBS_")) CheckLinkExist(LinkFile[j], knownPackages[8] + ".CBS_");
                            }
                        }
                    });
                    DownPkgBox.Text = await FileIO.ReadTextAsync(await LocalFolder.GetFileAsync("DownloadLinks.txt"));
                    DownloadLinks = await GetLines(LocalFolder.Path + "\\DownloadLinks.txt");
                    DownPkgBtn.IsEnabled = true;
                    State = 0;
                }
            }
        }

        private void CheckLinkExist(string Link, string Package)
        {
            string line = File.ReadAllText(LocalFolder.Path + "\\DownloadLinks.txt");
            if (!line.ToUpper().Contains(Package))
            {
                File.AppendAllText(LocalFolder.Path + "\\DownloadLinks.txt", Link + "\r\n");
            }
        }

        private void UpdtRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            State = 3;
            SelectedUpdate = sender as RadioButton;
            NextBtn.IsEnabled = true;
            DownPkgBtn.IsEnabled = false;
            UpdtFileBox.Text = string.Empty;
            DownProgress.Value = 0;
            UpdtBtn.IsEnabled = true;
            DownFileProgress.Visibility = Visibility.Visible;
        }

        private async void PackageSelBtn_Click(object sender, RoutedEventArgs e)
        {
            BuildOne.IsChecked = false;
            BuildTwo.IsChecked = false;
            BuildThree.IsChecked = false;
            BuildFour.IsChecked = false;
            BuildFive.IsChecked = false;
            BuildSix.IsChecked = false;
            NextBtn.IsEnabled = false;
            Button button = sender as Button;
            PkgTitleBox.Text = "Selected Packages:";
            DownPkgBox.Text = "Getting Selected Packages . . .";
            DownTitleBox.Text = "Load Packages:";
            UpdtFileBox.Text = string.Empty;
            DownProgress.Value = 0;
            DownFileProgress.Visibility = Visibility.Collapsed;
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.ViewMode = PickerViewMode.List;
            filePicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            filePicker.FileTypeFilter.Add(".spkg");
            filePicker.FileTypeFilter.Add(".cab");
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            folderPicker.FileTypeFilter.Add(".spkg");
            folderPicker.FileTypeFilter.Add(".cab");
            GetUpdateFiles = string.Empty;
            if (button.Content.ToString() == "Select Files")
            {
                UpdateFiles = await filePicker.PickMultipleFilesAsync();
                if (UpdateFiles != null)
                {
                    State = 1;
                    NextBtn.IsEnabled = true;
                    await Task.Run(() =>
                    {
                        foreach (var file in UpdateFiles)
                        {
                            GetUpdateFiles += file.Name + "\n";
                        }
                    });
                }
            }
            else
            {
                UpdateFolder = await folderPicker.PickSingleFolderAsync();
                if (UpdateFolder != null)
                {
                    State = 2;
                    NextBtn.IsEnabled = true;
                    await Task.Run(async () =>
                    {
                        foreach (var file in await UpdateFolder.GetFilesAsync())
                        {
                            if (file.FileType.ToUpper() == ".CAB" || file.FileType.ToUpper() == ".SPKG") GetUpdateFiles += file.Name + "\n";
                        }
                    });
                }
            }
            if (GetUpdateFiles != string.Empty)
            {
                DownPkgBox.Text = GetUpdateFiles;
                DownPkgBtn.IsEnabled = true;
                UpdtBtn.IsEnabled = true;
            }
            else
            {
                UpdateFiles = null;
                NextBtn.IsEnabled = false;
            }
        }

        private async void DownPkgBtn_Click(object sender, RoutedEventArgs e)
        {
            BuildSelItm.IsEnabled = false;
            DownPkgBtn.IsEnabled = false;
            Packages = await LocalFolder.CreateFolderAsync("Packages", CreationCollisionOption.ReplaceExisting);
            int FileCount = 0;
            if (flag == 1)
            {
                DownProgress.Maximum = UpdateFiles.Count();
                IProgress<double> progress = new Progress<double>(value =>
                {
                    DownProgress.Value = value;
                });
                foreach (var file in UpdateFiles)
                {
                    UpdtFileBox.Text = $"[{++FileCount}\\{DownProgress.Maximum}] {file.Name}";
                    await file.CopyAsync(Packages);
                    DownProgress.Value++;
                }
            }
            else if (flag == 2)
            {
                try
                {
                    string[] lines = GetUpdateFiles.Split('\n');
                    filteredFolder = await UpdateFolder.CreateFolderAsync("Filtered Packages", CreationCollisionOption.ReplaceExisting);
                    DownProgress.Maximum = InstalledPackages.Length - 1;
                    DownProgress.IsIndeterminate = true;
                    for (int i = 0; i < InstalledPackages.Length - 1; i++)
                    {
                        foreach (var line in lines)
                        {
                            if (line.ToUpper().Contains(knownPackages[0] + ".CAB") || line.ToUpper().Contains(knownPackages[1] + ".CAB") || line.ToUpper().Contains(knownPackages[2] + ".CAB") || line.ToUpper().Contains(knownPackages[3] + ".CAB") || line.ToUpper().Contains(knownPackages[4] + ".CAB") || line.ToUpper().Contains(knownPackages[5] + ".CAB") || line.ToUpper().Contains(knownPackages[6] + ".CAB") || line.ToUpper().Contains(knownPackages[7] + ".CAB") || line.ToUpper().Contains(knownPackages[8] + ".CAB")
                            || line.ToUpper().Contains(knownPackages[0] + ".SPKG") || line.ToUpper().Contains(knownPackages[1] + ".SPKG") || line.ToUpper().Contains(knownPackages[2] + ".SPKG") || line.ToUpper().Contains(knownPackages[3] + ".SPKG") || line.ToUpper().Contains(knownPackages[4] + ".SPKG")
                            || line.ToUpper().Contains(knownPackages[4] + ".CBS_") || line.ToUpper().Contains(knownPackages[5] + ".CBS_") || line.ToUpper().Contains(knownPackages[6] + ".CBS_") || line.ToUpper().Contains(knownPackages[7] + ".CBS_") || line.ToUpper().Contains(knownPackages[8] + ".CBS_"))
                            {
                                var file = await UpdateFolder.GetFileAsync(line);
                                if (file != null)
                                {
                                    await file.CopyAsync(filteredFolder, line, NameCollisionOption.ReplaceExisting);
                                    break;
                                }
                            }
                        }
                    }
                    DownProgress.IsIndeterminate = false;
                    for (int i = 0; i < InstalledPackages.Length - 1; i++)
                    {
                        foreach (var line in lines)
                        {
                            if (line.ToUpper().Contains(InstalledPackages[i].ToUpper() + ".CAB") || line.ToUpper().Contains(InstalledPackages[i].ToUpper() + ".SPKG") || line.ToUpper().Contains(InstalledPackages[i].ToUpper() + ".CBS_"))
                            {
                                UpdtFileBox.Text = $"[{++FileCount}\\{DownProgress.Maximum}] {line}";
                                var file = await UpdateFolder.GetFileAsync(line);
                                if (file != null)
                                {
                                    await file.MoveAsync(filteredFolder, line, NameCollisionOption.ReplaceExisting);
                                }
                                break;
                            }
                        }
                        DownProgress.Value++;
                    }
                }
                catch (Exception ex) { await new MessageDialog(ex.Message).ShowAsync(); }
                if (UpdtFileBox.Text == string.Empty)
                {
                    BuildSelItm.IsEnabled = true;
                    flag = 0;
                    _ = new MessageDialog("Selected folder doesn't contain packages for your device. Use \"Select Files\" instead you want to push anyway.").ShowAsync();
                    return;
                }
            }
            else if (flag == 3)
            {
                FolderPicker folderPicker = new FolderPicker();
                folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
                folderPicker.FileTypeFilter.Add("*");
                StorageFolder downloadsFolder = await folderPicker.PickSingleFolderAsync();
                if (downloadsFolder != null)
                {
                    IProgress<int> progress = new Progress<int>(value =>
                    {
                        if (value == -50)
                        {
                            UpdtFileBox.Text = "The update download has paused due to the internet connection lost. It will continue once the connection is back.";
                        }
                        else
                        {
                            DownFileProgress.Value = value;
                            UpdtFileBox.Text = $"[{FileCount}\\{DownloadLinks.Length - 1}] {packageName}";
                        }
                    });
                    DownProgress.Maximum = DownloadLinks.Length - 1;
                    DownPkgFold = await downloadsFolder.CreateFolderAsync($"Universal Updater ({SelectedUpdate.Content.ToString()})", CreationCollisionOption.ReplaceExisting);
                    for (int i = 0; i < DownloadLinks.Length - 1; i++)
                    {
                        Uri sourceFile = new Uri(DownloadLinks[i]);
                        packageName = Path.GetFileName(sourceFile.LocalPath);
                        UpdtFileBox.Text = $"[{++FileCount}\\{DownloadLinks.Length - 1}] {packageName}";
                        try
                        {
                            await Downloader.downloadFile(sourceFile, await DownPkgFold.CreateFileAsync(packageName, CreationCollisionOption.OpenIfExists), progress);
                            if (DownFileProgress.Value != 100)
                            {
                                i--;
                                continue;
                            }
                            DownProgress.Value = FileCount;
                            DownFileProgress.Value = 0;
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "Excep_FromHResult 0x80072EE4")
                            {
                                UpdtFileBox.Text = "Downloading the update is failed. Please try downloading again.";
                            }
                            else
                            {
                                UpdtFileBox.Text = "Please turn on the cellular data or connect to a wifi and then press Download again.";
                            }
                            DownPkgBtn.IsEnabled = true;
                        }
                    }
                }
                else
                {
                    DownPkgBtn.IsEnabled = true;
                    BuildSelItm.IsEnabled = true;
                    return;
                }
            }
            UpdtFileBox.Text = "Completed.";
            NextBtn.IsEnabled = true;
            BuildSelItm.IsEnabled = true;
            flag = 0;
        }

        private async void UpdtBtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                UpdtLoadGif.Source = new BitmapImage(new Uri("ms-appx:///Assets/Gifs/LoadingLight.gif"));
            }
            else
            {
                UpdtLoadGif.Source = new BitmapImage(new Uri("ms-appx:///Assets/Gifs/LoadingDark.gif"));
            }
            UpdtBtn.IsEnabled = false;
            UpdtLoadGif.Visibility = Visibility.Visible;
            UpdtLoadText.Visibility = Visibility.Visible;
            BuildSelItm.IsEnabled = false;
            if (File.Exists($"{LocalFolder.Path}\\End3.txt"))
            {
                File.Delete($"{LocalFolder.Path}\\End3.txt");
                File.Delete($"{LocalFolder.Path}\\Result.txt");
            }
            if (_displayRequest == null)
            {
                _displayRequest = new DisplayRequest();
                _displayRequest.RequestActive();
            }
            if (BuildOne.IsChecked == true || BuildTwo.IsChecked == true || BuildThree.IsChecked == true || BuildFour.IsChecked == true || BuildFive.IsChecked == true || BuildSix.IsChecked == true)
            {
                _ = sendCommand("\"" + InstalledLocation + "\\Contents\\BatchScripts\\SetUpdatePackages.bat\" \"" + LocalFolder.Path + "\" \"" + InstalledLocation + "\" \"" + DownPkgFold.Path + "\"");
            }
            else if (UpdateFolder != null)
            {
                _ = sendCommand("\"" + InstalledLocation + "\\Contents\\BatchScripts\\SetUpdatePackages.bat\" \"" + LocalFolder.Path + "\" \"" + InstalledLocation + "\" \"" + filteredFolder.Path + "\"");
            }
            else if (UpdateFiles != null)
            {
                _ = sendCommand("\"" + InstalledLocation + "\\Contents\\BatchScripts\\SetUpdatePackages.bat\" \"" + LocalFolder.Path + "\" \"" + InstalledLocation + "\" \"" + Packages.Path + "\"");
            }
            while (File.Exists($"{LocalFolder.Path}\\End3.txt") == false)
            {
                await Task.Delay(200);
            }
            UpdtLoadGif.Visibility = Visibility.Collapsed;
            UpdtLoadText.Visibility = Visibility.Collapsed;
            string result = await FileIO.ReadTextAsync(await LocalFolder.GetFileAsync("Result.txt"));
            _ = new MessageDialog(result, "Update Failed").ShowAsync();
            if (_displayRequest != null)
            {
                _displayRequest.RequestRelease();
                _displayRequest = null;
            }
            BuildSelItm.IsEnabled = true;
        }
    }
}
