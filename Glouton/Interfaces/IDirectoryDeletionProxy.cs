using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Interfaces;

public interface IDirectoryDeletionProxy : IFileSystemDeletionProxy
{
    string[] GetDirectories(string path);
    string[] GetFiles(string path);
}

