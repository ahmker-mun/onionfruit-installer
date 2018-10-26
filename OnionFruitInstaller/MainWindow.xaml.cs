using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using Telerik.Windows.Controls;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows.Documents;

namespace OnionFruitInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RadWindow
    {
        public MainWindow()
        {
            StyleManager.ApplicationTheme = new MaterialTheme();
            InitializeComponent();
            if (File.Exists(Path.Combine(tempdir, "OnionFruit.msi")))
                File.Delete(Path.Combine(tempdir, "OnionFruit.msi"));
            if (File.Exists(Path.Combine(tempdir, "OnionUninstall.exe")))
                File.Delete(Path.Combine(tempdir, "OnionUninstall.exe"));
        }

        private void wizard_Cancel(object sender, NavigationButtonsEventArgs e)
        {
            //cleanup
            Application.Current.Shutdown();
        }
        bool install = true;
        string tempdir = Path.GetTempPath();
        JObject details;

        private async void WizardPage_Loaded(object sender, RoutedEventArgs e)
        {
            window.CanClose = false;
            details = await Task.Run(async () => JObject.Parse(await new HttpClient().GetAsync("https://onionfruit.dragonfruit.ml/info.json").Result.Content.ReadAsStringAsync()));
            status.Content = "Downloading Files";
            await Task.Run(() => new WebClient().DownloadFile(new Uri((string)details["uninstallerlink"]), Path.Combine(tempdir, "OnionUninstall.exe")));

            if (install)
                await Task.Run(() => new WebClient().DownloadFile(new Uri((string)details["installerlink"]), Path.Combine(tempdir, "OnionFruit.msi")));

            status.Content = "Executing Uninstaller";
            await Task.Run(() => System.Diagnostics.Process.Start(Path.Combine(tempdir, "OnionUninstall.exe")).WaitForExit());
            if (install)
            {
                status.Content = "Installing OnionFruit Connect (V" + details["version"].ToString() + ")";
                Process process = new Process();
                process.StartInfo.WorkingDirectory = Environment.GetEnvironmentVariable("SystemRoot"); //sets the working directory in which the exe file resides.
                process.StartInfo.FileName = "msiexec.exe";

                process.StartInfo.Arguments = string.Format(@"/i " + Path.Combine(tempdir, "OnionFruit.msi") + " /passive"); //Pass the number of arguments.

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;
                process.Start();
                await Task.Run(() => process.WaitForExit());
            }
            status.Content = "Cleaning Up...";
            File.Delete(Path.Combine(tempdir, "OnionUninstall.exe"));
            if (install)
                File.Delete(Path.Combine(tempdir, "OnionFruit.msi"));
            installer.AllowNext = true;
            loading.IsBusy = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (chkbox.IsChecked == true)
                p1.AllowNext = true;
            else
                p1.AllowNext = false;
        }

        private void RadRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            install = true;
        }

        private void RadRadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            install = false;
        }

        private void WizardPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            window.CanClose = true;

        }

        private void p1_Loaded(object sender, RoutedEventArgs e)
        {
            licencebox.Document.Blocks.Add(new Paragraph(new Run("End-User License Agreement (EULA) of OnionFruit Connect")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("This End-User License Agreement (\"EULA\") is a legal agreement between you and DragonFruit Network")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("This EULA agreement governs your acquisition and use of our OnionFruit Connect software (\"Software\") directly from DragonFruit Network or indirectly through a DragonFruit Network authorized reseller or distributor (a \"Reseller\").")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("Please read this EULA agreement carefully before completing the installation process and using the OnionFruit Connect software. It provides a license to use the OnionFruit Connect software and contains warranty information and liability disclaimers.")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("If you are entering into this EULA agreement on behalf of a company or other legal entity, you represent that you have the authority to bind such entity and its affiliates to these terms and conditions. If you do not have such authority or if you do not agree with the terms and conditions of this EULA agreement, do not install or use the Software, and you must not accept this EULA agreement.")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("This EULA agreement shall apply only to the Software supplied by DragonFruit Network herewith regardless of whether other software is referred to or described herein. The terms also apply to any DragonFruit Network updates, supplements, Internet-based services, and support services for the Software, unless other terms accompany those items on delivery. If so, those terms apply.")));

            licencebox.Document.Blocks.Add(new Paragraph(new Run("License Grant")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("DragonFruit Network hereby grants you a personal, non-transferable, non-exclusive licence to use the OnionFruit Connect software on your devices in accordance with the terms of this EULA agreement.")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("You are permitted to load the OnionFruit Connect software (for example a PC, laptop, mobile or tablet) under your control. You are responsible for ensuring your device meets the minimum requirements of the OnionFruit Connect software.")));

            licencebox.Document.Blocks.Add(new Paragraph(new Run("You are not permitted to:")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("- Edit, alter, modify, adapt, translate or otherwise change the whole or any part of the Software nor permit the whole or any part of the Software to be combined with or become incorporated in any other software, nor decompile, disassemble or reverse engineer the Software or attempt to do any such things")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("- Reproduce, copy, distribute, resell or otherwise use the Software for any commercial purpose")));

            licencebox.Document.Blocks.Add(new Paragraph(new Run("- Allow any third party to use the Software on behalf of or for the benefit of any third party")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("- Use the Software in any way which breaches any applicable local, national or international law")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("- use the Software for any purpose that DragonFruit Network considers is a breach of this EULA agreement")));

            licencebox.Document.Blocks.Add(new Paragraph(new Run("Intellectual Property and Ownership")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("DragonFruit Network shall at all times retain ownership of the Software as originally downloaded by you and all subsequent downloads of the Software by you. The Software (and the copyright, and other intellectual property rights of whatever nature in the Software, including any modifications made thereto) are and shall remain the property of DragonFruit Network.")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("DragonFruit Network reserves the right to grant licences to use the Software to third parties.")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("Termination")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("This EULA agreement is effective from the date you first use the Software and shall continue until terminated. You may terminate it at any time upon written notice to DragonFruit Network.")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("It will also terminate immediately if you fail to comply with any term of this EULA agreement. Upon such termination, the licenses granted by this EULA agreement will immediately terminate and you agree to stop all access and use of the Software. The provisions that by their nature continue and survive will survive any termination of this EULA agreement.")));

            licencebox.Document.Blocks.Add(new Paragraph(new Run("Governing Law")));
            licencebox.Document.Blocks.Add(new Paragraph(new Run("This EULA agreement, and any dispute arising out of or in connection with this EULA agreement, shall be governed by and construed in accordance with the laws of the United Kingdom.")));

        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Window)
            {
                Window thisWindow = this.Parent as Window;
                thisWindow.ShowInTaskbar = true;
                thisWindow.Title = this.Header.ToString();
            }

        }
    }
}
