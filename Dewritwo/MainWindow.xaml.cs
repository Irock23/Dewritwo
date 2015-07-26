﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Dewritwo.Resources;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Crc32C;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Dewritwo
{
    public partial class MainWindow
    {
        #region Main
        public MainWindow()
        {
            InitializeComponent();
            
            try
            {
                Cfg.Initial(false);
                Load();
                AppendDebugLine("Cfg Load Complete", Color.FromRgb(0, 255, 0));
            }
            catch
            {
                AppendDebugLine("Cfg Load Error: Resetting Launcher Specific Settings", Color.FromRgb(255, 0, 0));
                Cfg.Initial(true);
                Load();
                AppendDebugLine("Cfg Reload Complete", Color.FromRgb(0, 255, 0));
            }
            try
            {
                InitialHash();
            }
            catch
            {
                AppendDebugLine("Files not found. Make sure this exe is in your install folder", Color.FromRgb(255, 0, 0));
                BTNAction.Content = "Error";
            }
        }
        private void InitialHash()
        {
            var watch = Stopwatch.StartNew();
            AppendDebugLine("CRC32 of tags.dat: " + Append("maps/tags.dat"), Color.FromRgb(255, 255, 0));
            AppendDebugLine("CRC32 of audio.dat: " + Append("maps/audio.dat"), Color.FromRgb(255, 255, 0));
            AppendDebugLine("CRC32 of textures.dat: " + Append("maps/textures.dat"), Color.FromRgb(255, 255, 0));
            AppendDebugLine("CRC32 of textures_b.dat: " + Append("maps/textures_b.dat"), Color.FromRgb(255, 255, 0));
            AppendDebugLine("CRC32 of resources.dat: " + Append("maps/resources.dat"), Color.FromRgb(255, 255, 0));
            AppendDebugLine("CRC32 of string_ids.dat: " + Append("maps/string_ids.dat"), Color.FromRgb(255, 255, 0));
            AppendDebugLine("CRC32 of video.dat: " + Append("maps/video.dat"), Color.FromRgb(255, 255, 0));
            AppendDebugLine("CRC32 of halo3.zip: " + Append("mods/medals/halo3.zip"), Color.FromRgb(255, 255, 0));
            watch.Stop();
            double seconds = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            AppendDebugLine("Hash Complete in: " + seconds + " Seconds", Color.FromRgb(0, 255, 0));
            BTNAction.Content = "Update";
        }

        public static uint Append(string filePath)
        {
            byte[] fileSize = BitConverter.GetBytes(new FileInfo(filePath).Length);
            uint initialHash = Crc32CAlgorithm.Compute(ReadFile(filePath));
            uint append = Crc32CAlgorithm.Append(initialHash, fileSize);
            return append;
        }

        public static byte[] ReadFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[1024 * 512];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        #endregion

        #region Variables/Dictionaries
        private Dictionary<int, string> doritoKey;
        private bool updateText = true;
        private string keyValue;
        private string bindDelete;
        #endregion

        #region Flyout Controls

        private void FlyoutHandler(Grid sender, string header)
        {
            Flyout.IsOpen = true;
            SettingsGrid.Visibility = Visibility.Hidden;
            CustomGrid.Visibility = Visibility.Hidden;
            VOIPSettingsGrid.Visibility = Visibility.Hidden;
            AutoExecGrid.Visibility = Visibility.Hidden;
            DebugGrid.Visibility = Visibility.Hidden;
            LauncherSettingsGrid.Visibility = Visibility.Hidden;
            sender.Visibility = Visibility.Visible;
            Flyout.Header = header;
        }

        private void LauncherSettings_Click(object sender, RoutedEventArgs e)
        {
            if (LauncherSettingsGrid.Visibility == Visibility.Visible && Flyout.IsOpen)
            {
                Flyout.IsOpen = false;
            }
            else if (LauncherSettingsGrid.Visibility == Visibility.Hidden)
            {
                Flyout.IsOpen = false;
                FlyoutHandler(LauncherSettingsGrid, "Launcher Settings");
            }
            else
            {
                FlyoutHandler(LauncherSettingsGrid, "Launcher Settings");
            }
        }

        private void Custom_Click(object sender, RoutedEventArgs e)
        {
            FlyoutHandler(CustomGrid, "Player Customization");
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            FlyoutHandler(SettingsGrid, "Settings");
        }

        private void Voip_Click(object sender, RoutedEventArgs e)
        {
            FlyoutHandler(VOIPSettingsGrid, "VOIP Settings");
        }

        private void AutoExec_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> Dict = Dictionaries.GetCommandLine();
            CommandLine.SetValue(TextBoxHelper.WatermarkProperty, Dict[Convert.ToString(Command.SelectedValue)]);
            FlyoutHandler(AutoExecGrid, "Auto Exec");
            Preview.Text = File.ReadAllText("autoexec.cfg");
        }

        private void Debug_OnClick(object sender, RoutedEventArgs e)
        {
            if (DebugGrid.Visibility == Visibility.Visible && Flyout.IsOpen)
            {
                Flyout.IsOpen = false;
            }
            else if (DebugGrid.Visibility != Visibility.Visible)
            {
                Flyout.IsOpen = false;
                FlyoutHandler(DebugGrid, "Debug Log");
            }
            else
            {
                FlyoutHandler(DebugGrid, "Debug Log");
            }
        }

        #endregion

        #region Controls

        #region Menu
        private void Action_OnClick(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "eldorado.exe";
            startInfo.Arguments = "-launcher";

            

            if (BTNAction.Content == "Play Game")
            {
                Process.Start(startInfo);
                if (Cfg.launcherConfigFile["Launcher.Random"] == "1")
                    RandomArmor();
                if (Cfg.launcherConfigFile["Launcher.Close"] == "1")
                    Application.Current.Shutdown();
            }
            else if (BTNAction.Content == "Update")
            {
                Update();
            }
        }

        private async void Update()
        {

            var controller = await this.ShowProgressAsync("Updating to 0.4.9.1", "Downloading: ");

            var i = 0.0;
            while (i < 35.0)
            {
                await Task.Delay(100);

                if (i > 1)
                    controller.SetMessage("Downloading: tags.dat");
                if (i > 10)
                    controller.SetMessage("Downloading: halo3.zip");
                if (i > 20)
                    controller.SetMessage("Downloading: dewritoupdater.exe");
                if (i > 25)
                    controller.SetMessage("Update Complete");

                if (controller.IsCanceled)
                    break; //canceled progressdialog auto closes.
                i += 1.0;
            }
            await controller.CloseAsync();
            BTNAction.Content = "Play Game";
        }

        private void Reddit_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.reddit.com/r/HaloOnline/");
        }

        private void Twitter_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://twitter.com/FishPhdOfficial");
        }

        private void Github_OnClick(object sender, RoutedEventArgs e)
        {
           Process.Start("https://github.com/fishphd");
        }

        #endregion

        #region AutoExec

        private void BindButton_OnGotFocus(object sender, RoutedEventArgs e)
        {
            BindButton.Text = "Press Key";
        }

        private void BindButton_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (BindButton.Text == "Press Key")
            {
                BindButton.Text = "Unbound";
            }
        }

        private void BindButton_OnKeyDown(object sender, KeyEventArgs e)
        {
            var keyPressed = KeyInterop.VirtualKeyFromKey(e.Key);
            BindButton.Text = Convert.ToString(e.Key);
            keyValue = doritoKey.FirstOrDefault(x => x.Key == keyPressed).Value;
            if (!doritoKey.ContainsKey(keyPressed))
            {
                BindButton.Text = "Invalid Key";
            }

            if (CommandLine.IsEnabled == false && BindButton.Text != "Invalid Key")
            {
                AutoExecWrite(Action.SelectedValue + " " + keyValue + " " + Command.SelectedValue,
            Preview.Text.Contains(Convert.ToString(Command.SelectedValue)) && Preview.Text.Contains(Convert.ToString(Action.SelectedValue)), Convert.ToString(Command.SelectedValue));
            }
        }

        private void AutoExecWrite(string write, bool duplicate, string search)
        {
            if (duplicate)
            {
                var lines = File.ReadAllLines("autoexec.cfg");
                if (Action.SelectedValue == "command")
                {
                    foreach (var line in lines)
                    {
                        if (!line.Contains("bind") && line.Contains(Convert.ToString(Command.SelectedValue)))
                        {
                            Preview.Text = Preview.Text.Replace(line, write);
                        }
                    }
                }
                else
                {
                    foreach (var line in lines)
                    {
                        if (line.Contains("bind") && line.Contains(Convert.ToString(Command.SelectedValue)))
                        {
                            Preview.Text = Preview.Text.Replace(line, write);
                        }
                    }
                }
            }
            if (!duplicate)
            {
                string sent = write;
                Preview.AppendText(sent);
                Preview.AppendText(Environment.NewLine);
            }
            File.WriteAllText("autoexec.cfg", Preview.Text);
        }

        private void Preview_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            File.WriteAllText("autoexec.cfg", Preview.Text);
        }

        private void Action_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Action.SelectedValue == "command")
            {
                Command.ItemsSource = Dictionaries.GetCommandList();
                updateText = false;
                CommandLine.Text = "";
                updateText = true;
                To.Visibility = Visibility.Hidden;
                BindButton.Visibility = Visibility.Hidden;
                CommandLine.Visibility = Visibility.Visible;
                Command.Width = 258;
                PreviewPanel.Margin = new Thickness(-3, 0, 0, 0);
                PreviewLabel.Margin = new Thickness(4, 5, 0, 0);
                Console.WriteLine("Command");
            }
            if (Action.SelectedValue == "bind")
            {
                Command.ItemsSource = Dictionaries.GetCommand();
                updateText = false;
                CommandLine.Text = "";
                updateText = true;
                To.Visibility = Visibility.Visible;
                BindButton.Visibility = Visibility.Visible;
                CommandLine.Visibility = Visibility.Visible;
                Command.Width = 150;
                PreviewPanel.Margin = new Thickness(5, 0, 0, 0);
                PreviewLabel.Margin = new Thickness(-4, 5, 0, 0);
                Console.WriteLine("Bind");
            }
        }

        private void CommandLine_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (updateText && Action.SelectedValue == "command")
            {
                AutoExecWrite(Command.SelectedValue + " " + CommandLine.Text,
                    Preview.Text.Contains(Convert.ToString(Command.SelectedValue)),
                    Convert.ToString(Command.SelectedValue));
            }
            else if (updateText && Action.SelectedValue == "bind" && BindButton.Text != "Invalid Key")
            {
                AutoExecWrite(
                    Action.SelectedValue + " " + keyValue + " " + Command.SelectedValue + " " + CommandLine.Text,
                    Preview.Text.Contains(Convert.ToString(Command.SelectedValue)),
                    Convert.ToString(Command.SelectedValue));
            }
        }

        private void Command_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Dictionary<string, string> Selection = Dictionaries.GetCommandLine();
            if (Selection[Convert.ToString(Command.SelectedValue)].Contains("[No Value]"))
            {
                CommandLine.IsEnabled = false;
                
            }
            else
            {
                BindButton.Text = "Unbound";
                updateText = false;
                CommandLine.Text = String.Empty;
                updateText = true;
                CommandLine.IsEnabled = true;
            }
            CommandLine.SetValue(TextBoxHelper.WatermarkProperty, Selection[Convert.ToString(Command.SelectedValue)]);
        }

        #endregion

        #region Debug
        public void AppendDebugLine(string status, Color color, bool updateLabel = true)
        {
            if (DebugLogger.Dispatcher.CheckAccess())
            {
                TextRange tr = new TextRange(DebugLogger.Document.ContentEnd , DebugLogger.Document.ContentEnd);
                tr.Text = status + "\u2028";
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
            }
            else
            {
                DebugLogger.Dispatcher.Invoke(new Action(() => AppendDebugLine(status, color, updateLabel)));
            }
        }
        #endregion

        #endregion

        #region Saving/Loading

        #region Customization

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Player.Name", Name.Text, ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void Helmet_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Player.Armor.Helmet", Convert.ToString(Helmet.SelectedValue), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void Chest_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Player.Armor.Chest", Convert.ToString(Chest.SelectedValue), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void Shoulders_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Player.Armor.Shoulders", Convert.ToString(Shoulders.SelectedValue), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void Arms_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Player.Armor.Arms", Convert.ToString(Arms.SelectedValue), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void Legs_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Player.Armor.Legs", Convert.ToString(Legs.SelectedValue), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void clrPrimary_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (!IsLoaded)
            {
                return;
            }
            var color = Convert.ToString(clrPrimary.SelectedColor).Remove(1, 2);
            Cfg.SetVariable("Player.Colors.Primary", color, ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void clrSecondary_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (!IsLoaded)
            {
                return;
            }
            var color = Convert.ToString(clrSecondary.SelectedColor).Remove(1, 2);
            Cfg.SetVariable("Player.Colors.Secondary", color, ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void clrVisor_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (!IsLoaded)
            {
                return;
            }
            var color = Convert.ToString(clrVisor.SelectedColor).Remove(1, 2);
            Cfg.SetVariable("Player.Colors.Visor", color, ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void clrLights_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (!IsLoaded)
            {
                return;
            }
            var color = Convert.ToString(clrLights.SelectedColor).Remove(1, 2);
            Cfg.SetVariable("Player.Colors.Lights", color, ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void clrHolo_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (!IsLoaded)
            {
                return;
            }
            var color = Convert.ToString(clrHolo.SelectedColor).Remove(1, 2);
            Cfg.SetVariable("Player.Colors.Holo", color, ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void btnRandom_Click(object sender, RoutedEventArgs e)
        {
            RandomArmor();
        }
        private void RandomArmor()
        {
            var r = new Random();
            var helmet = r.Next(0, 25);
            var chest = r.Next(0, 25);
            var shoulders = r.Next(0, 25);
            var arms = r.Next(0, 25);
            var legs = r.Next(0, 25);

            var randomColor = new Random();
            var primary = string.Format("#{0:X6}", randomColor.Next(0x1000000));
            var secondary = string.Format("#{0:X6}", randomColor.Next(0x1000000));
            var visor = string.Format("#{0:X6}", randomColor.Next(0x1000000));
            var lights = string.Format("#{0:X6}", randomColor.Next(0x1000000));
            var holo = string.Format("#{0:X6}", randomColor.Next(0x1000000));

            Helmet.SelectedIndex = helmet;
            Chest.SelectedIndex = chest;
            Shoulders.SelectedIndex = shoulders;
            Arms.SelectedIndex = arms;
            Legs.SelectedIndex = legs;

            clrPrimary.SelectedColor = (Color)ColorConverter.ConvertFromString(primary);
            clrSecondary.SelectedColor = (Color)ColorConverter.ConvertFromString(secondary);
            clrVisor.SelectedColor = (Color)ColorConverter.ConvertFromString(visor);
            clrLights.SelectedColor = (Color)ColorConverter.ConvertFromString(lights);
            clrHolo.SelectedColor = (Color)ColorConverter.ConvertFromString(holo);
            clrPrimary.SelectedColor = (Color)ColorConverter.ConvertFromString(primary);
            clrSecondary.SelectedColor = (Color)ColorConverter.ConvertFromString(secondary);
            clrVisor.SelectedColor = (Color)ColorConverter.ConvertFromString(visor);
            clrLights.SelectedColor = (Color)ColorConverter.ConvertFromString(lights);
            clrHolo.SelectedColor = (Color)ColorConverter.ConvertFromString(holo);

            Cfg.SetVariable("Player.Armor.Chest", Convert.ToString(Chest.SelectedValue), ref Cfg.configFile);
            Cfg.SetVariable("Player.Armor.Shoulders", Convert.ToString(Shoulders.SelectedValue), ref Cfg.configFile);
            Cfg.SetVariable("Player.Armor.Helmet", Convert.ToString(Helmet.SelectedValue), ref Cfg.configFile);
            Cfg.SetVariable("Player.Armor.Arms", Convert.ToString(Arms.SelectedValue), ref Cfg.configFile);
            Cfg.SetVariable("Player.Armor.Legs", Convert.ToString(Legs.SelectedValue), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void RandomCheck_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Launcher.Random", Convert.ToString(Convert.ToInt32(RandomCheck.IsChecked)), ref Cfg.launcherConfigFile);
            Cfg.SaveConfigFile("launcher_prefs.cfg", Cfg.launcherConfigFile);
        }

        #endregion

        #region Settings
        private void Fov_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Camera.FOV", Convert.ToString(Fov.Value), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void CrosshairCenter_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Camera.Crosshair", Convert.ToString(Convert.ToInt32(CrosshairCenter.IsChecked)),
                ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void RawInput_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Input.RawInput", Convert.ToString(Convert.ToInt32(RawInput.IsChecked)), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void ServerName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Server.Name", ServerName.Text, ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void ServerPassword_OnTextChanged(object sender, RoutedEventArgs args)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Server.Password", ServerPassword.Password, ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void MaxPlayer_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Server.MaxPlayers", Convert.ToString(MaxPlayer.Value), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void StartTimer_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Server.Countdown", Convert.ToString(StartTimer.Value), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Fov.Value = 90;
            CrosshairCenter.IsChecked = false;
            RawInput.IsChecked = true;
            ServerName.Text = "HaloOnline Server";
            MaxPlayer.Value = 16;
            StartTimer.Value = 5;
            chkIntro.IsChecked = true;
        }

        private void chkIntro_Changed(object sender, RoutedEventArgs e)
        {
            if (chkIntro.IsChecked == true && Directory.Exists("bink"))
            {
                Directory.Move("bink", "bink_disabled");
            }
            else if (chkIntro.IsChecked == false && Directory.Exists("bink_disabled"))
            {
                Directory.Move("bink_disabled", "bink");
            }
        }

        #endregion

        #region Launcher Settings

        private void Color_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Colors.SelectedValue.ToString()), ThemeManager.GetAppTheme(Cfg.launcherConfigFile["Launcher.Theme"]));
            Cfg.launcherConfigFile["Launcher.Color"] = Colors.SelectedValue.ToString();
            Cfg.SaveConfigFile("launcher_prefs.cfg", Cfg.launcherConfigFile);

            if (Cfg.launcherConfigFile["Launcher.Color"] == "yellow")
            {
                Color Dark = (Color)ColorConverter.ConvertFromString("#252525");
                CustomIcon.Fill = new SolidColorBrush(Dark);
                SettingsIcon.Fill = new SolidColorBrush(Dark);
                VOIPIcon.Fill = new SolidColorBrush(Dark);
                AutoExecIcon.Fill = new SolidColorBrush(Dark);
                Title.SetResourceReference(ForegroundProperty, "AccentColorBrush");
                L.Fill = new SolidColorBrush(Dark);
                E.Fill = new SolidColorBrush(Dark);
                Title.Content = "ELDEWRITO";
            }
            else
            {
                Color Light = (Color)ColorConverter.ConvertFromString("#FFFFFF");
                CustomIcon.Fill = new SolidColorBrush(Light);
                SettingsIcon.Fill = new SolidColorBrush(Light);
                Title.Foreground = new SolidColorBrush(Light);
                Title.SetResourceReference(ForegroundProperty, "AccentColorBrush");
                VOIPIcon.Fill = new SolidColorBrush(Light);
                AutoExecIcon.Fill = new SolidColorBrush(Light);
                Title.Content = "ELDEWRITO";
            }

            if (Cfg.launcherConfigFile["Launcher.Theme"] == "BaseLight" && Cfg.launcherConfigFile["Launcher.Color"] == "yellow")
            {
                Color Light = (Color)ColorConverter.ConvertFromString("#FFFFFF");
                CustomIcon.Fill = new SolidColorBrush(Light);
                SettingsIcon.Fill = new SolidColorBrush(Light);
                VOIPIcon.Fill = new SolidColorBrush(Light);
                AutoExecIcon.Fill = new SolidColorBrush(Light);
                Title.SetResourceReference(ForegroundProperty, "AccentColorBrush");
                L.Fill = new SolidColorBrush(Light);
                E.Fill = new SolidColorBrush(Light);
                Title.Content = "Where is your god now";
            }
        }

        private void Theme_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Cfg.launcherConfigFile["Launcher.Color"]), ThemeManager.GetAppTheme(Themes.SelectedValue.ToString()));
            Cfg.launcherConfigFile["Launcher.Theme"] = Themes.SelectedValue.ToString();
            Cfg.SaveConfigFile("launcher_prefs.cfg", Cfg.launcherConfigFile);

            if (Cfg.launcherConfigFile["Launcher.Theme"] == "BaseLight")
            {
                Title.SetResourceReference(ForegroundProperty, "AccentColorBrush");
            }
            else
            {
                Color Light = (Color)ColorConverter.ConvertFromString("#FFFFFF");
                Title.Foreground = new SolidColorBrush(Light);
            }

            if (Cfg.launcherConfigFile["Launcher.Theme"] == "BaseLight" && Cfg.launcherConfigFile["Launcher.Color"] == "yellow")
            {
                Color Light = (Color)ColorConverter.ConvertFromString("#FFFFFF");
                CustomIcon.Fill = new SolidColorBrush(Light);
                SettingsIcon.Fill = new SolidColorBrush(Light);
                VOIPIcon.Fill = new SolidColorBrush(Light);
                AutoExecIcon.Fill = new SolidColorBrush(Light);
                Title.SetResourceReference(ForegroundProperty, "AccentColorBrush");
                L.Fill = new SolidColorBrush(Light);
                E.Fill = new SolidColorBrush(Light);
                Title.Content = "Where is your god now";
            }

            if (Cfg.launcherConfigFile["Launcher.Theme"] == "BaseDark" && Cfg.launcherConfigFile["Launcher.Color"] == "yellow")
            {
                Color Dark = (Color)ColorConverter.ConvertFromString("#252525");
                CustomIcon.Fill = new SolidColorBrush(Dark);
                SettingsIcon.Fill = new SolidColorBrush(Dark);
                VOIPIcon.Fill = new SolidColorBrush(Dark);
                AutoExecIcon.Fill = new SolidColorBrush(Dark);
                Title.SetResourceReference(ForegroundProperty, "AccentColorBrush");
                L.Fill = new SolidColorBrush(Dark);
                E.Fill = new SolidColorBrush(Dark);
                Title.Content = "ELDEWRITO";
            }
        }

        private void Launch_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("Launcher.Close", Convert.ToString(Convert.ToInt32(Launch.IsChecked)), ref Cfg.launcherConfigFile);
            Cfg.SaveConfigFile("launcher_prefs.cfg", Cfg.launcherConfigFile);
        }

        #endregion

        #region VOIP Settings
        private void AGC_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("VoIP.AGC", Convert.ToString(Convert.ToInt32(AGC.IsChecked)), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void Echo_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("VoIP.EchoCancellation", Convert.ToString(Convert.ToInt32(Echo.IsChecked)), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }
        private void VolumeModifier_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("VoIP.VolumeModifier", Convert.ToString(VolumeModifier.Value), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void PTT_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("VoIP.PushToTalk", Convert.ToString(Convert.ToInt32(PTT.IsChecked)), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void VAL_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded)
            {
                return;
            }
            Cfg.SetVariable("VoIP.VoiceActivationLevel", Convert.ToString(VAL.Value), ref Cfg.configFile);
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        private void btnReset2_Click(object sender, EventArgs e)
        {
            AGC.IsChecked = true;
            Echo.IsChecked = true;
            VolumeModifier.Value = 6;
            PTT.IsChecked = true;
            VAL.Value = -45;
        }

        #endregion

        #region Extra

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Cfg.SaveConfigFile("dewrito_prefs.cfg", Cfg.configFile);
        }

        #endregion

        #region Loading

        private void Load()
        {
            //Customization
            if (Cfg.configFile["Player.Name"] == "Forgot")
                Cfg.SetVariable("Player.Name", "", ref Cfg.configFile);
            Name.Text = Cfg.configFile["Player.Name"];
            Helmet.SelectedValue = Cfg.configFile["Player.Armor.Helmet"];
            Chest.SelectedValue = Cfg.configFile["Player.Armor.Chest"];
            Shoulders.SelectedValue = Cfg.configFile["Player.Armor.Shoulders"];
            Arms.SelectedValue = Cfg.configFile["Player.Armor.Arms"];
            Legs.SelectedValue = Cfg.configFile["Player.Armor.Legs"];
            clrPrimary.SelectedColor =
                (Color) ColorConverter.ConvertFromString(Cfg.configFile["Player.Colors.Primary"]);
            clrSecondary.SelectedColor =
                (Color) ColorConverter.ConvertFromString(Cfg.configFile["Player.Colors.Secondary"]);
            clrVisor.SelectedColor = (Color) ColorConverter.ConvertFromString(Cfg.configFile["Player.Colors.Visor"]);
            clrLights.SelectedColor =
                (Color) ColorConverter.ConvertFromString(Cfg.configFile["Player.Colors.Lights"]);
            clrHolo.SelectedColor = (Color) ColorConverter.ConvertFromString(Cfg.configFile["Player.Colors.Holo"]);
            //Settings
            Fov.Value = Convert.ToDouble(Cfg.configFile["Camera.FOV"]);
            CrosshairCenter.IsChecked = Convert.ToBoolean(Convert.ToInt32(Cfg.configFile["Camera.Crosshair"]));
            RawInput.IsChecked = Convert.ToBoolean(Convert.ToInt32(Cfg.configFile["Input.RawInput"]));
            ServerName.Text = Cfg.configFile["Server.Name"];
            ServerPassword.Password = Cfg.configFile["Server.Password"];
            MaxPlayer.Value = Convert.ToDouble(Cfg.configFile["Server.MaxPlayers"]);
            StartTimer.Value = Convert.ToDouble(Cfg.configFile["Server.Countdown"]);
            //Launcher Settings
            Colors.SelectedValue = Cfg.launcherConfigFile["Launcher.Color"];
            Themes.SelectedValue = Cfg.launcherConfigFile["Launcher.Theme"];
            Launch.IsChecked = Convert.ToBoolean(Convert.ToInt32(Cfg.launcherConfigFile["Launcher.Close"]));
            RandomCheck.IsChecked = Convert.ToBoolean(Convert.ToInt32(Cfg.launcherConfigFile["Launcher.Random"]));
            //VoIP Settings
            AGC.IsChecked = Convert.ToBoolean(Convert.ToInt32(Cfg.configFile["VoIP.AGC"]));
            Echo.IsChecked = Convert.ToBoolean(Convert.ToInt32(Cfg.configFile["VoIP.EchoCancellation"]));
            VolumeModifier.Value = Convert.ToDouble(Cfg.configFile["VoIP.VolumeModifier"]);
            PTT.IsChecked = Convert.ToBoolean(Convert.ToInt32(Cfg.configFile["VoIP.PushToTalk"]));
            VAL.Value = Convert.ToDouble(Cfg.configFile["VoIP.VoiceActivationLevel"]);
            //Auto Exec
            if (!File.Exists("autoexec.cfg"))
                File.Create("autoexec.cfg");
            if (!Directory.Exists("mods/medals"))
                Directory.CreateDirectory("mods/medals");
            if (!Directory.Exists("mods/maps"))
                Directory.CreateDirectory("mods/maps");
            if (!Directory.Exists("mods/variants"))
                Directory.CreateDirectory("mods/variants");
            if (Directory.Exists("bink_disabled"))
                chkIntro.IsChecked = true;

            doritoKey = new Dictionary<int, string>()
            {
                {0x1B, "escape"},
                {0x70, "f1"},
                {0x71, "f2"},
                {0x72, "f3"},
                {0x73, "f4"},
                {0x74, "f5"},
                {0x75, "f6"},
                {0x76, "f7"},
                {0x77, "f8"},
                {0x78, "f9"},
                {0x79, "f10"},
                {0x7A, "f11"},
                {0x7B, "f12"},
                {0x2C, "printscreen"},
                {0x7D, "f14"},
                {0x7E, "f15"},
                {0xC0, "tilde"},
                {0x31, "1"},
                {0x32, "2"},
                {0x33, "3"},
                {0x34, "4"},
                {0x35, "5"},
                {0x36, "6"},
                {0x37, "7"},
                {0x38, "8"},
                {0x39, "9"},
                {0x30, "0"},
                {0xBD, "minus"},
                {0xBB, "plus"},
                {0x8, "back"},
                {0x9, "tab"},
                {0x51, "Q"},
                {0x57, "W"},
                {0x45, "E"},
                {0x52, "R"},
                {0x54, "T"},
                {0x59, "Y"},
                {0x55, "U"},
                {0x49, "I"},
                {0x4F, "O"},
                {0x50, "P"},
                {0xDB, "lbracket"},
                {0xDD, "rbracket"},
                {0xDC, "pipe"},
                {0x14, "capital"},
                {0x41, "A"},
                {0x53, "S"},
                {0x44, "D"},
                {0x46, "F"},
                {0x47, "G"},
                {0x48, "H"},
                {0x4A, "J"},
                {0x4B, "K"},
                {0x4C, "L"},
                {0xBA, "colon"},
                {0xDE, "quote"},
                {0xD, "enter"},
                {0xA0, "lshift"},
                {0x5A, "Z"},
                {0x58, "X"},
                {0x43, "C"},
                {0x56, "V"},
                {0x42, "B"},
                {0x4E, "N"},
                {0x4D, "M"},
                {0xBC, "comma"},
                {0xBE, "period"},
                {0xBF, "question"},
                {0xA1, "rshift"},
                {0xA2, "lcontrol"},
                {0xA4, "lmenu"},
                {0x20, "space"},
                {0xA5, "rmenu"},
                {0x5D, "apps"},
                {0xA3, "rcontrol"},
                {0x26, "up"},
                {0x28, "down"},
                {0x25, "left"},
                {0x27, "right"},
                {0x2D, "insert"},
                {0x24, "home"},
                {0x21, "pageup"},
                {0x2E, "delete"},
                {0x23, "end"},
                {0x22, "pagedown"},
                {0x90, "numlock"},
                {0x6F, "divide"},
                {0x6A, "multiply"},
                {0x60, "numpad0"},
                {0x61, "numpad1"},
                {0x62, "numpad2"},
                {0x63, "numpad3"},
                {0x64, "numpad4"},
                {0x65, "numpad5"},
                {0x66, "numpad6"},
                {0x67, "numpad7"},
                {0x68, "numpad8"},
                {0x69, "numpad9"},
                {0x6D, "subtract"},
                {0x6B, "add"},
                {0x6E, "decimal"},
            };
            
            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent(Cfg.launcherConfigFile["Launcher.Color"]),
                ThemeManager.GetAppTheme(Cfg.launcherConfigFile["Launcher.Theme"]));
            
            if (Cfg.launcherConfigFile["Launcher.Color"] == "yellow")
            {
                Color Dark = (Color) ColorConverter.ConvertFromString("#252525");
                CustomIcon.Fill = new SolidColorBrush(Dark);
                SettingsIcon.Fill = new SolidColorBrush(Dark);
                VOIPIcon.Fill = new SolidColorBrush(Dark);
                AutoExecIcon.Fill = new SolidColorBrush(Dark);
                Title.SetResourceReference(ForegroundProperty, "AccentColorBrush");
                L.Fill = new SolidColorBrush(Dark);
                E.Fill = new SolidColorBrush(Dark);
            }
            if (Cfg.launcherConfigFile["Launcher.Theme"] == "BaseLight")
            {
                Title.SetResourceReference(ForegroundProperty, "AccentColorBrush");
            }
            if (Cfg.launcherConfigFile["Launcher.Theme"] == "BaseLight" &&
                Cfg.launcherConfigFile["Launcher.Color"] == "yellow")
            {
                Color Light = (Color) ColorConverter.ConvertFromString("#FFFFFF");
                CustomIcon.Fill = new SolidColorBrush(Light);
                SettingsIcon.Fill = new SolidColorBrush(Light);
                VOIPIcon.Fill = new SolidColorBrush(Light);
                AutoExecIcon.Fill = new SolidColorBrush(Light);
                Title.SetResourceReference(ForegroundProperty, "AccentColorBrush");
                L.Fill = new SolidColorBrush(Light);
                E.Fill = new SolidColorBrush(Light);
                Title.Content = "Where is your god now";
            }
        }
        #endregion

        #endregion
    }
}



