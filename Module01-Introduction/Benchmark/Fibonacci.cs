using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using static System.Console;

namespace Dotnetos.AsyncExpert.Homework.Module01.Benchmark
{
    [DisassemblyDiagnoser(exportCombinedDisassemblyReport: true)]
    [MemoryDiagnoser]
    [RankColumn]
    public class FibonacciCalc
    {
        // HOMEWORK:
        // 1. Write implementations for RecursiveWithMemoization and Iterative solutions
        // 2. Add MemoryDiagnoser to the benchmark
        // 3. Run with release configuration and compare results
        // 4. Open disassembler report and compare machine code
        // 
        // You can use the discussion panel to compare your results with other students
        // Memoization - Storage or Cache during the execution 

        Dictionary<ulong, ulong> TempMemo = new Dictionary<ulong, ulong>(); 

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Data))]
        public ulong Recursive(ulong n)
        {
            if (n == 1 || n == 2) return 1;
            return Recursive(n - 2) + Recursive(n - 1);
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public ulong RecursiveWithMemoization(ulong n)
        {
            if (TempMemo.TryGetValue(n, out ulong tempResult))
                return tempResult;

            if (n == 1 || n == 2)
            {
                TempMemo.Add(n, 1);
                return 1;
            }
            var result = Recursive(n - 2) + Recursive(n - 1);
            TempMemo.Add(n, result);
            return result;
        }
        
        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public ulong Iterative(ulong n)
        {
            var list = new List<ulong>();

            for (int i = 0; i <= (int)n; i++)
            {
                if (i <= 2)
                {
                    if (!list.Any())
                    {
                        list.Add(0);
                    }
                    else
                    {
                        list.Add(1);
                    }
                }
                else
                {
                    var result = list[i - 2] + list[i - 1];
                    list.Add(result);
                }
            }
            return list.LastOrDefault();

        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public ulong IterativeNoArray(ulong n)
        {
            ulong tmp2 = default;
            ulong result = default;

            for (int i = default; i < (int)n; i++)
            {
                ulong tmp1;
                if (i <= 1)
                {
                    tmp1 = 0;
                    tmp2 = (ulong)i;
                }
                else
                {
                    tmp1 = tmp2;
                    tmp2 = result;
                }
                result = tmp1 + tmp2;
            }

            return result;
        }

        public IEnumerable<ulong> Data()
        {
            yield return 15;
            yield return 35;
        }
    }
}
