using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_1
{
    //3.	Третий поток заполняет массив случайными целыми числами,
    //упорядочивает массив по правилу - нечетные значения в начале массива,
    //четные – в конец массива, выводит в консоль(при помощи StringBuilder)
    //массив до и после упорядочивания.Метод потока – лямбда-выражение.
   // •	Массив3 – int, 19 элементов
    public class Arr3
    {

        public int[] Array { get; set; }

        public Arr3()
        {
            Random rnd = new Random();
            // Заполнение массива случайными числами
            //С помощью статических, не расширяющих методов класса Enumerable
            Array = Enumerable
                .Repeat(0, 19)
                .Select(x => rnd.Next(-50, 50 + 1))
                .ToArray();
        }//Arr3()

        public void ShowArr()
        {
            Thread.Sleep(10000);
            Console.WriteLine("Массив до сортировки:\n");
            List<int> listArray = new List<int>(Array);
            listArray.ForEach(item => Console.Write($"{item}  "));
            Console.WriteLine();
            Console.WriteLine("Массив после сортировки:\n");
            listArray = new List<int>(Array.OrderBy(x => x % 2 == 0));
            listArray.ForEach(item => Console.Write($"{item}  "));
            Console.WriteLine();
         }//ShowArr()
        
    }//Arr3
}//Task_1
