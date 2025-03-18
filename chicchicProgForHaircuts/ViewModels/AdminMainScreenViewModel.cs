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
using Avalonia.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;

namespace chicchicProgForHaircuts.ViewModels
{
    public class AdminMainScreenViewModel : ViewModelBase
    {
        private GoodhaircutContext _db;
        private ObservableCollection<Appointment> _appointments;
        private ObservableCollection<Client> _clients;
        private string _errorMessage;

        private List<Appointmentsstatus> availableStatuses;
        private Appointmentsstatus _appointmentsstatusEntity;

        public List<Appointmentsstatus> AvailableStatuses
        {
            get => availableStatuses;
            set => this.RaiseAndSetIfChanged(ref availableStatuses, value);
        }

        public Appointmentsstatus SelectedappointmentsstatusEntity
        {
            get => _appointmentsstatusEntity;
            set => this.RaiseAndSetIfChanged(ref _appointmentsstatusEntity, value);
        }
        
        /// <summary>
        /// ������ ������� �� �������.
        /// </summary>
        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set => this.RaiseAndSetIfChanged(ref _appointments, value);
        }
        
        private void LoadAppointmentsstatus()
        {
            try
            {
                availableStatuses = _db.Appointmentsstatuses.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ �������� �����: {ex.Message}");
            }
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
            LoadAppointmentsstatus();
            PrintAppointmentsCommand = ReactiveCommand.Create(PrintAppointments);

            //foreach (Appointment appointment in Appointments)
            //{
            //    appointment.AvailableStatuses = db.Appointmentsstatuses.ToList();
            //}

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
                .Include(x => x.Appointmentsstatus)  // ��������� ������ � ������� ������
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

        public void OnStatusChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedStatus = comboBox?.SelectedItem as Appointmentsstatus;

            if (selectedStatus != null)
            {
                var appointment = comboBox?.DataContext as Appointment;
                if (appointment != null)
                {
                    appointment.Appointmentsstatus = selectedStatus;
                    // ���� ����������, ��������� ���������
                    SaveAppointmentStatus(appointment);
                }
            }
        }

        public void SaveAppointmentStatus(Appointment appointment)
        {
            if (appointment == null || appointment.Appointmentsstatus == null) return;

            try
            {
                var appointmentToUpdate = _db.Appointments
                    .Include(a => a.Appointmentsstatus)
                    .FirstOrDefault(a => a.Id == appointment.Id);

                if (appointmentToUpdate != null)
                {
                    appointmentToUpdate.Appointmentsstatus = appointment.Appointmentsstatus; // ��������� ������
                    _db.SaveChanges(); // ��������� ��������� � ���� ������
                    ErrorMessage = "������ ������� ��������!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ ���������� �������: {ex.Message}");
                ErrorMessage = $"������ ��� ���������� �������: {ex.Message}";
            }
        }

        public async Task OpenStatusWindowCommand(int id)
        {
            var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;

            var statusWindow = new ChangesStatusWindow
            {
                DataContext = new ChangesStatusWindowViewModel(_db, id)
            };

            // ���������� ����
            await statusWindow.ShowDialog(mainWindow);
            MainWindowViewModel.Self.Us = new AdminMainScreen();

        }
    }
}
