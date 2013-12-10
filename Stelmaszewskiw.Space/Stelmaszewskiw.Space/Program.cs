namespace Stelmaszewskiw.Space
{
    class Program
    {
        static void Main()
        {
            using (var program = new Space())
            {
                program.Run();
            }
        }
    }
}
