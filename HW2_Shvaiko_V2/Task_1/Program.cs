using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_1
{
   // . Создать консольное приложение с использованием потоков, таймера.
   // Приложение при помощи таймера каждые 5 секунд выполняет очистку экрана консоли,
   // создание и запуск трех потоков, обрабатывающих три массива, созданных вне методов потоков.
   // Завершение приложения – по клавише F10, ожидать завершения потоков в главном потоке.
   // Массивы для обработки:
   // •	Массив1 – int, 12 элементов
   // •	Массив2 – double, 21 элемент
   // •	Массив3 – int, 19 элементов

   //1.	Первый поток заполняет массив1 случайными целыми числами,
   //вычисляет сумму элементов этого массива, сумму и элементы массива
   //выводит в консоль(при помощи StringBuilder). Метод потока – статический метод.



    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                
              Console.WriteLine($"Main: id потока равен {Thread.CurrentThread.ManagedThreadId}");

            
            
              Arr1 arr1 = new Arr1();
              Arr2 arr2 = new Arr2();
              Arr3 arr3 = new Arr3();

              //Создать объект делегата TimerCallback и связать сним метод
              TimerCallback callback = new
                TimerCallback(TimerMethod);
           
              // связываем статический метод с делегатом
              ParameterizedThreadStart threadstart =
                new ParameterizedThreadStart(Arr1.Run);

              //объект Timer и передать конструктор - делегат
              Timer timer = new Timer(callback);

               timer.Change(3000, 5000);

               // создаем объект потока 1
               Thread thread1 = new Thread(threadstart);
               // создаем объект потока 2
                Thread thread2 = new Thread(arr2.ShowArr);
                //создаем объект потока 3
                Thread thread3 = new Thread(()=>{arr3.ShowArr();});

                //запускаем  дочерний поток
                 thread1.Start((object)arr1.Array);
                Console.WriteLine();
                //запускаем  дочерний поток
                thread2.Start();
                thread3.Start();
                thread1.Join();
                thread2.Join();
                thread3.Join();

            } while (Console.ReadKey().Key != ConsoleKey.F10);


        }//Main

        //метод, который будет выполняться по истечении определенного периода времени.
        public static void TimerMethod(object a)
        {
            Console.Clear();
        }//TimerMethod

    }//Program
}//Task_1
