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
        /// Список записей на стрижки.
        /// </summary>
        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set => this.RaiseAndSetIfChanged(ref _appointments, value);
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
            PrintAppointmentsCommand = ReactiveCommand.Create(PrintAppointments);
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
    }
}
