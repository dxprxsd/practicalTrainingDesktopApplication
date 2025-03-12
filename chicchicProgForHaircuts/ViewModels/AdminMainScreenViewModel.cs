using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using chicchicProgForHaircuts.Models;
using ReactiveUI;
using Microsoft.EntityFrameworkCore;

namespace chicchicProgForHaircuts.ViewModels
{
    public class AdminMainScreenViewModel : ViewModelBase
    {
        private readonly GoodhaircutContext _db;
        private ObservableCollection<Appointment> _appointments;

        /// <summary>
        /// ������ ������� �� �������.
        /// </summary>
        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set => this.RaiseAndSetIfChanged(ref _appointments, value);
        }

        public AdminMainScreenViewModel(GoodhaircutContext db)
        {
            _db = db;
            LoadAppointments();
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
    }

}
