using System;

namespace Stage0
{
    partial class Program
    {
         static void Main(string[] args)
        {
            Welcome7582();
            Welcome3332();
            Console.ReadKey();
        }
        static partial void Welcome3332();
        private static void Welcome7582()
        {
            //input the users's name
            Console.WriteLine("Enter your name: ");
            string name = Console.ReadLine()!;

            //print message with the users's name
            Console.WriteLine("{0}, welcome to my first console application", name);
        }
    }
}
