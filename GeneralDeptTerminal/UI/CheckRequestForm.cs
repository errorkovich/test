using System;
using System.Text;
using System.Windows.Forms;
using GeneralDeptTerminal.Models;

namespace GeneralDeptTerminal.UI
{
    public class CheckRequestForm : Form
    {
        private readonly Request _request;

        private readonly Label _lblBlacklist = new();
        private readonly TextBox _txtInfo = new();
        private readonly DateTimePicker _dtpDate = new();
        private readonly DateTimePicker _dtpTime = new();
        private readonly ComboBox _cmbStatus = new();
        private readonly Button _btnRejectFakeData = new();
        private readonly Button _btnRejectFiles = new();
        private readonly Button _btnSave = new();
        private readonly Button _btnCancel = new();

        private bool _rejectByFakeData;
        private bool _rejectByFiles;

        public CheckRequestForm(Request request)
        {
            _request = request;

            Text = $"Формальная проверка заявки #{request.Id}";
            Width = 700;
            Height = 550;
            StartPosition = FormStartPosition.CenterParent;

            _lblBlacklist.Left = 10;
            _lblBlacklist.Top = 10;
            _lblBlacklist.Width = 660;
            _lblBlacklist.Height = 30;

            _txtInfo.Left = 10;
            _txtInfo.Top = 45;
            _txtInfo.Width = 660;
            _txtInfo.Height = 260;
            _txtInfo.Multiline = true;
            _txtInfo.ReadOnly = true;
            _txtInfo.ScrollBars = ScrollBars.Vertical;

            var lblDate = new Label { Text = "Дата посещения:", Left = 10, Top = 320, Width = 130 };
            _dtpDate.Left = 150;
            _dtpDate.Top = 315;
            _dtpDate.Width = 150;

            var lblTime = new Label { Text = "Время посещения:", Left = 320, Top = 320, Width = 130 };
            _dtpTime.Left = 460;
            _dtpTime.Top = 315;
            _dtpTime.Width = 100;
            _dtpTime.Format = DateTimePickerFormat.Time;
            _dtpTime.ShowUpDown = true;

            var lblStatus = new Label { Text = "Статус:", Left = 10, Top = 360, Width = 60 };
            _cmbStatus.Left = 80;
            _cmbStatus.Top = 355;
            _cmbStatus.Width = 150;

            _btnRejectFakeData.Text = "Отклонить: недостоверные данные";
            _btnRejectFakeData.Left = 250;
            _btnRejectFakeData.Top = 350;
            _btnRejectFakeData.Width = 220;
            _btnRejectFakeData.Click += BtnRejectFakeData_Click;

            _btnRejectFiles.Text = "Отклонить: проблема с файлами";
            _btnRejectFiles.Left = 480;
            _btnRejectFiles.Top = 350;
            _btnRejectFiles.Width = 190;
            _btnRejectFiles.Click += BtnRejectFiles_Click;

            _btnSave.Text = "Сохранить";
            _btnSave.Left = 390;
            _btnSave.Top = 420;
            _btnSave.Width = 120;
            _btnSave.Click += BtnSave_Click;

            _btnCancel.Text = "Отмена";
            _btnCancel.Left = 520;
            _btnCancel.Top = 420;
            _btnCancel.Width = 120;
            _btnCancel.Click += (_, _) => Close();

            Controls.Add(_lblBlacklist);
            Controls.Add(_txtInfo);
            Controls.Add(lblDate);
            Controls.Add(_dtpDate);
            Controls.Add(lblTime);
            Controls.Add(_dtpTime);
            Controls.Add(lblStatus);
            Controls.Add(_cmbStatus);
            Controls.Add(_btnRejectFakeData);
            Controls.Add(_btnRejectFiles);
            Controls.Add(_btnSave);
            Controls.Add(_btnCancel);

            LoadData();
        }

        private void LoadData()
        {
            _txtInfo.Text = BuildRequestInfo(_request);

            var svc = Program.RequestService;
            if (svc.IsInBlacklist(_request, out var _))
            {
                _lblBlacklist.Text = "ВНИМАНИЕ: найдены записи в черном списке! Заявка автоматически отклонена.";
                _lblBlacklist.ForeColor = System.Drawing.Color.Red;

                _request.Status = RequestStatus.Rejected;
                svc.SendMessageToApplicants(_request,
                    "Заявка на посещение объекта КИИ отклонена в связи с нарушением закона.");

                _dtpDate.Enabled = false;
                _dtpTime.Enabled = false;
                _cmbStatus.Enabled = false;
                _btnRejectFakeData.Enabled = false;
                _btnRejectFiles.Enabled = false;
                _btnSave.Enabled = false;
            }
            else
            {
                _lblBlacklist.Text = "Записей в черном списке не найдено.";
                _lblBlacklist.ForeColor = System.Drawing.Color.DarkGreen;

                _cmbStatus.Items.Add("Одобрена");
                _cmbStatus.Items.Add("Отклонена");
                _cmbStatus.SelectedIndex = _request.Status == RequestStatus.Approved ? 0 : 1;

                _dtpDate.Value = _request.VisitDate ?? DateTime.Today;
                _dtpTime.Value = DateTime.Today + (_request.VisitTime ?? new TimeSpan(9, 0, 0));
            }
        }

        private static string BuildRequestInfo(Request r)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"ID: {r.Id}");
            sb.AppendLine($"Тип: {r.Type}");
            sb.AppendLine($"Подразделение: {r.Department}");
            sb.AppendLine($"Статус: {r.Status}");
            sb.AppendLine($"Файлы: {r.AttachedFilesCount}");
            sb.AppendLine("Заявители:");
            foreach (var a in r.Applicants)
            {
                sb.AppendLine($" - {a.FullName}, паспорт {a.PassportNumber}, {a.Email}");
            }

            return sb.ToString();
        }

        private void BtnRejectFakeData_Click(object? sender, EventArgs e)
        {
            _rejectByFakeData = true;
            _rejectByFiles = false;
            _cmbStatus.SelectedItem = "Отклонена";
        }

        private void BtnRejectFiles_Click(object? sender, EventArgs e)
        {
            _rejectByFiles = true;
            _rejectByFakeData = false;
            _cmbStatus.SelectedItem = "Отклонена";
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (_cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Укажите статус.");
                return;
            }

            var svc = Program.RequestService;

            if (_cmbStatus.SelectedItem.ToString() == "Одобрена")
            {
                _request.Status = RequestStatus.Approved;
                _request.VisitDate = _dtpDate.Value.Date;
                _request.VisitTime = _dtpTime.Value.TimeOfDay;

                var msg = $"Заявка на посещение объекта КИИ одобрена, дата посещения: " +
                          $"{_request.VisitDate:dd.MM.yyyy}, время посещения: {_request.VisitTime:hh\\:mm}.";
                svc.SendMessageToApplicants(_request, msg);
            }
            else
            {
                _request.Status = RequestStatus.Rejected;

                string msg;
                if (_rejectByFakeData)
                {
                    _request.FakeDataRejectCount++;
                    svc.AddToBlacklistIfNeeded(_request);
                    msg = "Заявка на посещение объекта КИИ отклонена в связи с нарушением закона (недостоверные данные).";
                }
                else if (_rejectByFiles)
                {
                    msg = "Заявка на посещение объекта КИИ отклонена в связи с нарушением закона (приложенные файлы).";
                }
                else
                {
                    msg = "Заявка на посещение объекта КИИ отклонена в связи с нарушением закона.";
                }

                svc.SendMessageToApplicants(_request, msg);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}


