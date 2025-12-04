using System;
using System.Drawing;
using System.Windows.Forms;
using GeneralDeptTerminal.Models;
using GeneralDeptTerminal.Services;

namespace GeneralDeptTerminal.UI
{
    public class AccessControlForm : Form
    {
        private readonly Request _request;
        private readonly Label _lblInfo = new();
        private readonly Button _btnGrantAccess = new();

        public AccessControlForm(Request request)
        {
            _request = request;

            Text = $"Управление доступом для заявки #{request.Id}";
            Width = 400;
            Height = 250;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            _lblInfo.Left = 10;
            _lblInfo.Top = 10;
            _lblInfo.Width = 360;
            _lblInfo.Height = 100;
            _lblInfo.Text = $"Заявка ID: {_request.Id}\nТип: {_request.Type}\nПодразделение: {_request.Department}\nЗаявители: {_request.Applicants.Count}";
            _lblInfo.TextAlign = ContentAlignment.MiddleCenter;

            _btnGrantAccess.Text = "Разрешить проход";
            _btnGrantAccess.Left = 120;
            _btnGrantAccess.Top = 130;
            _btnGrantAccess.Width = 150;
            _btnGrantAccess.Height = 50;
            _btnGrantAccess.Click += BtnGrantAccess_Click;

            Controls.Add(_lblInfo);
            Controls.Add(_btnGrantAccess);

            LoadData();
        }

        private void LoadData()
        {
            // Здесь можно добавить дополнительную логику для отображения информации о заявке
            // Например, более подробную информацию о заявителях
        }

        private void BtnGrantAccess_Click(object? sender, EventArgs e)
        {
            // 1. Системный звук
            System.Media.SystemSounds.Beep.Play();
            MessageBox.Show("Сигнал о разрешении на проход подан!", "Доступ разрешен", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 2. Отправка сообщения на сервер об открытии турникета (в рамках чемпионата)
            // В реальном приложении здесь будет вызов API или другой сервис для взаимодействия с турникетом.
            // Сейчас это будет просто вывод в консоль.
            Console.WriteLine($"Сообщение на сервер: Турникет открыт для заявки ID: {_request.Id}");

            // 3. Фиксация времени начала посещения в базе данных
            _request.ActualVisitStartTime = DateTime.Now;
            Program.RequestService.UpdateRequest(_request); // Метод UpdateRequest нужно будет создать

            MessageBox.Show($"Доступ разрешен для заявки ID: {_request.Id}. Время начала посещения: {_request.ActualVisitStartTime:HH:mm:ss}", "Доступ разрешен", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
