using System;

namespace Stelmaszewskiw.Space.Main
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using(var system = new System())
            {
                try
                {
                    var result = system.Initialize();
                    if (result)
                    {
                        system.Run();
                    }
                }
                finally
                {
                    
                }
            }
        }
    }
}
