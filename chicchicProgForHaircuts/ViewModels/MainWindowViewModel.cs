using Avalonia.Controls;
using chicchicProgForHaircuts.Models;
using chicchicProgForHaircuts.Views;
using HarfBuzzSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace chicchicProgForHaircuts.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly GoodhaircutContext _db;
        private UserControl _us;
        public static MainWindowViewModel Self;

        private Haircut _haircut;
        private ObservableCollection<Haircut> _haircuts;

        private Haircutsgender haircutsgenders;

        /// <summary>
        /// Контроллер текущего экрана.
        /// </summary>
        public UserControl Us
        {
            get => _us;
            set => this.RaiseAndSetIfChanged(ref _us, value);
        }

        /// <summary>
        /// Пол стрижки.
        /// </summary>
        public Haircutsgender Haircutsgenders
        {
            get => haircutsgenders;
            set => this.RaiseAndSetIfChanged(ref haircutsgenders, value);
        }

        /// <summary>
        /// Выбранная стрижка.
        /// </summary>
        public Haircut Haircut
        {
            get => _haircut;
            set => this.RaiseAndSetIfChanged(ref _haircut, value);
        }

        public ObservableCollection<Haircut> Haircuts
        {
            get => _haircuts;
            set => this.RaiseAndSetIfChanged(ref _haircuts, value);
        }

        public MainWindowViewModel(GoodhaircutContext db)
        {
            _db = db;
            Self = this;
            //Us = new LoginScreen();
            Us = new MainScreen();
            //Us = new AdminMainScreen();
            //Us = new RegistrationOnHairCut();

            // Загружаем стрижки и подгружаем данные о поле (GenderNavigation)
            Haircuts = new ObservableCollection<Haircut>(
                _db.Haircuts.Include(x => x.GenderNavigation).ToList()
            );
        }

        /// <summary>
        /// Переход на основной экран приложения.
        /// </summary>
        public void GoToRegOnHairCut() => Us = new RegistrationOnHairCut();

        /// <summary>
        /// Загружает стрижки из базы данных.
        /// </summary>
        private void LoadHaircuts()
        {
            var haircutsFromDb = _db.Haircuts
                                    .Include(x => x.GenderNavigation) // Загружаем пол
                                    .ToList();
            Haircuts = new ObservableCollection<Haircut>(haircutsFromDb);
        }

    }
}
