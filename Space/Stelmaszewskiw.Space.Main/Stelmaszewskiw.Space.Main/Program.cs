using System;

namespace Stelmaszewskiw.Space.Main
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var result = false;

            var system = new System();

            try
            {
                result = system.Initialize();
                if(result)
                {
                    system.Run();
                }
            }
            finally 
            {
                system.Shutdown();
            }
        }
    }
}
