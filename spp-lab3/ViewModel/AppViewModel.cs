using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using directory_scanner.wpf.Model;
using System.Windows.Forms;
using Presentation.Command;
using directory_scanner.directory_scanner;

namespace directory_scanner.wpf.ViewModel
{

    public class AppViewModel : INotifyPropertyChanged
    {
        public RelayCommand SetDirectoryCommand { get; set; }
        public RelayCommand StartScanningCommand { get; set; }
        public RelayCommand StopScanningCommand { get; set; }

        public RelayCommand ShowLength { get; set; }

        private DirectoryScanner _directoryScanner;

        private string? _path;
        public string? Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }

        private ushort _maxThreadCount = 50;

        public ushort MaxThreadCount
        {
            get => _maxThreadCount;
            set
            {
                _maxThreadCount = value;
                OnPropertyChanged();
            }
        }

        private ModelFileTree _tree;

        public ModelFileTree Tree
        {
            get => _tree;
            set
            {
                _tree = value;
                OnPropertyChanged();
            }
        }

        private volatile bool _isScanning;
        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                _isScanning = value;
                OnPropertyChanged();
            }
        }

        private ulong _length  = 0;

        public ulong Length
        {
            get => _length;
            set
            {
                _length = value;
                OnPropertyChanged("Length");
            }
        }

        public AppViewModel()
        {
            SetDirectoryCommand = new RelayCommand(_ =>
            {
                using var folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    Path = folderBrowserDialog.SelectedPath;
                }
            });

            StartScanningCommand = new RelayCommand(_ =>
            {
                IsScanning = true;
                Task.Run(() =>
                {
                    _directoryScanner = new DirectoryScanner(MaxThreadCount);
                    _directoryScanner.Start(Path);
                    /*(for (int i = 0; i < 50; i++)
                    {
                        //System.Threading.Thread.Sleep(100);
                        Length = _directoryScanner.RootSize;
                        //MyValue = i.ToString();
                    }*/
                    
                    while (_directoryScanner._taskQueue._taskCount != _maxThreadCount && !_directoryScanner._tokenSource.Token.IsCancellationRequested)
                    {
                        
                        Length = _directoryScanner.RootSize;
                    }

                    var directoryTree = _directoryScanner.Finish();

                    Length = _directoryScanner.RootSize;
                    Tree = new ModelFileTree();
                    Tree.Children = new List<ModelFileTree>() { new(directoryTree) };
                    IsScanning = false;
                });
            }, _ => Path != null && !IsScanning);

            StopScanningCommand = new RelayCommand(_ =>
            {
                var directoryTree = _directoryScanner.Stop();
                IsScanning = false;
                Tree = new ModelFileTree()
                {
                    Children = new List<ModelFileTree>()
                    {
                    new(directoryTree)
                    }
                };
            }, _ => IsScanning);

           
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            /*
            if (IsScanning)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Length));
            }*/
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
        }
    }
}