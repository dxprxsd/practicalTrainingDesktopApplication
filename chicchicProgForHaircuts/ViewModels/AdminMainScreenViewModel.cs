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
        /// Список записей на стрижки.
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
        /// Загружает записи на стрижки из базы данных.
        /// </summary>
        private void LoadAppointments()
        {
            var appointmentsFromDb = _db.Appointments
                .Include(x => x.Client)    // Загружаем данные о клиенте
                .Include(x => x.Employee)  // Загружаем данные о сотруднике
                .Include(x => x.Haircut)  // Загружаем данные о стрижке
                .ToList();

            Appointments = new ObservableCollection<Appointment>(appointmentsFromDb);
        }
    }

}
