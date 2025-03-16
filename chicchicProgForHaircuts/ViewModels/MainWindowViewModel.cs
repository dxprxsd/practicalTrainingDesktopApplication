using Avalonia.Controls;
using chicchicProgForHaircuts.Models;
using chicchicProgForHaircuts.Views;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace chicchicProgForHaircuts.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly GoodhaircutContext _db;
        private UserControl _us;
        public static MainWindowViewModel Self;
        private int _idClient;

        private Haircut _haircut;
        private ObservableCollection<Haircut> _haircuts;

        private Haircutsgender _selectedHaircutGender;
        private List<Haircutsgender> _haircutGenders;

        /// <summary>
        /// Контроллер текущего экрана.
        /// </summary>
        public UserControl Us
        {
            get => _us;
            set => this.RaiseAndSetIfChanged(ref _us, value);
        }

        /// <summary>
        /// Список доступных полов для стрижек.
        /// </summary>
        public List<Haircutsgender> HaircutGenders
        {
            get => _haircutGenders;
            set => this.RaiseAndSetIfChanged(ref _haircutGenders, value);
        }

        /// <summary>
        /// Выбранный пол для фильтрации стрижек.
        /// </summary>
        public Haircutsgender SelectedHaircutGender
        {
            get => _selectedHaircutGender;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedHaircutGender, value);
                FilterHaircuts();
            }
        }

        /// <summary>
        /// ID клиента.
        /// </summary>
        public int IdClient
        {
            get => _idClient;
            set => this.RaiseAndSetIfChanged(ref _idClient, value);
        }

        /// <summary>
        /// Выбранная стрижка.
        /// </summary>
        public Haircut Haircut
        {
            get => _haircut;
            set => this.RaiseAndSetIfChanged(ref _haircut, value);
        }

        /// <summary>
        /// Список стрижек.
        /// </summary>
        public ObservableCollection<Haircut> Haircuts
        {
            get => _haircuts;
            set => this.RaiseAndSetIfChanged(ref _haircuts, value);
        }

        public MainWindowViewModel(GoodhaircutContext db)
        {
            _db = db;
            Self = this;
            Us = new LoginScreen() { DataContext = new LoginScreenViewModel(_db) };
            LoadData();
        }

        /// <summary>
        /// Загружает данные стрижек и полов из базы данных.
        /// </summary>
        private void LoadData()
        {
            // Добавляем "Все виды" в список полов
            var allGenders = new Haircutsgender { Id = 0, HairgenderName = "Все виды" };
            HaircutGenders = new List<Haircutsgender> { allGenders };
            HaircutGenders.AddRange(_db.Haircutsgenders.ToList());

            Haircuts = new ObservableCollection<Haircut>(_db.Haircuts.Include(x => x.GenderNavigation).ToList());

            // По умолчанию выбираем "Все виды"
            SelectedHaircutGender = allGenders;
        }

        /// <summary>
        /// Фильтрует стрижки по выбранному полу.
        /// </summary>
        private void FilterHaircuts()
        {
            if (SelectedHaircutGender != null && SelectedHaircutGender.Id != 0)
            {
                Haircuts = new ObservableCollection<Haircut>(_db.Haircuts
                    .Include(x => x.GenderNavigation)
                    .Where(x => x.Gender == SelectedHaircutGender.Id)
                    .ToList());
            }
            else
            {
                Haircuts = new ObservableCollection<Haircut>(_db.Haircuts.Include(x => x.GenderNavigation).ToList());
            }
        }

        /// <summary>
        /// Переход на основной экран приложения.
        /// </summary>
        public void GoToRegOnHairCut() => Us = new RegistrationOnHairCut() { DataContext = new RegistrationOnHairCutViewModel(_db) };

        public void GoToLogin() => Us = new LoginScreen() { DataContext = new LoginScreenViewModel(_db) };

        public void GoToProfile() => Us = new UserProfileScreen() { DataContext = new UserProfileScreenViewModel(_db) };
    }
}
