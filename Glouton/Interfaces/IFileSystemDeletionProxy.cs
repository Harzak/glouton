using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Interfaces;

public interface IFileSystemDeletionProxy
{
    void Delete(string path);
    bool Exists(string path);
}