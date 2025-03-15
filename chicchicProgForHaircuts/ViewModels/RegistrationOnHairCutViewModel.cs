using System;
using System.Collections.Generic;
using chicchicProgForHaircuts.Models;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Linq;
using System.Reactive;
using chicchicProgForHaircuts.Views;
using HarfBuzzSharp;
using System.Net.Sockets;

namespace chicchicProgForHaircuts.ViewModels
{
    public class RegistrationOnHairCutViewModel : ViewModelBase
    {
        private readonly GoodhaircutContext _db;

        //public ObservableCollection<Employee> Employees { get; set; }
        public ObservableCollection<Haircut> Haircuts { get; set; }

        private List<Employee> _employees;
        private Employee _selectedEmployee;
        private Haircut _selectedHaircut;
        private DateTime _appointmentDate;
        private double? _finalPrice = null;
        int idClient;
        double k;

        public ReactiveCommand<Unit, Unit> BookAppointmentCommand { get; }

        public List<Employee> Employeis { get => _employees; set => this.RaiseAndSetIfChanged(ref _employees, value); }

        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set => this.RaiseAndSetIfChanged(ref _selectedEmployee, value);
        }

        public Haircut SelectedHaircut
        {
            get => _selectedHaircut;
            set { this.RaiseAndSetIfChanged(ref _selectedHaircut, value); FinalPrice = SelectedHaircut.Price; }
        }

        public DateTimeOffset AppointmentDate
        {
            get => new DateTimeOffset(_appointmentDate, TimeSpan.Zero);
            set => this.RaiseAndSetIfChanged(ref _appointmentDate, new DateTime(value.Year, value.Month, value.Day));
        }

        public double? FinalPrice
        {
            get => _finalPrice*k;
            set => this.RaiseAndSetIfChanged(ref _finalPrice, value);
        }

        public RegistrationOnHairCutViewModel(GoodhaircutContext db)
        {
            _db = db;
            LoadEmployeesAndHaircuts();
            AppointmentDate = DateTime.Now;
            LoadEmployees();

            //this.idClient = idClient;
            var client = _db.Clients.FirstOrDefault(c => c.Id == MainWindowViewModel.Self.IdClient);
            if (client.VisitCount >= 5)
            {
                k = 0.97;
            }
            else k = 1;

            // Initialize the BookAppointmentCommand
            BookAppointmentCommand = ReactiveCommand.Create(BookAppointment);
        }

        // Load employees with "����������" role and list of haircuts
        private void LoadEmployeesAndHaircuts()
        {
            Employeis = new List<Employee>(_db.Employees.Where(x => x.RoleId == 1).ToList());
            Haircuts = new ObservableCollection<Haircut>(_db.Haircuts.ToList());
        }

        public void MethodForTakePrice()
        {

        }

        /// <summary>
        /// ��������� ������ � ������������ �� ���� ������.
        /// </summary>
        private void LoadEmployees()
        {
            try
            {
                _employees = _db.Employees.Where(x => x.RoleId == 1).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ �������� ������������: {ex.Message}");
            }
        }

        /// <summary>
        /// ��������� ������ � ������������ �� ���� ������.
        /// </summary>
        private void LoadHaurcuts()
        {
            try
            {
                _employees = _db.Employees.Where(x => x.RoleId == 1).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ �������� ������������: {ex.Message}");
            }
        }

        // Book an appointment
        public void BookAppointment()
        {
            if (SelectedEmployee == null || SelectedHaircut == null || FinalPrice <= 0)
            {
                // Handle invalid form submission
                return;
            }

            // �������� �������� �������
            var client = _db.Clients.FirstOrDefault(c => c.Id == MainWindowViewModel.Self.IdClient);
            if (client == null)
            {
                Console.WriteLine("������ �� ������.");
                return;
            }

            // ����������� ������� ���������
            client.VisitCount = (client.VisitCount ?? 0) + 1;

            // ���������, ���� ���������� ��������� >= 5, ��������� ������
            if (client.VisitCount >= 5 && client.StatusId == 1)
            {
                client.StatusId = 2; // ��������� ������
            }


            var appointment = new Appointment
            {
                ClientId = MainWindowViewModel.Self.IdClient,
                EmployeeId = SelectedEmployee.Id,
                HaircutId = SelectedHaircut.Id,
                AppointmentDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                FinalPrice = FinalPrice
            };

            _db.Appointments.Add(appointment);
            _db.SaveChanges();

            // ��������� ����������� VisitCount �������
            _db.Clients.Update(client);
            _db.SaveChanges();

            ExitToMainScreen();
        }


        /// <summary>
        /// ����� �� ������� �����.
        /// </summary>
        public void ExitToMainScreen() => MainWindowViewModel.Self.Us = new MainScreen();


    }
}