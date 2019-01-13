using System;
using System.Threading.Tasks;

namespace ParallelxReactive
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await new FileSystemTests().Run();
            await new HttpTests().Run();

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}