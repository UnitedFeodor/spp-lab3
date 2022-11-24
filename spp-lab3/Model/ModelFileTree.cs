using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace directory_scanner.wpf.Model
{
    public class ModelFileTree
    {
        public bool IsDirectory { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public ulong Length { get; set; } //file size
        public double LengthPercentage { get; set; }
        public List<ModelFileTree> Children { get; set; }
        public string Icon { get; set; }

        public ModelFileTree()
        {

        }

        public ModelFileTree(FileTree tree)
        {
            Name = tree.Name;
            Length = tree.Length;
            LengthPercentage = tree.LengthPercentage;
            Icon = tree.IsDirectory ? "Icons/folder.png" : "Icons/file.png";
            
            Children = tree.Children?.Select(c => new ModelFileTree(c)).ToList();
        }
       
        

    }
}
