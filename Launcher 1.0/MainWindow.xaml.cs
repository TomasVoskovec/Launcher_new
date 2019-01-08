using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Launcher_1._0
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Dictionary<Button, string> filePaths = new Dictionary<Button, string>();

        string curResult;
        string curExeFile;

        List<FileInfo> getFileInfos(string path, string fileName)
        {
            return new DirectoryInfo(path.ToString()).GetFiles(fileName, SearchOption.AllDirectories).ToList();
        }

        void renderIcos(List<FileInfo> files)
        {
            foreach(FileInfo file in files)
            {
                var fileType = file.Directory.Extension;
                createIco(file.Name, file.Directory.ToString());
            }
        }

        void createIco(string name, string path)
        {
            StackPanel icon = new StackPanel();

            Button newButton = new Button();
            newButton.Width = 80;
            newButton.Height = 80;
            newButton.Tag = name;
            newButton.Content = new Image
            {
                Source = new BitmapImage(new Uri("http://icons.iconarchive.com/icons/pelfusion/flat-file-type/128/exe-icon.png")),
                VerticalAlignment = VerticalAlignment.Center
            };
            newButton.Background = Brushes.Transparent;
            newButton.BorderBrush = Brushes.Transparent;
            newButton.Margin = new Thickness(5);
            newButton.HorizontalAlignment = HorizontalAlignment.Left;
            newButton.VerticalAlignment = VerticalAlignment.Top;
            newButton.Click += icon_click;

            ContextMenu contextMenu = new ContextMenu();

            MenuItem delete = new MenuItem();
            delete.Header = "Delete";
            delete.Tag = path + "/" + name;
            delete.Click += deleteFile_click;
            contextMenu.Items.Add(delete);

            MenuItem copy = new MenuItem();
            copy.Header = "Copy";
            copy.Tag = path + "/" + name;
            copy.Click += copyFile_click;
            contextMenu.Items.Add(copy);

            MenuItem move = new MenuItem();
            move.Header = "Move";
            move.Tag = path + "/" + name;
            move.Click += moveFile_click;
            contextMenu.Items.Add(move);

            newButton.ContextMenu = contextMenu;

            icon.Children.Add(newButton);

            TextBlock nameText = new TextBlock
            {
                Text = name,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Width = 80,
                Foreground = Brushes.White,
            };

            icon.Children.Add(nameText);
            
            filePaths.Add(newButton, path);

            iconPanel.Children.Insert(0, icon);
        }

        private void deleteFile_click(object sender, EventArgs e)
        {
            MenuItem thisItem = sender as MenuItem;

            if (File.Exists(thisItem.Tag.ToString()))
            {
                File.Delete(thisItem.Tag.ToString());
                iconPanel.Children.Clear();
                renderIcos(getFileInfos(curResult, curExeFile));
            }
            else
            {
                MessageBox.Show("This file canot be delted");
            }
        }
        
        void copyFile_click(object sender, EventArgs e)
        {
            MenuItem thisItem = sender as MenuItem;

            string thisFilePath = thisItem.Tag.ToString();
            string thisFileName = System.IO.Path.GetFileName(thisFilePath);

            try
            {
                string result ="";

                using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        result = dialog.SelectedPath;
                    }
                }

                string destFilePath = result + "/" + thisFileName;

                if (File.Exists(thisFilePath))
                {
                    if (File.Exists(destFilePath))
                    {
                        bool isExistName = true;
                        int copyNum = 1;

                        while (isExistName)
                        {
                            if (File.Exists(destFilePath))
                            {
                                copyNum++;
                                destFilePath = result + "/" + thisFileName.Replace(".exe", "(" + copyNum.ToString() + ").exe");
                            }
                            else
                            {
                                File.Copy(thisFilePath, destFilePath);
                                iconPanel.Children.Clear();
                                renderIcos(getFileInfos(curResult, curExeFile));
                                isExistName = false;
                            }
                        }
                    }
                    else
                    {
                        File.Copy(thisFilePath, destFilePath);
                        iconPanel.Children.Clear();
                        renderIcos(getFileInfos(curResult, curExeFile));
                    }
                }
                else
                {
                    MessageBox.Show("This file does not exist");
                }
            }
            catch (Exception copyE)
            {
                MessageBox.Show("Copy error: " + copyE.Message);
            }
        }

        void moveFile_click(object sender, EventArgs e)
        {
            MenuItem thisItem = sender as MenuItem;

            string thisFilePath = thisItem.Tag.ToString();
            string thisFileName = System.IO.Path.GetFileName(thisFilePath);

            try
            {
                string result = "";

                using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        result = dialog.SelectedPath;
                    }
                }

                string destFilePath = result + "/" + thisFileName;

                if (File.Exists(thisFilePath))
                {
                    if (!File.Exists(destFilePath))
                    {
                        File.Copy(thisFilePath, destFilePath);
                        File.Delete(thisFilePath);
                        iconPanel.Children.Clear();
                        renderIcos(getFileInfos(curResult, curExeFile));
                    }
                    else 
                    {
                        MessageBox.Show("File with this name already in this folder");
                    }
                }
                else
                {
                    MessageBox.Show("This file does not exist");
                }
            }
            catch (Exception copyE)
            {
                MessageBox.Show("Copy error: " + copyE.Message);
            }
        }

        void openFile(string fileName, string directory)
        {
            Process proc = new Process();

            try
            {
                proc.StartInfo.FileName = fileName;
                proc.StartInfo.WorkingDirectory = directory;
                proc.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show("Aplication did not start (" + e.Message + ")");
            }
        }

        private void icon_click(object sender, RoutedEventArgs e)
        {
            Button ico = sender as Button;

            string directory = filePaths[ico];

            openFile(ico.Tag.ToString(), directory);
        }

        private void selectPath_click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog x = new OpenFileDialog();
                x.Multiselect = false;
                x.Filter = "All Files (*.exe*)|*.csproj*";
                x.ShowDialog();
                string filePath = x.FileName;
                string result = System.IO.Path.GetDirectoryName(filePath);
                string fileName = x.SafeFileName;
                string exeFile = fileName.Replace(".csproj", ".exe");

                MessageBox.Show("Selected directory: " + result);

                iconPanel.Children.Clear();
                renderIcos(getFileInfos(result, exeFile));

                curResult = result;
                curExeFile = exeFile;
            }
            catch (Exception pathE)
            {
                MessageBox.Show("Error: " + pathE.Message);
            }
        }
    }
}
