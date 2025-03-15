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
        public Client CurrentClient
        {
            get => _currentClient;
            set => this.RaiseAndSetIfChanged(ref _currentClient, value);
        }

        public UserProfileScreenViewModel(GoodhaircutContext db)
        {
            _db = db;
            LoadUserProfile();
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
        /// Выход на главный экран.
        /// </summary>
        public void ExitToMainScreen() => MainWindowViewModel.Self.Us = new MainScreen();
    }
}
