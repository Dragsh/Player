using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_1
{
    //1.	Первый поток заполняет массив1 случайными целыми числами,
    //вычисляет сумму элементов этого массива, сумму и элементы массива
    //выводит в консоль(при помощи StringBuilder). Метод потока – статический метод.
   // •	Массив1 – int, 12 элементов
    public class Arr1
    {
        public int[] Array { get; set; }

        public Arr1()
        {
            Random rnd = new Random();
            // Заполнение массива случайными числами
            //С помощью статических, не расширяющих методов класса Enumerable
            Array = Enumerable
                .Repeat(0, 12)
                .Select(x => rnd.Next(-50, 50 + 1))
                .ToArray();
        }//Arr1()

        public void ShowArr()
        {
            List<int> listArray = new List<int>(Array);
            listArray.ForEach(item => Console.Write($"{item}  "));
            Console.WriteLine();

        }//ShowArr()

        //Статический метод для вызова в дочернем потоке который принимает данные
        public static void Run(object a )
        {
            int[] arr = (int[])a;
            Console.WriteLine("Массив случайных чисел:");
            List<int> listArray = new List<int>(arr);
            listArray.ForEach(item => Console.Write($"{item}  "));
            Console.WriteLine();
            Thread.Sleep(2000);
            //сумма всех элементов массива
            int sum = arr.Aggregate((x, y) => x + y);
            Console.WriteLine($"Cумма элементов массива: {sum}");
            Console.WriteLine();
            //  задержка потока 2 сек:
            Thread.Sleep(2000);
        }//Run()

    }//Arr1
}//Task_1
