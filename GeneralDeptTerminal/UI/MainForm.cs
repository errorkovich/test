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
        private readonly ComboBox _cmbStatus = new();
        private readonly Button _btnCheck = new();

        public MainForm()
        {
            Text = $"Терминал общего отдела – {Program.CurrentEmployee?.FullName}";
            Width = 900;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;

            var lblType = new Label { Text = "Тип:", Left = 10, Top = 15, Width = 40 };
            _cmbType.Left = 50;
            _cmbType.Top = 10;
            _cmbType.Width = 150;

            var lblDep = new Label { Text = "Подразделение:", Left = 220, Top = 15, Width = 110 };
            _cmbDepartment.Left = 335;
            _cmbDepartment.Top = 10;
            _cmbDepartment.Width = 150;

            var lblStatus = new Label { Text = "Статус:", Left = 505, Top = 15, Width = 60 };
            _cmbStatus.Left = 565;
            _cmbStatus.Top = 10;
            _cmbStatus.Width = 150;

            _btnCheck.Text = "Формальная проверка";
            _btnCheck.Left = 730;
            _btnCheck.Top = 8;
            _btnCheck.Width = 150;
            _btnCheck.Click += BtnCheck_Click;

            _grid.Left = 10;
            _grid.Top = 45;
            _grid.Width = 870;
            _grid.Height = 500;
            _grid.ReadOnly = true;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.AutoGenerateColumns = false;

            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "Id", Width = 50 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Тип", DataPropertyName = "Type", Width = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Подразделение", DataPropertyName = "Department", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Статус", DataPropertyName = "Status", Width = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Создана", DataPropertyName = "CreatedAt", Width = 180 });

            _cmbType.SelectedIndexChanged += (_, _) => LoadData();
            _cmbDepartment.SelectedIndexChanged += (_, _) => LoadData();
            _cmbStatus.SelectedIndexChanged += (_, _) => LoadData();

            Controls.Add(lblType);
            Controls.Add(_cmbType);
            Controls.Add(lblDep);
            Controls.Add(_cmbDepartment);
            Controls.Add(lblStatus);
            Controls.Add(_cmbStatus);
            Controls.Add(_btnCheck);
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

            _cmbStatus.Items.Add("Все");
            _cmbStatus.Items.AddRange(Enum.GetNames(typeof(RequestStatus)));
            _cmbStatus.SelectedIndex = 0;
        }

        private void LoadData()
        {
            RequestType? type = null;
            Department? dep = null;
            RequestStatus? status = null;

            if (_cmbType.SelectedIndex > 0)
                type = Enum.Parse<RequestType>(_cmbType.SelectedItem!.ToString()!);
            if (_cmbDepartment.SelectedIndex > 0)
                dep = Enum.Parse<Department>(_cmbDepartment.SelectedItem!.ToString()!);
            if (_cmbStatus.SelectedIndex > 0)
                status = Enum.Parse<RequestStatus>(_cmbStatus.SelectedItem!.ToString()!);

            var data = Program.RequestService
                .GetRequests(type, dep, status)
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
    }
}


