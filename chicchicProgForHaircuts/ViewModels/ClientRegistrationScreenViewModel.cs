using System;
using System.Collections.Generic;
using chicchicProgForHaircuts.Models;
using chicchicProgForHaircuts.Views;
using ReactiveUI;
using System.IO;
using System.Linq;
using System.Numerics;
using Tmds.DBus.Protocol;

namespace chicchicProgForHaircuts.ViewModels
{
    public class ClientRegistrationScreenViewModel : ViewModelBase
    {
        private GoodhaircutContext _db;
        string _firstName;
        string _secondName;
        string _patronymic;
        private List<Gender> _genders;
        private Gender _selectedGenderEntity;
        private string _message;
        private string _password;
        private string _phone;
        public string Message { get => _message; set => this.RaiseAndSetIfChanged(ref _message, value); }
        public string Phone
        {
            get => _phone; set
            {
                if (value != null)
                {
                    this.RaiseAndSetIfChanged(ref _phone, value);
                }
            }
        }
        public string FirstName { get => _firstName; set => this.RaiseAndSetIfChanged(ref _firstName, value); }
        public string SecondName { get => _secondName; set => this.RaiseAndSetIfChanged(ref _secondName, value); }
        public string Patronymic { get => _patronymic; set => this.RaiseAndSetIfChanged(ref _patronymic, value); }
        public List<Gender> Genders { get => _genders; set => this.RaiseAndSetIfChanged(ref _genders, value); }
        public string Password { get => _password; set => this.RaiseAndSetIfChanged(ref _password, value); }
        public Gender SelectedGenderEntity { get => _selectedGenderEntity; set => this.RaiseAndSetIfChanged(ref _selectedGenderEntity, value); }


        public ClientRegistrationScreenViewModel(GoodhaircutContext _db)
        {
            this._db = _db;
            LoadGenders();
        }

        private void LoadGenders()
        {
            try
            {
                _genders = _db.Genders.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ �������� �����: {ex.Message}");
            }
        }

        /// <summary>
        /// ����������� ������������ � �������.
        /// </summary>
        public void Register()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(SecondName) ||
                string.IsNullOrWhiteSpace(Patronymic) || string.IsNullOrWhiteSpace(Phone) ||
                string.IsNullOrWhiteSpace(Password))

            {
                Message = "��� ������������ ���� ������ ���� ���������.";
                return;
            }

            if (!ValidatePassword(Password))
            {
                Message = "������ ������� �������";
                return;
            }

            try
            {
                Client newClient = new Client
                {
                    SurnameClient = SecondName,
                    NameClient = FirstName,
                    PatronymicClient = Patronymic,
                    Gender = SelectedGenderEntity.Id,
                    PhoneNumber = Phone,
                    VisitCount = 0,
                    StatusId = 1,
                    Password = Password
                };
                _db.Clients.Add(newClient);

                _db.SaveChanges();
                Message = "����������� ������ �������!";
                MainWindowViewModel.Self.Us = new AdminMainScreen();
            }
            catch (Exception ex)
            {
                Message = $"������ �����������: {ex.Message}";
            }
        }

        /// <summary>
        /// ���������� ������ �� ���������� ���������.
        /// </summary>
        /// <param name="password">������ ��� ��������.</param>
        /// <returns>��������� ��������� ������.</returns>
        private bool ValidatePassword(string password)
        {
            return password.Length >= 6 &&
                   password.IndexOfAny("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray()) != -1 &&
                   password.IndexOfAny("abcdefghijklmnopqrstuvwxyz".ToCharArray()) != -1 &&
                   password.IndexOfAny("0123456789".ToCharArray()) != -1 &&
                   password.IndexOfAny("!@#$%^&*()_-+=<>?/|".ToCharArray()) != -1;
        }

        /// <summary>
        /// ����� �� ������� �����.
        /// </summary>
        public void ExitToMainScreen() => MainWindowViewModel.Self.Us = new AdminMainScreen();
    }
}