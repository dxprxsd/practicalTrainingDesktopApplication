using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using chicchicProgForHaircuts.Models;
using ReactiveUI;
using Microsoft.EntityFrameworkCore;
using chicchicProgForHaircuts.Views;

namespace chicchicProgForHaircuts.ViewModels
{
    public class AdminMainScreenViewModel : ViewModelBase
    {
        private GoodhaircutContext _db;
        private ObservableCollection<Appointment> _appointments;
        private ObservableCollection<Client> _clients;

        /// <summary>
        /// ������ ������� �� �������.
        /// </summary>
        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set => this.RaiseAndSetIfChanged(ref _appointments, value);
        }

        /// <summary>
        /// ������ ��������.
        /// </summary>
        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set => this.RaiseAndSetIfChanged(ref _clients, value);
        }

        public AdminMainScreenViewModel(GoodhaircutContext db)
        {
            _db = db;
            LoadAppointments();
            LoadClients();
        }

        /// <summary>
        /// ��������� ������ �� ������� �� ���� ������.
        /// </summary>
        private void LoadAppointments()
        {
            var appointmentsFromDb = _db.Appointments
                .Include(x => x.Client)    // ��������� ������ � �������
                .Include(x => x.Employee)  // ��������� ������ � ����������
                .Include(x => x.Haircut)  // ��������� ������ � �������
                .ToList();

            Appointments = new ObservableCollection<Appointment>(appointmentsFromDb);
        }

        public void GoToLoginScreen() => MainWindowViewModel.Self.Us = new LoginScreen() { DataContext = new LoginScreenViewModel(_db) };

        /// <summary>
        /// ������� �� �������� ����� ����������.
        /// </summary>
        public void GoToRegClientScreen()
        {
            var regClientViewModel = new ClientRegistrationScreenViewModel(_db);
            MainWindowViewModel.Self.Us = new ClientRegistrationScreen() { DataContext = regClientViewModel };
        }


        /// <summary>
        /// ��������� ������ � �������� �� ���� ������.
        /// </summary>
        private void LoadClients()
        {
            var clientsFromDb = _db.Clients
                .Include(x => x.GenderNavigation) // ���������� ������������� ��������
                .Include(x => x.Status)  // ��������� ������ �������
                .ToList();

            Clients = new ObservableCollection<Client>(clientsFromDb);
        }

    }
}
