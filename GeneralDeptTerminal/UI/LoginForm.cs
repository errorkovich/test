using System;
using System.Windows.Forms;
using GeneralDeptTerminal.Services;

namespace GeneralDeptTerminal.UI
{
    public class LoginForm : Form
    {
        private readonly TextBox _txtCode = new();
        private readonly Button _btnLogin = new();

        public LoginForm()
        {
            Text = "Авторизация сотрудника общего отдела";
            Width = 400;
            Height = 180;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            var lbl = new Label
            {
                Text = "Код сотрудника:",
                Left = 20,
                Top = 20,
                Width = 130
            };
            _txtCode.Left = 160;
            _txtCode.Top = 18;
            _txtCode.Width = 180;

            _btnLogin.Text = "Войти";
            _btnLogin.Left = 160;
            _btnLogin.Top = 60;
            _btnLogin.Width = 100;
            _btnLogin.Click += BtnLogin_Click;

            Controls.Add(lbl);
            Controls.Add(_txtCode);
            Controls.Add(_btnLogin);
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            var emp = Program.RequestService.Authorize(_txtCode.Text.Trim());
            if (emp == null)
            {
                MessageBox.Show("Неверный код сотрудника", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Program.CurrentEmployee = emp;
            Hide();
            using var main = new MainForm();
            main.ShowDialog();
            Close();
        }
    }
}


