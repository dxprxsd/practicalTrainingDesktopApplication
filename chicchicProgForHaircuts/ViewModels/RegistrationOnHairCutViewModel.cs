using System;
using System.Collections.Generic;
using chicchicProgForHaircuts.Models;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Linq;
using System.Reactive;

namespace chicchicProgForHaircuts.ViewModels
{
	public class RegistrationOnHairCutViewModel : ViewModelBase
	{
        private readonly GoodhaircutContext _db;

        public ObservableCollection<Employee> Employees { get; set; }
        public ObservableCollection<Haircut> Haircuts { get; set; }

        private Employee _selectedEmployee;
        private Haircut _selectedHaircut;
        private DateTime _appointmentDate;
        private decimal _finalPrice;

        public ReactiveCommand<Unit, Unit> BookAppointmentCommand { get; }

        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set => this.RaiseAndSetIfChanged(ref _selectedEmployee, value);
        }

        public Haircut SelectedHaircut
        {
            get => _selectedHaircut;
            set => this.RaiseAndSetIfChanged(ref _selectedHaircut, value);
        }

        public DateTime AppointmentDate
        {
            get => _appointmentDate;
            set => this.RaiseAndSetIfChanged(ref _appointmentDate, value);
        }

        public decimal FinalPrice
        {
            get => _finalPrice;
            set => this.RaiseAndSetIfChanged(ref _finalPrice, value);
        }

        public RegistrationOnHairCutViewModel(GoodhaircutContext db)
        {
            _db = db;
            LoadEmployeesAndHaircuts();
            AppointmentDate = DateTime.Now;

            // Initialize the BookAppointmentCommand
            BookAppointmentCommand = ReactiveCommand.Create(BookAppointment);
        }

        // Load employees with "Парикмахер" role and list of haircuts
        private void LoadEmployeesAndHaircuts()
        {
            Employees = new ObservableCollection<Employee>(_db.Employees.Where(x => x.RoleId == 1).ToList());
            Haircuts = new ObservableCollection<Haircut>(_db.Haircuts.ToList());
        }

        // Book an appointment
        public void BookAppointment()
        {
            if (SelectedEmployee == null || SelectedHaircut == null || FinalPrice <= 0)
            {
                // Handle invalid form submission
                return;
            }

            var appointment = new Appointment
            {
                ClientId = 1, // Replace with the actual client ID from your authentication logic
                EmployeeId = SelectedEmployee.Id,
                HaircutId = SelectedHaircut.Id,
                AppointmentDate = AppointmentDate,
                FinalPrice = FinalPrice
            };

            _db.Appointments.Add(appointment);
            _db.SaveChanges();
        }
    }
}