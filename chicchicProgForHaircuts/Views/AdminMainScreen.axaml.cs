using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using chicchicProgForHaircuts.Models;
using chicchicProgForHaircuts.ViewModels;

namespace chicchicProgForHaircuts.Views;

public partial class AdminMainScreen : UserControl
{
    public AdminMainScreen()
    {
        InitializeComponent();
        this.DataContext = new AdminMainScreenViewModel(new GoodhaircutContext());
    }
}