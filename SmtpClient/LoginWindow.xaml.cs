using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using SmtpClient.MailHandler;

namespace SmtpClient;

public partial class LoginWindow : Window
{
    private static Regex NumberRegex { get; set; } = new Regex("[^0-9]+");

    public LoginWindow()
    {
        InitializeComponent();
        EmailBox.Text = LocalStorage.Email;
        PasswordBox.Password = LocalStorage.Password;
    }

    private void Enter(object sender, RoutedEventArgs e)
    {
        LocalStorage.Email = EmailBox.Text;
        LocalStorage.Password = PasswordBox.Password;
        LocalStorage.SmptAdress = SmtpAddr.Text;
        LocalStorage.PopAdress = PopAddr.Text;
        LocalStorage.SmptPortNum = int.TryParse(SmtpPort.Text, out var smtp) ? smtp : LocalStorage.SmptPortNum;
        LocalStorage.PopPortNum = int.TryParse(PopPort.Text, out var pop) ? pop : LocalStorage.PopPortNum;
        
        if (!string.IsNullOrEmpty(LocalStorage.Email) && !string.IsNullOrEmpty(LocalStorage.Password))
        {
            DialogResult = true;
        }
    }

    private void CheckPortNum(object sender, TextCompositionEventArgs e)
    {
        e.Handled = NumberRegex.IsMatch(e.Text);
    }
}