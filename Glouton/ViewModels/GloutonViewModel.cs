using Glouton.Interfaces;
using System;

namespace Glouton.ViewModels;

public class GloutonViewModel : BaseViewModel
{
    private readonly IGlouton _glouton;

    public string Image => $"/Ressources/{ImageId}.bmp";
    public int ImageId { get; set; }

    public GloutonViewModel(IGlouton glouton)
    {
        _glouton = glouton;
        _glouton.HungerLevelChanged += OnHungerLevelChanged;
        _glouton.WakeUp();
        ImageId = 2;
    }

    private void OnHungerLevelChanged(object? sender, EventArgs e)
    {
        ImageId =  Math.Min(_glouton.HungerLevel / 20, 4);
        base.OnPropertyChanged(nameof(Image));
    }
}