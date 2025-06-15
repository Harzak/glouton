using Glouton.Features.FileManagement.FileEvent;
using System;
using System.Collections.Generic;

namespace Glouton.Interfaces;

public interface IFileEventBatchProcessor : IDisposable
{
    void Initialize(Action<List<FileEventActionModel>> filesAction);
    void Enqueue(FileEventActionModel model);
}