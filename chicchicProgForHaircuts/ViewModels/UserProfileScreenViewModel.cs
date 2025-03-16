using System;
using System.Linq;
using chicchicProgForHaircuts.Models;
using chicchicProgForHaircuts.Views;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace chicchicProgForHaircuts.ViewModels
{
    public class UserProfileScreenViewModel : ViewModelBase
    {
        private GoodhaircutContext _db;

        private Client _currentClient;

        private string _greeting;

        /// <summary>
        /// Полное имя клиента (ФИО).
        /// </summary>
        public string FullName => CurrentClient != null
            ? $"{CurrentClient.SurnameClient} {CurrentClient.NameClient} {CurrentClient.PatronymicClient}".Trim()
            : string.Empty;

        public Client CurrentClient
        {
            get => _currentClient;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentClient, value);
                this.RaisePropertyChanged(nameof(FullName));
            }
        }

        /// <summary>
        /// Приветствие для отображения на экране клиента.
        /// </summary>
        public string Greeting
        {
            get => _greeting;
            set => this.RaiseAndSetIfChanged(ref _greeting, value);
        }

        public UserProfileScreenViewModel(GoodhaircutContext db)
        {
            _db = db;
            LoadUserProfile();
            LoadClientData();
        }

        // Метод для загрузки профиля пользователя
        private void LoadUserProfile()
        {
            // Получаем ID текущего пользователя из MainWindowViewModel
            var clientId = MainWindowViewModel.Self.IdClient;

            // Получаем информацию о клиенте из базы данных по ID
            var clientFromDb = _db.Clients
                .Where(c => c.Id == clientId)
                .Include(c => c.GenderNavigation)  // Загружаем информацию о поле
                .Include(c => c.Status)  // Загружаем информацию о статусе
                .FirstOrDefault();

            if (clientFromDb != null)
            {
                // Присваиваем загруженные данные текущему клиенту
                CurrentClient = clientFromDb;
            }
        }

        /// <summary>
        /// Загружает данные клиента, включая приветствие и другие параметры.
        /// </summary>
        /// <param name="client">Информация о текущем клиенте.</param>
        private void LoadClientData()
        {
            // Получаем текущий час
            var currentHour = DateTime.Now.Hour;

            // Определяем время суток и корректируем "Доброе"/"Добрый"
            string greetingWord = currentHour < 11 ? "Доброе утро!" :
                                  currentHour < 18 ? "Добрый день!" :
                                  "Добрый вечер!";

            // Устанавливаем две строки приветствия
            Greeting = $"{greetingWord}".Trim();
        }

        /// <summary>
        /// Выход на главный экран.
        /// </summary>
        public void ExitToMainScreen() => MainWindowViewModel.Self.Us = new MainScreen();
    }
}
