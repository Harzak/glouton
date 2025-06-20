using Glouton.Interfaces;
using System.Diagnostics;

namespace Glouton.Wrappers;

public class ProcessFacade : IProcessFacade
{
    public Process? Start(ProcessStartInfo startInfo) => Process.Start(startInfo);
}