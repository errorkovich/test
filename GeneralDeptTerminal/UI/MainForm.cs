using System;
using System.Linq;
using System.Windows.Forms;
using GeneralDeptTerminal.Models;

namespace GeneralDeptTerminal.UI
{
    public class MainForm : Form
    {
        private readonly DataGridView _grid = new();
        private readonly ComboBox _cmbType = new();
        private readonly ComboBox _cmbDepartment = new();
        private readonly Button _btnCheck = new();
        private readonly DateTimePicker _dtpDate = new();
        private readonly Button _btnClearDateFilter = new();
        private readonly TextBox _txtSearch = new();
        private readonly Button _btnSearch = new();
        private readonly Button _btnGrantAccess = new();
        private readonly Button _btnRecordDeparture = new();

        private bool _isDateFilterActive = false;

        public MainForm()
        {
            Text = $"Терминал общего отдела – {Program.CurrentEmployee?.FullName}";
            Width = 950;
            Height = 640; // Increased height
            StartPosition = FormStartPosition.CenterScreen;

            var lblType = new Label { Text = "Тип:", Left = 10, Top = 15, Width = 40 };
            _cmbType.Left = 50;
            _cmbType.Top = 10;
            _cmbType.Width = 150;
            _cmbType.SelectedIndexChanged += (_, _) => LoadData();

            var lblDep = new Label { Text = "Подразделение:", Left = 220, Top = 15, Width = 110 };
            _cmbDepartment.Left = 335;
            _cmbDepartment.Top = 10;
            _cmbDepartment.Width = 150;
            _cmbDepartment.SelectedIndexChanged += (_, _) => LoadData();

            var lblDate = new Label { Text = "Дата:", Left = 505, Top = 15, Width = 40 };
            _dtpDate.Left = 550;
            _dtpDate.Top = 10;
            _dtpDate.Width = 150;
            _dtpDate.Format = DateTimePickerFormat.Short;
            _dtpDate.ValueChanged += (_, _) =>
            {
                _isDateFilterActive = true;
                LoadData();
            };

            _btnClearDateFilter.Text = "X";
            _btnClearDateFilter.Left = 710;
            _btnClearDateFilter.Top = 8;
            _btnClearDateFilter.Width = 30;
            _btnClearDateFilter.Click += (_, _) =>
            {
                _isDateFilterActive = false;
                _dtpDate.Value = DateTime.Now; // Reset date picker to current date
                LoadData();
            };

            var lblSearch = new Label { Text = "Поиск:", Left = 10, Top = 50, Width = 40 };
            _txtSearch.Left = 50;
            _txtSearch.Top = 45;
            _txtSearch.Width = 435; // Adjusted width to fit next to button

            _btnSearch.Text = "Найти";
            _btnSearch.Left = 495;
            _btnSearch.Top = 45;
            _btnSearch.Width = 80;
            _btnSearch.Click += (_, _) => LoadData();

            _btnCheck.Text = "Формальная проверка";
            _btnCheck.Left = 500;
            _btnCheck.Top = 45;
            _btnCheck.Width = 130;
            _btnCheck.Click += BtnCheck_Click;

            _btnGrantAccess.Text = "Разрешить проход";
            _btnGrantAccess.Left = 650;
            _btnGrantAccess.Top = 45;
            _btnGrantAccess.Width = 130;
            _btnGrantAccess.Click += BtnGrantAccess_Click;

            _btnRecordDeparture.Text = "Зафиксировать убытие";
            _btnRecordDeparture.Left = 800;
            _btnRecordDeparture.Top = 45;
            _btnRecordDeparture.Width = 130;
            _btnRecordDeparture.Click += BtnRecordDeparture_Click;

            _grid.Left = 10;
            _grid.Top = 80;
            _grid.Width = 930; // Adjusted width
            _grid.Height = 500;
            _grid.ReadOnly = true;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.AutoGenerateColumns = false;

            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "Id", Width = 50 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Тип", DataPropertyName = "Type", Width = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Подразделение", DataPropertyName = "Department", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Статус", DataPropertyName = "Status", Width = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Создана", DataPropertyName = "CreatedAt", Width = 180 });

            Controls.Add(lblType);
            Controls.Add(_cmbType);
            Controls.Add(lblDep);
            Controls.Add(_cmbDepartment);
            Controls.Add(lblDate);
            Controls.Add(_dtpDate);
            Controls.Add(_btnClearDateFilter);
            Controls.Add(lblSearch);
            Controls.Add(_txtSearch);
            Controls.Add(_btnSearch);
            Controls.Add(_btnCheck);
            Controls.Add(_btnGrantAccess);
            Controls.Add(_btnRecordDeparture);
            Controls.Add(_grid);

            LoadFilters();
            LoadData();
        }

        private void LoadFilters()
        {
            _cmbType.Items.Add("Все");
            _cmbType.Items.AddRange(Enum.GetNames(typeof(RequestType)));
            _cmbType.SelectedIndex = 0;

            _cmbDepartment.Items.Add("Все");
            _cmbDepartment.Items.AddRange(Enum.GetNames(typeof(Department)));
            _cmbDepartment.SelectedIndex = 0;

            // Removed _cmbStatus initialization
        }

        private void LoadData()
        {
            RequestType? type = null;
            Department? dep = null;
            RequestStatus? status = RequestStatus.Approved;
            DateTime? createdAt = null;
            string? search = null;

            if (_cmbType.SelectedIndex > 0)
                type = Enum.Parse<RequestType>(_cmbType.SelectedItem!.ToString()!);
            if (_cmbDepartment.SelectedIndex > 0)
                dep = Enum.Parse<Department>(_cmbDepartment.SelectedItem!.ToString()!);

            if (_isDateFilterActive)
                createdAt = _dtpDate.Value.Date;

            search = _txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(search))
                search = null;

            var data = Program.RequestService
                .GetRequests(type, dep, status, createdAt, search)
                .ToList();

            _grid.DataSource = data;
        }

        private Request? CurrentRequest =>
            _grid.CurrentRow?.DataBoundItem as Request;

        private void BtnCheck_Click(object? sender, EventArgs e)
        {
            var req = CurrentRequest;
            if (req == null)
            {
                MessageBox.Show("Выберите заявку");
                return;
            }

            using var f = new CheckRequestForm(req);
            f.ShowDialog();
            LoadData();
        }

        private void BtnGrantAccess_Click(object? sender, EventArgs e)
        {
            var req = CurrentRequest;
            if (req == null)
            {
                MessageBox.Show("Выберите заявку");
                return;
            }

            if (req.Status != RequestStatus.Approved)
            {
                MessageBox.Show("Разрешение на проход возможно только для одобренных заявок.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (req.ActualVisitStartTime.HasValue)
            {
                MessageBox.Show("Проход по этой заявке уже был разрешен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using var f = new AccessControlForm(req);
            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void BtnRecordDeparture_Click(object? sender, EventArgs e)
        {
            var req = CurrentRequest;
            if (req == null)
            {
                MessageBox.Show("Выберите заявку");
                return;
            }

            if (req.Status != RequestStatus.Approved)
            {
                MessageBox.Show("Фиксация убытия возможна только для одобренных заявок.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!req.ActualVisitStartTime.HasValue)
            {
                MessageBox.Show("Сначала необходимо разрешить проход для этой заявки.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (req.ActualVisitEndTime.HasValue)
            {
                MessageBox.Show("Убытие по этой заявке уже было зафиксировано.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using var f = new DepartureControlForm(req);
            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }
    }
}


