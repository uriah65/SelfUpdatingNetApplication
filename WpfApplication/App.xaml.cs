using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //todo: check if app.exe / upgrader.exe exist before launching ...
        //todo: better msg if upgrader requires main app to start ...
        //todo: changes file's case when copy

        protected override void OnStartup(StartupEventArgs e)
        {
            /* check if application update is required */            
            try
            {
                if (Upgrader.Program.IsApplicationRestart(null))
                {
                    Shutdown(0);
                    return;
                }
            }
            catch (Exception ex)
            {
                string message = $"\nSorry, an error has occurred.\n{ex.Message}";
                MessageBox.Show(message, "Upgrade exception", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(0);
                return;
            }
 
            /* continue with application normally */ 
            base.OnStartup(e);
        }

    }
}
