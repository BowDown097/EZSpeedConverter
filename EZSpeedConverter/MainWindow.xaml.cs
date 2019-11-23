using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace EZSpeedConverter
{
    public partial class MainWindow : Window
    {
        string file = "";
        string[] files = { };
        double speed = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Begin_Click(object sender, RoutedEventArgs e)
        {
            string directory = selecteddir.Text;
            if (includeSubdirs.IsChecked == true && fileOnly.IsChecked == false)
            {
                files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            }
            else if(includeSubdirs.IsChecked == false && fileOnly.IsChecked == false)
            {
                files = Directory.GetFiles(directory);
            }
            else
            {
                file = selectedfile.Text;
            }
            if (string.IsNullOrEmpty(directory) && fileOnly.IsChecked == false)
            {
                MessageBox.Show("You didn't select an Input/Output directory! Click the \"...\" button next to the directory text box to do so.", "Error!");
            }
            else if(string.IsNullOrEmpty(file) && fileOnly.IsChecked == true)
            {
                MessageBox.Show("You didn't select an Input/Output file! Click the \"...\" button next to the file text box to do so.", "Error!");
            }
            else
            {
                int totalTasks = files.Length;
                if(totalTasks == 0)
                {
                    MessageBox.Show("Your conversion task on file " + file + " will begin.", "Information");
                }
                else
                {
                    MessageBox.Show("A total of " + totalTasks + " conversion tasks will be ran, with " + Environment.ProcessorCount + " tasks running concurrently.", "Information");
                }
                ProcessStartInfo winStartInfo = new ProcessStartInfo
                {
                    FileName = Directory.GetCurrentDirectory() + "\\ffmpeg\\" + "ffmpeg.exe",
                    RedirectStandardInput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                BackgroundWorker bw = new BackgroundWorker();
                if (string.IsNullOrEmpty(file))
                {
                    if (Menu_KeepPitch.IsChecked == true)
                    {
                        if (ToPAL.IsChecked == true)
                        {
                            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                            {
                                Parallel.ForEach(files, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, currentFile =>
                                {
                                    // create command process
                                    Process cmdWin = new Process();
                                    cmdWin.StartInfo = winStartInfo;
                                    cmdWin.StartInfo.Arguments = "-i \"" + currentFile + "\" -r 25 -filter_complex \"[0:v]setpts = 0.959 * PTS[v];[0:a] atempo=1.0427[a]\" -map \"[v]\" -map \"[a]\" -b:a 192k \"" + directory + "\\" + Path.GetFileNameWithoutExtension(currentFile) + "-PAL.mkv\"";
                                    cmdWin.Start();
                                    // flush, close
                                    cmdWin.StandardInput.Flush();
                                    cmdWin.StandardInput.Close();

                                    cmdWin.WaitForExit();
                                    if (deleteAfterConvert.IsChecked == true)
                                    {
                                        File.Delete(currentFile);
                                    }
                                });
                            });
                        }
                        else if (ToNTSC.IsChecked == true)
                        {
                            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                            {
                                Parallel.ForEach(files, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, currentFile =>
                                {
                                    // create command process
                                    Process cmdWin = new Process();
                                    cmdWin.StartInfo = winStartInfo;
                                    cmdWin.StartInfo.Arguments = "-i \"" + currentFile + "\" -r 23.976 -filter_complex \"[0:v]setpts = 1.0427 * PTS[v];[0:a] atempo=0.959[a]\" -map \"[v]\" -map \"[a]\" -b:a 192k \"" + directory + "\\" + Path.GetFileNameWithoutExtension(currentFile) + "-NTSC.mkv\"";
                                    cmdWin.Start();

                                    // flush, close
                                    cmdWin.StandardInput.Flush();
                                    cmdWin.StandardInput.Close();

                                    cmdWin.WaitForExit();
                                    if (deleteAfterConvert.IsChecked == true)
                                    {
                                        File.Delete(currentFile);
                                    }
                                });
                            });
                        }
                        else if (speed != 0)
                        {
                            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                            {
                                Parallel.ForEach(files, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, currentFile =>
                                {
                                    // create command process
                                    Process cmdWin = new Process();
                                    cmdWin.StartInfo = winStartInfo;
                                    cmdWin.StartInfo.Arguments = "-i \"" + currentFile + "\" -filter_complex \"[0:v]setpts = " + Math.Truncate(Math.Round(Math.Pow(speed, -1), 5) * 10000) / 10000 + " * PTS[v];[0:a] atempo=" + speed + "[a]\" -map \"[v]\" -map \"[a]\" -b:a 192k \"" + directory + "\\" + Path.GetFileNameWithoutExtension(currentFile) + "-EZSC.mkv\"";
                                    cmdWin.Start();

                                    // flush, close
                                    cmdWin.StandardInput.Flush();
                                    cmdWin.StandardInput.Close();

                                    cmdWin.WaitForExit();
                                    if (deleteAfterConvert.IsChecked == true)
                                    {
                                        File.Delete(currentFile);
                                    }
                                });
                            });
                        }
                    }
                    else
                    {
                        if (ToPAL.IsChecked == true)
                        {
                            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                            {
                                Parallel.ForEach(files, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, currentFile =>
                                {
                                    // create command process
                                    Process cmdWin = new Process();
                                    cmdWin.StartInfo = winStartInfo;
                                    cmdWin.StartInfo.Arguments = "-i \"" + currentFile + "\" -r 25 -filter_complex \"[0:v]setpts = 0.959 * PTS[v];[0:a] asetrate=48000*1.0427,aresample=48000[a]\" -map \"[v]\" -map \"[a]\" -b:a 192k \"" + directory + "\\" + Path.GetFileNameWithoutExtension(currentFile) + "-PAL.mkv\"";
                                    cmdWin.Start();
                                    // flush, close
                                    cmdWin.StandardInput.Flush();
                                    cmdWin.StandardInput.Close();

                                    cmdWin.WaitForExit();
                                    if (deleteAfterConvert.IsChecked == true)
                                    {
                                        File.Delete(currentFile);
                                    }
                                });
                            });
                        }
                        else if (ToNTSC.IsChecked == true)
                        {
                            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                            {
                                Parallel.ForEach(files, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, currentFile =>
                                {
                                    // create command process
                                    Process cmdWin = new Process();
                                    cmdWin.StartInfo = winStartInfo;
                                    cmdWin.StartInfo.Arguments = "-i \"" + currentFile + "\" -r 23.976 -filter_complex \"[0:v]setpts = 1.0427 * PTS[v];[0:a] asetrate=48000*0.959,aresample=48000[a]\" -map \"[v]\" -map \"[a]\" -b:a 192k \"" + directory + "\\" + Path.GetFileNameWithoutExtension(currentFile) + "-NTSC.mkv\"";
                                    cmdWin.Start();

                                    // flush, close
                                    cmdWin.StandardInput.Flush();
                                    cmdWin.StandardInput.Close();

                                    cmdWin.WaitForExit();
                                    if (deleteAfterConvert.IsChecked == true)
                                    {
                                        File.Delete(currentFile);
                                    }
                                });
                            });
                        }
                        else if (speed != 0)
                        {
                            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                            {
                                Parallel.ForEach(files, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, currentFile =>
                                {
                                    // create command process
                                    Process cmdWin = new Process();
                                    cmdWin.StartInfo = winStartInfo;
                                    cmdWin.StartInfo.Arguments = "-i \"" + currentFile + "\" -filter_complex \"[0:v]setpts = " + Math.Truncate(Math.Round(Math.Pow(speed, -1), 5) * 10000) / 10000 + " * PTS[v];[0:a] asetrate=48000*" + speed + ",aresample=48000[a]\" -map \"[v]\" -map \"[a]\" -b:a 192k \"" + directory + "\\" + Path.GetFileNameWithoutExtension(currentFile) + "-EZSC.mkv\"";
                                    cmdWin.Start();

                                    // flush, close
                                    cmdWin.StandardInput.Flush();
                                    cmdWin.StandardInput.Close();

                                    cmdWin.WaitForExit();
                                    if (deleteAfterConvert.IsChecked == true)
                                    {
                                        File.Delete(currentFile);
                                    }
                                });
                            });
                        }
                    }
                }
                else
                {
                    if (Menu_KeepPitch.IsChecked == true)
                    {
                        if (ToPAL.IsChecked == true)
                        {
                            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                            {
                                // create command process
                                Process cmdWin = new Process();
                                cmdWin.StartInfo = winStartInfo;
                                cmdWin.StartInfo.Arguments = "-i \"" + file + "\" -r 25 -filter_complex \"[0:v]setpts = 0.959 * PTS[v];[0:a] atempo=1.0427[a]\" -map \"[v]\" -map \"[a]\" -b:a 192k \"" + Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + "-PAL.mkv\"";
                                cmdWin.Start();
                                // flush, close
                                cmdWin.StandardInput.Flush();
                                cmdWin.StandardInput.Close();

                                cmdWin.WaitForExit();
                                if (deleteAfterConvert.IsChecked == true)
                                {
                                    File.Delete(file);
                                }
                            });
                        }
                        else if (ToNTSC.IsChecked == true)
                        {
                            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                            {
                                // create command process
                                Process cmdWin = new Process();
                                cmdWin.StartInfo = winStartInfo;
                                cmdWin.StartInfo.Arguments = "-i \"" + file + "\" -r 23.976 -filter_complex \"[0:v]setpts = 1.0427 * PTS[v];[0:a] atempo=0.959[a]\" -map \"[v]\" -map \"[a]\" -b:a 192k \"" + Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + "-NTSC.mkv\"";
                                cmdWin.Start();

                                // flush, close
                                cmdWin.StandardInput.Flush();
                                cmdWin.StandardInput.Close();

                                cmdWin.WaitForExit();
                                if (deleteAfterConvert.IsChecked == true)
                                {
                                    File.Delete(file);
                                }
                            });
                        }
                        else if (speed != 0)
                        {
                            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                            {
                                // create command process
                                Process cmdWin = new Process();
                                cmdWin.StartInfo = winStartInfo;
                                cmdWin.StartInfo.Arguments = "-i \"" + file + "\" -filter_complex \"[0:v]setpts = " + Math.Truncate(Math.Round(Math.Pow(speed, -1), 5) * 10000) / 10000 + " * PTS[v];[0:a] atempo=" + speed + "[a]\" -map \"[v]\" -map \"[a]\" -b:a 192k \"" + Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + "-EZSC.mkv\"";
                                cmdWin.Start();

                                // flush, close
                                cmdWin.StandardInput.Flush();
                                cmdWin.StandardInput.Close();

                                cmdWin.WaitForExit();
                                if (deleteAfterConvert.IsChecked == true)
                                {
                                    File.Delete(file);
                                }   
                            });
                        }
                    }
                    else
                    {
                        if (ToPAL.IsChecked == true)
                        {
                            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                            {
                                // create command process
                                Process cmdWin = new Process();
                                cmdWin.StartInfo = winStartInfo;
                                cmdWin.StartInfo.Arguments = "-i \"" + file + "\" -r 25 -filter_complex \"[0:v]setpts = 0.959 * PTS[v];[0:a] asetrate=48000*1.0427,aresample=48000[a]\" -map \"[v]\" -map \"[a]\" -b:a 192k \"" + Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + "-PAL.mkv\"";
                                cmdWin.Start();
                                // flush, close
                                cmdWin.StandardInput.Flush();
                                cmdWin.StandardInput.Close();

                                cmdWin.WaitForExit();
                                if (deleteAfterConvert.IsChecked == true)
                                {
                                    File.Delete(file);
                                }
                            });
                        }
                        else if (ToNTSC.IsChecked == true)
                        {
                            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                            {
                                // create command process
                                Process cmdWin = new Process();
                                cmdWin.StartInfo = winStartInfo;
                                cmdWin.StartInfo.Arguments = "-i \"" + file + "\" -r 23.976 -filter_complex \"[0:v]setpts = 1.0427 * PTS[v];[0:a] asetrate=48000*0.959,aresample=48000[a]\" -map \"[v]\" -map \"[a]\" -b:a 192k \"" + Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + "-NTSC.mkv\"";
                                cmdWin.Start();

                                // flush, close
                                cmdWin.StandardInput.Flush();
                                cmdWin.StandardInput.Close();

                                cmdWin.WaitForExit();
                                if (deleteAfterConvert.IsChecked == true)
                                {
                                    File.Delete(file);
                                }
                            });
                        }
                        else if (speed != 0)
                        {
                            bw.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs args)
                            {
                                // create command process
                                Process cmdWin = new Process();
                                cmdWin.StartInfo = winStartInfo;
                                cmdWin.StartInfo.Arguments = "-i \"" + file + "\" -filter_complex \"[0:v]setpts = " + Math.Truncate(Math.Round(Math.Pow(speed, -1), 5) * 10000) / 10000 + " * PTS[v];[0:a] asetrate=48000*" + speed + ",aresample=48000[a]\" -map \"[v]\" -map \"[a]\" -b:a 192k \"" + Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + "-EZSC.mkv\"";
                                cmdWin.Start();

                                // flush, close
                                cmdWin.StandardInput.Flush();
                                cmdWin.StandardInput.Close();

                                cmdWin.WaitForExit();
                                if (deleteAfterConvert.IsChecked == true)
                                {
                                    File.Delete(file);
                                }
                            });
                        }
                    }
                }
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate (object o, RunWorkerCompletedEventArgs args)
                {
                    MessageBox.Show("Your conversion operation has completed! Check to see if it worked.", "Success!");
                });
                bw.RunWorkerAsync();
            }
        }

        private void FindDir_Click(object sender, RoutedEventArgs e)
        {
            string selected = "";
            using(System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selected = dialog.SelectedPath;
                }
            }
            selecteddir.Text = selected;
        }

        private void FindFile_Click(object sender, RoutedEventArgs e)
        {
            string selected = "";
            using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
            {
                if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selected = dialog.FileName;
                }
            }
            selectedfile.Text = selected;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            foreach (var process in Process.GetProcesses().Where(proc => proc.ProcessName == "ffmpeg"))
            {
                process.Kill();
            }
        }

        private void FileOnly_Checked(object sender, RoutedEventArgs e)
        {
            selecteddir.IsEnabled = false;
            selectdir.IsEnabled = false;
            ToPAL.IsEnabled = false;
            ToPAL.IsChecked = false;
            ToNTSC.IsEnabled = false;
            ToNTSC.IsChecked = false;
            includeSubdirs.IsEnabled = false;
            selectedfile.IsEnabled = true;
            selectfile.IsEnabled = true;
            ToPALFile.IsEnabled = true;
            ToNTSCFile.IsEnabled = true;
            ToPALFile.IsChecked = true;
        }

        private void FileOnly_Unchecked(object sender, RoutedEventArgs e)
        {
            selecteddir.IsEnabled = true;
            selectdir.IsEnabled = true;
            ToPAL.IsEnabled = true;
            ToPAL.IsChecked = true;
            ToNTSC.IsEnabled = true;
            includeSubdirs.IsEnabled = true;
            selectedfile.IsEnabled = false;
            selectfile.IsEnabled = false;
            ToPALFile.IsEnabled = false;
            ToNTSCFile.IsEnabled = false;
            ToPALFile.IsChecked = false;
            ToNTSCFile.IsChecked = false;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == true)
            {
                MenuButton.Margin = new Thickness(600, 129, 0, 0);
                MenuButton.Content = ">";
                Menu.Visibility = Visibility.Visible;
            }
            else
            {
                MenuButton.Margin = new Thickness(762, 129, 0, 0);
                MenuButton.Content = "<";
                Menu.Visibility = Visibility.Hidden;
            }
        }

        private void Menu_PresetSpeed_Checked(object sender, RoutedEventArgs e)
        {
            Menu_SpeedLabel.Visibility = Visibility.Visible;
            Menu_Speed.Visibility = Visibility.Visible;
            Menu_OKSpeed.Visibility = Visibility.Visible;
        }

        private void Menu_PresetSpeed_Unchecked(object sender, RoutedEventArgs e)
        {
            Menu_SpeedLabel.Visibility = Visibility.Hidden;
            Menu_Speed.Visibility = Visibility.Hidden;
            Menu_OKSpeed.Visibility = Visibility.Hidden;
        }

        private void Menu_OKSpeed_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Menu_Speed.Text, out _))
            {
                speed = double.Parse(Menu_Speed.Text);
                Menu_SpeedLabel.Visibility = Visibility.Hidden;
                Menu_Speed.Visibility = Visibility.Hidden;
                Menu_OKSpeed.Visibility = Visibility.Hidden;
                Menu_PresetSpeed.IsChecked = false;
                ToPALFile.Visibility = Visibility.Hidden;
                ToPALFile.IsChecked = false;
                ToNTSCFile.Visibility = Visibility.Hidden;
                ToNTSCFile.IsChecked = false;
                ToPAL.Visibility = Visibility.Hidden;
                ToPAL.IsChecked = false;
                ToNTSC.Visibility = Visibility.Hidden;
                ToNTSC.IsChecked = false;
                CustomSpeedLabel.Visibility = Visibility.Visible;
                CustomSpeedLabel.Content = "Custom Speed: " + speed + "x";
            }
            else
            {
                MessageBox.Show("Speed input is not a number! Please put in a number (ie. 0.957).", "Error!");
            }
        }
    }
}