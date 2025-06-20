using Glouton.EventArgs;
using Glouton.Interfaces;
using System;

namespace Glouton.ViewModels;

public class GloutonViewModel : BaseViewModel
{
    private readonly IGlouton _glouton;

    public const int DefaultImageId = 2;
    public string Image => $"/Ressources/{ImageId}.bmp";
    public int ImageId { get; set; }

    public GloutonViewModel(IGlouton glouton)
    {
        _glouton = glouton;
        _glouton.HungerLevelChanged += OnHungerLevelChanged;
        _glouton.WakeUp();
        ImageId = DefaultImageId;
    }

    private void OnHungerLevelChanged(object? sender, HungerLevelEventArgs e)
    {
        ImageId =  Math.Clamp(e.HungerLevel / 20, 0, 4);
        base.OnPropertyChanged(nameof(Image));
    }
}