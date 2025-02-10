using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloom.Models
{
    public class FileItem
    {
        public string FileName { get; set; }
        public string FileSize { get; set; } // Display in KB
        public string LastModified { get; set; }
    }
}
