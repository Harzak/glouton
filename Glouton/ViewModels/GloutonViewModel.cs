using Glouton.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.ViewModels;

public class GloutonViewModel : BaseViewModel
{
    private readonly IGlouton _glouton; 

    public GloutonViewModel(IGlouton glouton)
    {
        _glouton = glouton;
        _glouton.WakeUp();
    }
}