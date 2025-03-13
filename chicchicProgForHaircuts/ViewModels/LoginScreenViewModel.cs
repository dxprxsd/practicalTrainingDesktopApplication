using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using chicchicProgForHaircuts.Models;
using chicchicProgForHaircuts.Views;
using ReactiveUI;

namespace chicchicProgForHaircuts.ViewModels
{
	public class LoginScreenViewModel : ViewModelBase
	{
        private GoodhaircutContext _db;
        private string _idClient;
        private string _password;
        private string _errorMessage;
        Canvas captchaImage;
        string �aptchaText;
        string capchaCheck;
        int counter = 3;
        bool isEnableButton = true;
        Client client;

        public Canvas CaptchaImage { get => captchaImage; set => this.RaiseAndSetIfChanged(ref captchaImage, value); }
        public string CaptchaText { get => �aptchaText; set => this.RaiseAndSetIfChanged(ref �aptchaText, value); }
        public string CapchaCheck { get => capchaCheck; set => this.RaiseAndSetIfChanged(ref capchaCheck, value); }
        public int Counter { get => counter; set => this.RaiseAndSetIfChanged(ref counter, value); }
        public bool IsEnableButton { get => isEnableButton; set => this.RaiseAndSetIfChanged(ref isEnableButton, value); }

        /// <summary>
        /// ������� ��� ���������� ��������������.
        /// </summary>
        public ReactiveCommand<Unit, Unit> LoginCommand { get; }

        // �������� ������ �� ���� ������ MainWindowViewModel
        private readonly MainWindowViewModel _mainWindowViewModel;

        /// <summary>
        /// ������������� ������� ��� �����.
        /// </summary>
        public string IdClient
        {
            get => _idClient;
            set => this.RaiseAndSetIfChanged(ref _idClient, value);
        }

        /// <summary>
        /// ������ ��� �����.
        /// </summary>
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        /// <summary>
        /// ��������� �� ������ ��� �������� ��������������.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        /// <summary>
        /// ����������� ��� ������������� ViewModel ��� ������ �����.
        /// </summary>
        /// <param name="db">�������� ���� ������.</param>
        /// <param name="mainWindowViewModel">ViewModel �������� ����.</param>
        /// , MainWindowViewModel mainWindowViewModel
        public LoginScreenViewModel(GoodhaircutContext db) 
        {
            _db = db;
            //_mainWindowViewModel = mainWindowViewModel ?? throw new ArgumentNullException(nameof(mainWindowViewModel));  // �������� �� null
            LoginCommand = ReactiveCommand.Create(Authenticate);
            GenerateCaptcha();
        }

        /// <summary>
        /// ����� ��� �������������� ������������.
        /// ��������� ��������� ������ � �������������� �� ��������������� �����.
        /// </summary>
        private async void Authenticate()
        {
            try
            {
                if (Counter == 0)
                {
                    await BlockLoginButtonAsync();
                    return;
                }

                if (CaptchaText != CapchaCheck)
                {
                    Counter--;
                    ErrorMessage = $"������� ������� �����, �������� �������: {Counter}";
                    GenerateCaptcha();
                    CapchaCheck = "";

                    if (Counter == 0)
                        await BlockLoginButtonAsync();
                    return;
                }

                if (!int.TryParse(IdClient, out int userId))
                {
                    Counter--;
                    ErrorMessage = $"������������ ������ ID ������������, �������� �������: {Counter}";

                    if (Counter == 0)
                        await BlockLoginButtonAsync();
                    return;
                }

                // Check for client in Clients table
                client = _db.Clients.FirstOrDefault(u => u.Id == userId && u.Password == Password);
                // Check for employee in Employee table
                var employee = _db.Employees.FirstOrDefault(u => u.Id == userId && u.Password == Password);

                if (client != null)
                {
                    // Client found, navigate to MainScreen
                    ErrorMessage = "�������� ���� ��� ������!";
                    GoToMainScreen();
                }
                else if (employee != null)
                {
                    // Employee found, navigate to AdminMainScreen
                    ErrorMessage = "�������� ���� ��� ���������!";
                    GoToAdminMainScreen();
                }
                else
                {
                    Counter--;
                    ErrorMessage = $"�������� ������, �������� �������: {Counter}";

                    if (Counter == 0)
                        await BlockLoginButtonAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "��������� ������: " + ex.Message;
            }
        }


        /// <summary>
        /// ��������� ������ ����� �� 10 ������
        /// </summary>
        private async Task BlockLoginButtonAsync()
        {
            IsEnableButton = false;
            for (int i = 10; i >= 0; i--)
            {
                ErrorMessage = $"�� �������������, ��������� �����: {i} ������";
                await Task.Delay(1000);
            }
            ErrorMessage = "";
            Counter = 3;
            IsEnableButton = true;
        }


        /// <summary>
        /// ������� �� ������� �����.
        /// </summary>
        public void GoToMainScreen() => MainWindowViewModel.Self.Us = new MainScreen();

        /// <summary>
        /// ������� �� ����� ��������������.
        /// </summary>
        public void GoToAdminMainScreen() => MainWindowViewModel.Self.Us = new AdminMainScreen();

        /// <summary>
        /// ����� ��� ��������� �����.
        /// </summary>
        public void GenerateCaptcha()
        {
            Random randomizer = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int codeLength = 4;
            CaptchaText = "";
            Canvas captchaCanvas = new Canvas()
            {
                Width = 200,
                Height = 90,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Background = new SolidColorBrush(Color.Parse("#66ff66"))
            };
            double initialX = randomizer.Next(10, 20); // ���������� ��������� ������� ������� �� �����������
            for (int i = 0; i < codeLength; i++)
            {
                TextBlock textElement = new TextBlock();
                textElement.FontSize = 20;
                textElement.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                textElement.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
                textElement.Foreground = new SolidColorBrush(Colors.BlueViolet);
                textElement.FontWeight = FontWeight.Bold;
                if (randomizer.Next(10) % 2 == randomizer.Next(1)) // 50% �����������, ��� ����� ����� ��� �����
                {
                    int randomDigit = randomizer.Next(10);
                    textElement.Text = randomDigit.ToString();
                    CaptchaText += Convert.ToString(randomDigit);
                }
                else
                {
                    int randomIndex = randomizer.Next(chars.Length);
                    char randomChar = chars[randomIndex];
                    textElement.Text = randomChar.ToString();
                    CaptchaText += Convert.ToString(randomChar);
                }
                Canvas.SetLeft(textElement, initialX + (i * 35));
                Canvas.SetTop(textElement, randomizer.Next(25, 40));
                captchaCanvas.Children.Add(textElement);
            }
            for (int i = 0; i < randomizer.Next(10, 15); i++) // ��������� ���������� �����
            {
                Line distortionLine = new Line()
                {
                    StartPoint = new Avalonia.Point(randomizer.Next(210), randomizer.Next(90)),
                    EndPoint = new Avalonia.Point(randomizer.Next(210), randomizer.Next(90)),
                    Stroke = new SolidColorBrush(Color.FromRgb(Convert.ToByte(randomizer.Next(256)), Convert.ToByte(randomizer.Next(255)), Convert.ToByte(randomizer.Next(255)))),
                    StrokeThickness = randomizer.Next(3)
                };
                captchaCanvas.Children.Add(distortionLine);
            }
            CaptchaImage = captchaCanvas;
        }
    }
}