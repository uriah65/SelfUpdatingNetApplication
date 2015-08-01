using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhoLocked
{
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                if (Upgrader.Program.IsApplicationRestart(args))
                {
                    //todo: introduce test delay shutting application down ....
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\nSorry, an error has occurred.\n");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nProgram was terminated.\nPress any key to continue.");
                Console.ReadKey();
            }

            Console.WriteLine("Application finished. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
