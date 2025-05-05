using System;

namespace Lab15
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double[,] table = { 
                {-8, -4, -1}, // 4·x1 + x2 ≥ 8
                {10, -2, 3}, // -2·x1 + 3·x2 ≤ 10
                {16, 1, 2}, // x1 + 2·x2 ≤ 16
                {12, 2, -1}, // 2·x1 - x2 ≤ 12
                {0, 0, -1} // f(x)= x2 → max
            };
            double[] result = new double[2];
            double[,] table_result;
            Simplex S = new Simplex(table);
            table_result = S.Calculate(result);
            Console.WriteLine("Розв’язок симплекс-таблиці:");
            for (int i = 0; i < table_result.GetLength(0); i++)
            {
                for (int j = 0; j < table_result.GetLength(1); j++)
                    Console.Write(table_result[i, j] + " ");
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Розв’язок:");
            Console.WriteLine("X[1] = " + result[0]);
            Console.WriteLine("X[2] = " + result[1]);
            Console.WriteLine("F = " + table_result[4,0]);
            Console.ReadLine();
        }
    }
}
