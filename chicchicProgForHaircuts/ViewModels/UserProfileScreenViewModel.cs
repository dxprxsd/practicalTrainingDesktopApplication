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

        // ����� ��� �������� ������� ������������
        private void LoadUserProfile()
        {
            // �������� ID �������� ������������ �� MainWindowViewModel
            var clientId = MainWindowViewModel.Self.IdClient;

            // �������� ���������� � ������� �� ���� ������ �� ID
            var clientFromDb = _db.Clients
                .Where(c => c.Id == clientId)
                .Include(c => c.GenderNavigation)  // ��������� ���������� � ����
                .Include(c => c.Status)  // ��������� ���������� � �������
                .FirstOrDefault();

            if (clientFromDb != null)
            {
                // ����������� ����������� ������ �������� �������
                CurrentClient = clientFromDb;
            }
        }

        /// <summary>
        /// ����� �� ������� �����.
        /// </summary>
        public void ExitToMainScreen() => MainWindowViewModel.Self.Us = new MainScreen();
    }
}
