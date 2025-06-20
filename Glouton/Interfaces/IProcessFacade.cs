using System.Diagnostics;

namespace Glouton.Interfaces;

public interface IProcessFacade
{
    Process? Start(ProcessStartInfo startInfo);
}
