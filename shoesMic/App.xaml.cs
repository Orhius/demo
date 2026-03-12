using shoesMic.Views;
using System.Windows;

namespace shoesMic
{
    /// <summary>
    /// Точка входа WPF-приложения.
    /// При запуске открывает окно авторизации <see cref="LoginWindow"/>.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Вызывается при старте приложения.
        /// Создаёт и отображает окно входа.
        /// </summary>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }

}
