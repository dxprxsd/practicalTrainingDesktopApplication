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
        /// ������ ��� ������� (���).
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
        /// ����������� ��� ����������� �� ������ �������.
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
        /// ��������� ������ �������, ������� ����������� � ������ ���������.
        /// </summary>
        /// <param name="client">���������� � ������� �������.</param>
        private void LoadClientData()
        {
            // �������� ������� ���
            var currentHour = DateTime.Now.Hour;

            // ���������� ����� ����� � ������������ "������"/"������"
            string greetingWord = currentHour < 11 ? "������ ����!" :
                                  currentHour < 18 ? "������ ����!" :
                                  "������ �����!";

            // ������������� ��� ������ �����������
            Greeting = $"{greetingWord}".Trim();
        }

        /// <summary>
        /// ����� �� ������� �����.
        /// </summary>
        public void ExitToMainScreen() => MainWindowViewModel.Self.Us = new MainScreen();
    }
}
