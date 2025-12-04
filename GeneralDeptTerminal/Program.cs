using GeneralDeptTerminal.Models;
using GeneralDeptTerminal.Services;
using System;
using System.Windows.Forms;

namespace GeneralDeptTerminal;

internal static class Program
{
    public static RequestService RequestService = new();
    public static Employee? CurrentEmployee;

    [STAThread]
    private static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Application.Run(new UI.LoginForm());
    }
}



