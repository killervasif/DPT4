using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace CompositeTask
{
    public interface ISystemItem
    {
        string? Name { get; set; }
        string? Location { get; set; }
        double Size { get; }
    }

    public class Folder : ISystemItem
    {
        public List<ISystemItem> SystemItems { get; }

        public Folder(string? name, string? location)
        {
            Name = name;
            Location = location;

            SystemItems = new();
        }

        public string? Name { get; set; }
        public string? Location { get; set; }
        public double Size
        {
            get
            {
                Console.WriteLine(Name);
                return SystemItems.Sum(item => item.Size);
            }
        }


        public void Add(ISystemItem item)
        {
            item.Location = $@"{Location}\{item.Name}";
            SystemItems.Add(item);
        }

        public void Delete(ISystemItem item)
            => SystemItems.Remove(item);


    }

    public class File : ISystemItem
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        public double Size { get; }


        public File(string? name, double size, string? location = "")
        {
            Name = name;
            Location = location;
            Size = size;
        }
    }

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private Folder? currentFolder;

        public Folder? CurrentFolder
        {
            get { return currentFolder; }
            set
            {
                currentFolder = value;
                OnPropertyChanged(nameof(CurrentFolder));
            }
        }



        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderDialog = new();

            if (folderDialog.ShowDialog() is true)
            {
                DirectoryInfo directory = new DirectoryInfo(folderDialog.SelectedPath);

                CurrentFolder = new Folder(directory.Name, directory.FullName);
                listAllFiles(CurrentFolder, directory);
            }

            txtSize.Text = CurrentFolder?.Size.ToString() + " Bytes";
        }



        public void listAllFiles(Folder folder, DirectoryInfo directory)
        {
            // listFilesCurrDir: Table containing the list of files in the 'path' folder
            var listFilesCurrDir = directory.GetFiles();

            foreach (var file in listFilesCurrDir)
                folder.Add(new File(file.Name, file.Length, file.FullName));

            listFilesCurrDir = null;

            var listDirCurrDir = directory.GetDirectories();

            // if there are subfolders (if the list is not empty)
            if (listDirCurrDir.Length != 0)
            {
                foreach (var dir in listDirCurrDir)
                {
                    var subFolder = new Folder(dir.Name, dir.FullName);
                    folder.Add(subFolder);

                    listAllFiles(subFolder, dir);
                }
            }

            listDirCurrDir = null;

        }
    }
}
