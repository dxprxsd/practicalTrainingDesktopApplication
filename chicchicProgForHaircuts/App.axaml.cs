using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using chicchicProgForHaircuts.Models;
using chicchicProgForHaircuts.ViewModels;
using chicchicProgForHaircuts.Views;

namespace chicchicProgForHaircuts
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var dbContext = new GoodhaircutContext(); // Создаем экземпляр БД
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(dbContext),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}