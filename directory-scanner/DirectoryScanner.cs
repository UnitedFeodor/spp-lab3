using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace directory_scanner
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace directory_scanner
    {
        public class DirectoryScanner
        {
            public FileTree _tree { get; set; }
            public CancellationTokenSource _tokenSource;
            public TaskQueue _taskQueue;
            private ushort _maxThreadCount;

            public ulong RootSize;

            public DirectoryScanner(ushort maxThreadCount)
            {
                if (maxThreadCount == 0)
                {
                    throw new Exception($"Illegal thread count: {maxThreadCount}");
                }

                _maxThreadCount = maxThreadCount;
                _tokenSource = new CancellationTokenSource();
                
            }

            public void Start(string path)
            {
                if (File.Exists(path))
                {
                    var fileInfo = new FileInfo(path);
                    _tree = new FileTree(false, fileInfo.Name, fileInfo.FullName, (double) 100, (ulong) fileInfo.Length);
                    return;
                }

                if (!Directory.Exists(path))
                {
                    throw new Exception($"No such dir: {path}");
                }

                var directoryInfo = new DirectoryInfo(path);
                _tree = new FileTree(true, directoryInfo.Name, directoryInfo.FullName,(double) 100);

                
                _taskQueue = new TaskQueue(_tokenSource, _maxThreadCount);

                _tree.Children = new ObservableCollection<FileTree>();
                _tree.Children.CollectionChanged += (e, v) => updateConditions();


                _taskQueue.EnqueueTask(() => ScanDirectory(_tree));
            }

            public void updateConditions()
            {
                lock (_tree)
                {
                                                                                                                    Thread.Sleep(10);
                    RootSize = _tree.GetLength();
                }
            }
            public FileTree Stop()
            {
                try
                {
                    _taskQueue.Cancel();
                } catch
                {

                }
                _tree.GetLength();
                _tree.GetLengthPercentage();
                return _tree;
            }

            public FileTree Finish()
            {
                //_taskQueue.WaitForEnd();
                
                _tree.GetLength();
                _tree.GetLengthPercentage();
                return _tree;
            }

            

            private void ScanDirectory(FileTree node)
            {
                var token = _tokenSource.Token;
                var directoryInfo = new DirectoryInfo(node.FullName);
                

                List<DirectoryInfo>? directories;
                try
                {
                    directories = directoryInfo.GetDirectories()
                        .Where(d => d.LinkTarget == null)
                        .ToList();
                }
                catch (Exception)
                {
                    directories = null;
                }

                List<FileInfo>? files;
                try
                {
                    files = directoryInfo.GetFiles()
                        .Where(f => f.LinkTarget == null)
                        .ToList();
                }
                catch (Exception)
                {
                    files = null;
                }

                if (directories != null)
                {
                    foreach (var directory in directories)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        var tree = new FileTree(true, directory.Name, directory.FullName);
                        tree.Children = new ObservableCollection<FileTree>();
                        tree.Children.CollectionChanged += (e, v) => updateConditions();
                        node.Children.Add(tree);
                        _taskQueue.EnqueueTask(() => ScanDirectory(tree));
                    }
                }

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (token.IsCancellationRequested)
                            return;
                        var tree = new FileTree(false, file.Name, file.FullName,(ulong) file.Length);
                        node.Children.Add(tree);
                    }
                }

                

            }
        }
    }

}
