using System;
using System.Collections.Generic;
using System.Linq;
using chicchicProgForHaircuts.Models;
using ReactiveUI;

namespace chicchicProgForHaircuts.ViewModels
{
	public class ChangesStatusWindowViewModel : ViewModelBase
	{
        private GoodhaircutContext _db;
        int idAppoinment;

        private string _errorMessage;

        private List<Appointmentsstatus> availableStatuses;
        private Appointmentsstatus _appointmentsstatusEntity;

        /// <summary>
        /// Сообщение об ошибке или успешной аутентификации.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

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

        private void LoadAppointmentsstatus()
        {
            try
            {
                availableStatuses = _db.Appointmentsstatuses.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки полов: {ex.Message}");
            }
        }

        public ChangesStatusWindowViewModel(GoodhaircutContext db, int id)
        {
            _db = db;
            LoadAppointmentsstatus();
            idAppoinment = id;
        }

        public void ConfirmStatus()
        {
            //if (SelectedappointmentsstatusEntity != null)
            //{
                // Update the appointment status in the database (or the source)
                var appointmentToUpdate = _db.Appointments.FirstOrDefault(a => a.Id == idAppoinment);
                if (appointmentToUpdate != null)
                {
                    appointmentToUpdate.Appointmentsstatus = SelectedappointmentsstatusEntity;
                    _db.SaveChanges();
                }

                // Close the window (if applicable)
                // For example, you can notify the parent window to close this one
                ErrorMessage = $"Статус изменен на {SelectedappointmentsstatusEntity.StatusapName}";
            //}
        }

        public void ConfirmStatus2()
        {

        }
    }
}
