using System;
using System.Reflection;
using System.Windows;

namespace Glouton.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SetMenuDirection();
    }

    private void SetMenuDirection()
    {
        FieldInfo? menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
        Action setAlignmentValue = () => {
            if (SystemParameters.MenuDropAlignment && menuDropAlignmentField != null) menuDropAlignmentField.SetValue(null, false);
        };
        setAlignmentValue();
        SystemParameters.StaticPropertyChanged += (sender, e) => { setAlignmentValue(); };
    }
}