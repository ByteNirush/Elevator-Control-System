using System;
using System.Windows.Forms;

namespace ElevatorControlSystem
{
    /// <summary>
    /// Main Program Entry Point
    /// TASK 5: Exception handling at application level
    /// TASK 7: Professional application initialization
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Includes global exception handling for production quality.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // TASK 5: Global exception handlers for unhandled exceptions
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                // Enable visual styles for modern UI appearance
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Initialize and run main form
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                // Catch any startup exceptions
                MessageBox.Show(
                    $"Fatal error during application startup:\n\n{ex.Message}\n\nThe application will now close.",
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Handle UI thread exceptions
        /// TASK 5: Exception handling for UI thread
        /// </summary>
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                MessageBox.Show(
                    $"An error occurred:\n\n{e.Exception.Message}\n\nPlease try again.",
                    "Application Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch
            {
                // If error handling fails, exit gracefully
                Application.Exit();
            }
        }

        /// <summary>
        /// Handle non-UI thread exceptions
        /// TASK 5: Exception handling for background threads
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.ExceptionObject as Exception;
                string message = ex?.Message ?? "Unknown error occurred";

                MessageBox.Show(
                    $"A critical error occurred:\n\n{message}\n\nThe application will now close.",
                    "Critical Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                Application.Exit();
            }
        }
    }
}
