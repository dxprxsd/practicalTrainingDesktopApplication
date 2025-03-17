using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using chicchicProgForHaircuts.Models;
using ReactiveUI;
using Microsoft.EntityFrameworkCore;
using chicchicProgForHaircuts.Views;
using System.IO;
using System.Reactive;
using System.Diagnostics.Metrics;

namespace chicchicProgForHaircuts.ViewModels
{
    public class AdminMainScreenViewModel : ViewModelBase
    {
        private GoodhaircutContext _db;
        private ObservableCollection<Appointment> _appointments;
        private ObservableCollection<Client> _clients;
        private string _errorMessage;

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

        /// <summary>
        /// ��������� �� ���������� ������ ��� ������.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        // ������� ��� ������ �������
        public ReactiveCommand<Unit, Unit> PrintAppointmentsCommand { get; }

        public AdminMainScreenViewModel(GoodhaircutContext db)
        {
            _db = db;
            LoadAppointments();
            LoadClients();
            PrintAppointmentsCommand = ReactiveCommand.Create(PrintAppointments);
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


        /// <summary>
        /// ����� ��� ��������� ������ ��� ������
        /// </summary>
        private void PrintAppointments()
        {
            // ���� � �����, � ������� ����� �������� ������
            string filePath = "appointments.txt";

            // ������������ ������������ ������ �������
            var popularHaircut = Appointments
                .GroupBy(a => a.Haircut.Name)
                .OrderByDescending(g => g.Count()) // ��������� �� ���������� �������
                .FirstOrDefault(); // ����� ����� ����������

            string popularHaircutName = popularHaircut?.Key ?? "����������"; // ���� ��� ���������� �������, ������� "����������"
            int popularHaircutCount = popularHaircut?.Count() ?? 0;

            // ��������� StreamWriter ��� ������ � ����
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("��� ������� | ��� ����������� | �������� ������� | ���� ������� | �������� ����");

                foreach (var appointment in Appointments)
                {
                    writer.WriteLine($"{appointment.Client.NameClient} | {appointment.Employee.NameEmployee} | {appointment.Haircut.Name} | {appointment.AppointmentDate} | {appointment.FinalPrice}");
                }

                // �������� ����� ���������� ��������
                writer.WriteLine();
                writer.WriteLine($"����� ���������� �������: {popularHaircutName}");
                writer.WriteLine($"���������� �������: {popularHaircutCount}");
            }

            // �� ������ �������� ����������� ������������, ��� ���� ��� ������
            Console.WriteLine("Data has been written to appointments.txt");
            ErrorMessage = $"����� ������ � ���� {filePath}";
        }
    }
}
