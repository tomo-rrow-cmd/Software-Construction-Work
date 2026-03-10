using System;
using System.Collections.Generic;

namespace PrimePrinter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("hello world");
            Console.WriteLine("你好！我是陈铭杰");
            Console.WriteLine("请输入两个整数：");

            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("输入不能为空。");
                return;
            }

            string[] parts = input.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                Console.WriteLine("请输入两个整数。");
                return;
            }

            if (!int.TryParse(parts[0], out int lower) || !int.TryParse(parts[1], out int upper))
            {
                Console.WriteLine("输入必须为整数。");
                return;
            }

           
            if (lower > upper)
            {
                int temp = lower;
                lower = upper;
                upper = temp;
            }

            // 找出范围内的所有素数
            List<int> primes = new List<int>();
            for (int num = lower; num <= upper; num++)
            {
                if (IsPrime(num))
                {
                    primes.Add(num);
                }
            }

 
            int count = primes.Count;
            for (int i = 0; i < count; i++)
            {
                Console.Write(primes[i]);
                if ((i + 1) % 10 == 0)
                {
                    Console.WriteLine(); // 每10个换行
                }
                else if (i < count - 1)
                {
                    Console.Write(" ");
                }
            }

            
            if (count > 0 && count % 10 != 0)
            {
                Console.WriteLine();
            }
        }

        
        // 判断一个整数是否为素数
        static bool IsPrime(int n)
        {
            if (n <= 1) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;

            int boundary = (int)Math.Floor(Math.Sqrt(n));
            for (int i = 3; i <= boundary; i += 2)
            {
                if (n % i == 0) return false;
            }
            return true;
        }
    }
}
