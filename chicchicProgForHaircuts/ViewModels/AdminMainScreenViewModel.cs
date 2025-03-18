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
        /// Список записей на стрижки.
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
                Console.WriteLine($"Ошибка загрузки полов: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Список клиентов.
        /// </summary>
        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set => this.RaiseAndSetIfChanged(ref _clients, value);
        }

        /// <summary>
        /// Сообщение об сохранении данных для отчета.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        

        // Команда для печати записей
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
        /// Загружает записи на стрижки из базы данных.
        /// </summary>
        private void LoadAppointments()
        {
            var appointmentsFromDb = _db.Appointments
                .Include(x => x.Client)    // Загружаем данные о клиенте
                .Include(x => x.Employee)  // Загружаем данные о сотруднике
                .Include(x => x.Haircut)  // Загружаем данные о стрижке
                .Include(x => x.Appointmentsstatus)  // Загружаем данные о статусе заказа
                .ToList();

            Appointments = new ObservableCollection<Appointment>(appointmentsFromDb);
        }

        public void GoToLoginScreen() => MainWindowViewModel.Self.Us = new LoginScreen() { DataContext = new LoginScreenViewModel(_db) };

        /// <summary>
        /// Переход на основной экран приложения.
        /// </summary>
        public void GoToRegClientScreen()
        {
            var regClientViewModel = new ClientRegistrationScreenViewModel(_db);
            MainWindowViewModel.Self.Us = new ClientRegistrationScreen() { DataContext = regClientViewModel };
        }

        /// <summary>
        /// Загружает записи о клиентах из базы данных.
        /// </summary>
        private void LoadClients()
        {
            var clientsFromDb = _db.Clients
                .Include(x => x.GenderNavigation) // Правильное навигационное свойство
                .Include(x => x.Status)  // Загружаем статус клиента
                .ToList();

            Clients = new ObservableCollection<Client>(clientsFromDb);
        }

        /// <summary>
        /// Метод для выведения отчета для печати
        /// </summary>
        private void PrintAppointments()
        {
            // Путь к файлу, в который будут записаны данные
            string filePath = "appointments.txt";

            // Подсчитываем популярность каждой стрижки
            var popularHaircut = Appointments
                .GroupBy(a => a.Haircut.Name)
                .OrderByDescending(g => g.Count()) // Сортируем по количеству записей
                .FirstOrDefault(); // Берем самую популярную

            string popularHaircutName = popularHaircut?.Key ?? "Неизвестно"; // Если нет популярных стрижек, выводим "Неизвестно"
            int popularHaircutCount = popularHaircut?.Count() ?? 0;

            // Открываем StreamWriter для записи в файл
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Имя клиента | Имя парикмахера | Название стрижки | Дата стрижки | Итоговая цена");

                foreach (var appointment in Appointments)
                {
                    writer.WriteLine($"{appointment.Client.NameClient} | {appointment.Employee.NameEmployee} | {appointment.Haircut.Name} | {appointment.AppointmentDate} | {appointment.FinalPrice}");
                }

                // Печатаем самую популярную прическу
                writer.WriteLine();
                writer.WriteLine($"Самая популярная стрижка: {popularHaircutName}");
                writer.WriteLine($"Количество заказов: {popularHaircutCount}");
            }

            // Вы можете добавить уведомление пользователю, что файл был создан
            Console.WriteLine("Data has been written to appointments.txt");
            ErrorMessage = $"Отчет собран в файл {filePath}";
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
                    // Если необходимо, сохранить изменения
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
                    appointmentToUpdate.Appointmentsstatus = appointment.Appointmentsstatus; // Обновляем статус
                    _db.SaveChanges(); // Сохраняем изменения в базе данных
                    ErrorMessage = "Статус успешно обновлен!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения статуса: {ex.Message}");
                ErrorMessage = $"Ошибка при обновлении статуса: {ex.Message}";
            }
        }

        public async Task OpenStatusWindowCommand(int id)
        {
            var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;

            var statusWindow = new ChangesStatusWindow
            {
                DataContext = new ChangesStatusWindowViewModel(_db, id)
            };

            // Отобразить окно
            await statusWindow.ShowDialog(mainWindow);
            MainWindowViewModel.Self.Us = new AdminMainScreen();

        }
    }
}
