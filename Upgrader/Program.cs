using System;

namespace Upgrader
{
    public class Program
    {
        internal static IUpdateOperations _operations = new Operations(); // to do as factory injection

        private static void Main()
        {
            try
            {
                Constants.LoadConfiguration();
                Main_Inner();
            }
            catch (UpgradeUpgraderException ex)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{ex.Message}");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                if (Constants.Tracer != null)
                {
                    Constants.Tracer.Trace(ex.Message);
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\nSorry, an error has occurred.\n");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nProgram was terminated.\nPress any key to continue.");
                Console.ReadKey();
                return;
            }
        }

        internal static void Main_Inner()
        {
            Constants.Tracer.Trace("Main inner started.");

            /* We come here when AutoUpdater, not main application, has been launched first
            */
            if (_operations.IsUpgraderUpdateRequired())
            {
                /* Updater has been launched first,
                *  and the new version of Updater has been found.
                *  We recommend user to start main application as if should be in the first place.
                */
                Constants.Tracer.Trace("Main inner: Exception Upgrader Update required.");
                throw new UpgradeUpgraderException($"Please start main application '{Constants.ApplicationExe}'.", null);// UpgradeException(Constants.MESSAGE_START_MAINAPP);
            }

            /* if Updater itself is up to date, we can check main application, update it if required,
            * and launch main application it.
            */
            if (_operations.IsApplicationUpdateRequired(true))
            {
                Constants.Tracer.Trace("Main inner: Upgrading application.");
                _operations.UpdateApplication(true);
            }

            Constants.Tracer.Trace("Main inner: launching application.");
            _operations.LaunchApplication(null);

            Constants.Tracer.Trace("Main inner finished.");
        }

        /// <summary>
        /// This static method is called from the main application
        /// </summary>
        /// <param name="args">arguments received by the main application</param>
        /// <returns>True is a signal to the main application to quit an start AutoUpdater as a stand alone process.
        /// This happens either main application require an update or AutoUpdater just updated itself and needs to be rerun to accept possible new rules.
        /// False, - neither main application, nor AutoUpdater needs to be updated. Main application can continue normally.</returns>
        public static bool IsApplicationRestart(string[] args)
        {
            Constants.LoadConfiguration();
            return IsApplicationRestart_Inner(args);
        }

        internal static bool IsApplicationRestart_Inner(string[] args)
        {
            Constants.Tracer.Trace("IsApplicationRestart_Inner started.");

            bool restart = false;
            bool wasLunchedByUpdater = Helpers.HasArg(args, Constants.LAUNCHED_FROM_UPDATER);

            if (_operations.IsUpgraderUpdateRequired())
            {
                if (wasLunchedByUpdater)
                {
                    /* This method is called only from withing the process of the main application. Main application has been called by updater.
                    *  This can only be if user started updater, which, after possible update of the main application, launched main application.
                    *  Since updater run OK, the new version of it has not been discovered few ticks back.
                    *  Yet, now new updater discovered.
                    *  Even it is so, we report an error, and user can start main application, so that new version of Updater can be updated.
                    */
                    Constants.Tracer.Trace("IsApplicationRestart_Inner: Exception Launch by Upgrader.");
                    throw new UpgradeException(Constants.MESSAGE_CANNOT_COMPLETE_UPDATE, null); //todo: custom exception
                }

                /* Here we know that the main application has been started first.
                *  We found that AutoUpdater itself requires an update.
                *  Since at that point current version of Updater is loaded in the memory of the main application, overwriting AutoUpdater.exe is possible.
                */
                Constants.Tracer.Trace("IsApplicationRestart_Inner: Upgrade-ing Upgrader.");
                _operations.UpgradeUpdater();
                /* We just performed update of Updater. We would like to shut down main application
                *  and launch this new updater to let updated version run.
                */
                restart = true;
            }

            if (_operations.IsApplicationUpdateRequired(true))
            {
                if (wasLunchedByUpdater)
                {
                    /* Since it was came here already been in updater, main application has to be updated.
                    *  In this not the case, something is wrong, or very new version just appears.
                    *  In both these case re-lunch is reasonable.
                    */
                    Constants.Tracer.Trace("IsApplicationRestart_Inner: Exception Launch by upgrader 2.");
                    throw new UpgradeException(Constants.MESSAGE_CANNOT_COMPLETE_UPDATE, null);
                }

                /* We are in the process of the main application. New version has been discovered.
                *  We request closure on the current main application and launching Updater to update main app.
                */
                
                restart = true;
            }

            if (restart)
            {
                Constants.Tracer.Trace("IsApplicationRestart_Inner: Launching upgrader.");
                _operations.LaunchUpgrader(args);
            }

            Constants.Tracer.Trace($"IsApplicationRestart_Inner finished. Application restart request: {restart}.");
            return restart;
        }
    }
}