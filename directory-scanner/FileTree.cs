using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace directory_scanner
{
    public class FileTree
    {
        public bool IsDirectory { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public ulong Length { get; set; } //file size
        public double LengthPercentage { get; set; }

        public ObservableCollection<FileTree> Children { get; set; }

        public FileTree(bool isDir, string name,string fullName, double lengthPercentage, ulong length)
        {
            IsDirectory = isDir;
            Name = name;
            FullName = fullName;
            Length = length;
            LengthPercentage = lengthPercentage;
            
        }

        public FileTree(bool isDir, string name, string fullName)
        {
            IsDirectory = isDir;
            Name = name;
            FullName = fullName;

        }

        public FileTree(bool isDir, string name, string fullName, double lengthPercentage)
        {
            IsDirectory = isDir;
            Name = name;
            FullName = fullName;
            LengthPercentage = lengthPercentage;

        }

        public FileTree(bool isDir, string name, string fullName, ulong length)
        {
            IsDirectory = isDir;
            Name = name;
            FullName = fullName;
            Length = length;

        }
        public ulong GetLength()
        {
            lock (this)
            {
                if (IsDirectory)
                {
                    if (Children.Count != 0)
                    {
                        Length = 0;
                        foreach (var child in Children)
                        {
                            Length += child.GetLength();
                        }
                    }
                    else
                    {
                        Length = 0;
                    }
                }
            }
            return Length;
        }

        public double GetLengthPercentage()
        {
            if (!IsDirectory) return LengthPercentage;
            foreach (var child in Children)
            {
                
                child.LengthPercentage = (double)child.Length / Length * 100;
                

                child.GetLengthPercentage();
            }
            return LengthPercentage;
        }

    }
}
