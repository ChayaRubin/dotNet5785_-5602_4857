using System;
namespace Targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome5602();
            Welcome4857();
            Console.ReadKey();
        }

        static partial void Welcome5602();

        private static void Welcome4857()
        {
            Console.WriteLine("Enter your name: ");
            string userName = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console application", userName);
        }
    }
}

