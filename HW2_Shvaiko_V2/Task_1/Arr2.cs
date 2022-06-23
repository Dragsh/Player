using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_1
{
   // Второй поток заполняет массив2 случайными вещественными числами,
   // находит максимальный элемент массива этого массива, значение максимального
   // элемента и элементы массива выводит в консоль(при помощи StringBuilder).
   // Метод потока – нестатический метод класса.
   //	Массив2 – double, 21 элемент

    public class Arr2
    {
        double[] Array { get; set; }

        public Arr2()
        {
            Random rnd = new Random();
            // Заполнение массива случайными числами
            //С помощью статических, не расширяющих методов класса Enumerable
            Array = Enumerable
                .Repeat(0d, 21)
                .Select(x => rnd.NextDouble())
                .ToArray();
        }//Arr1()

        public void ShowArr()
        {
            Thread.Sleep(5000);

            List<double> listArray = new List<double>(Array);
            Console.WriteLine("Массив случайных чисел:");
            listArray.ForEach(item => Console.Write($"{item:F3}  "));
            Console.WriteLine();
            Thread.Sleep(2000);
            double max = Array.Max();
            Console.WriteLine($"Максимальное значение массива:{max}");
            Console.WriteLine();
            //  задержка потока 2 сек:
            Thread.Sleep(5000);
        }//ShowArr()
    }//Arr2
}//Task_1
