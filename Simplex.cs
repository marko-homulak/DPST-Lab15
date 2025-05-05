using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab15
{
    public class Simplex
    {
        double[,] table; //симплекс таблиця
        int m, n;
        List<int> basis; //список базисних змінних
        public Simplex(double[,] source) //source - симплекс таблиця без базисних змінних
        {
            m = source.GetLength(0);
            n = source.GetLength(1);
            table = new double[m, n + m - 1];
            basis = new List<int>();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    if (j < n)
                        table[i, j] = source[i, j];
                    else
                        table[i, j] = 0;
                }
                //виставляємо коефіцієнт 1 перед базисною змінною в рядку
                if ((n + i) < table.GetLength(1))
                {
                    table[i, n + i] = 1;
                    basis.Add(n + i);
                }
            }

            n = table.GetLength(1);
        }
        //result – в цей масив будуть записані отримані значення X
        public double[,] Calculate(double[] result)
        {
            int mainCol, mainRow;
            int processorCount = Environment.ProcessorCount;

            while (!IsItEnd())
            {
                mainCol = findMainCol();
                mainRow = findMainRow(mainCol);
                basis[mainRow] = mainCol;

                double[,] new_table = new double[m, n];

                // КРОК 4-а: нормалізуємо головний рядок (тільки один потік!)
                for (int j = 0; j < n; j++)
                    new_table[mainRow, j] = table[mainRow, j] / table[mainRow, mainCol];

                // КРОК 4-б: паралельна обробка решти рядків
                Parallel.For(0, m, new ParallelOptions { MaxDegreeOfParallelism = processorCount }, i =>
                {
                    if (i == mainRow) return;

                    for (int j = 0; j < n; j++)
                    {
                        new_table[i, j] = table[i, j] - table[i, mainCol] * new_table[mainRow, j];
                    }
                });

                // Оновлюємо таблицю
                table = new_table;
            }

            for (int i = 0; i < result.Length; i++)
            {
                int k = basis.IndexOf(i + 1);
                if (k != -1)
                    result[i] = table[k, 0];
                else
                    result[i] = 0;
            }

            return table;
        }

        private bool IsItEnd()
        {
            bool flag = true;
            for (int j = 1; j < n; j++)
            {
                if (table[m - 1, j] < 0)
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        private int findMainCol()
        {

            int mainCol = 1;
            for (int j = 2; j < n; j++)
                if (table[m - 1, j] < table[m - 1, mainCol])
                    mainCol = j;
            return mainCol;
        }

        private int findMainRow(int mainCol)
        {
            int mainRow = 0;
            for (int i = 0; i < m - 1; i++)
                if (table[i, mainCol] > 0)
                {
                    mainRow = i;
                    break;
                }
            for (int i = mainRow + 1; i < m - 1; i++)
                if ((table[i, mainCol] > 0) && ((table[i, 0] / table[i, mainCol]) < (table[mainRow, 0] / table[mainRow,
                mainCol])))
                    mainRow = i;
            return mainRow;
        }

    }
}
