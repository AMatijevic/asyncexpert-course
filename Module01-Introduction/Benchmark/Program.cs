using System;
using System.Reflection;
using BenchmarkDotNet.Running;

namespace Dotnetos.AsyncExpert.Homework.Module01.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
            
            //var calculator = new FibonacciCalc();
            //Console.WriteLine(calc.Recursive(15));
            //Console.WriteLine(calc.RecursiveWithMemoization(15));
            //Console.WriteLine(calc.Iterative(15));
            //Console.WriteLine(calc.IterativeNoArray(15));
        }
    }
}
