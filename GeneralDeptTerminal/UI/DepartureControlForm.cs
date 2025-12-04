using System;
using System.Drawing;
using System.Windows.Forms;
using GeneralDeptTerminal.Models;
using GeneralDeptTerminal.Services;

namespace GeneralDeptTerminal.UI
{
    public class DepartureControlForm : Form
    {
        private readonly Request _request;
        private readonly Label _lblInfo = new();
        private readonly Button _btnRecordDeparture = new();

        public DepartureControlForm(Request request)
        {
            _request = request;

            Text = $"Фиксация убытия для заявки #{request.Id}";
            Width = 400;
            Height = 250;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            _lblInfo.Left = 10;
            _lblInfo.Top = 10;
            _lblInfo.Width = 360;
            _lblInfo.Height = 100;
            _lblInfo.Text = $"Заявка ID: {_request.Id}\nТип: {_request.Type}\nПодразделение: {_request.Department}\nЗаявители: {_request.Applicants.Count}\nВремя начала посещения: {_request.ActualVisitStartTime?.ToString("HH:mm:ss") ?? "Не зафиксировано"}";
            _lblInfo.TextAlign = ContentAlignment.MiddleCenter;

            _btnRecordDeparture.Text = "Зафиксировать убытие";
            _btnRecordDeparture.Left = 120;
            _btnRecordDeparture.Top = 130;
            _btnRecordDeparture.Width = 150;
            _btnRecordDeparture.Height = 50;
            _btnRecordDeparture.Click += BtnRecordDeparture_Click;

            Controls.Add(_lblInfo);
            Controls.Add(_btnRecordDeparture);

            LoadData();
        }

        private void LoadData()
        {
            // Здесь можно добавить дополнительную логику для отображения информации о заявке
        }

        private void BtnRecordDeparture_Click(object? sender, EventArgs e)
        {
            if (!_request.ActualVisitStartTime.HasValue)
            {
                MessageBox.Show("Сначала необходимо зафиксировать время начала посещения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _request.ActualVisitEndTime = DateTime.Now;
            Program.RequestService.UpdateRequest(_request);

            MessageBox.Show($"Время убытия зафиксировано для заявки ID: {_request.Id}. Время убытия: {_request.ActualVisitEndTime:HH:mm:ss}", "Убытие зафиксировано", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
