using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Task_2.Models
{
    static class Utils
    {
        // объект для получения случайных чисел
        public static readonly Random Random = new Random(Environment.TickCount);

        // вывести строку в заданную позицию
        public static void WritePos(int left, int top, string str) {
            Console.SetCursorPosition(left, top);
            Console.Write(str);
        } // WritePos

        // пример краткой формы записи метода
        public static void SetPos(int left, int top) =>
            Console.SetCursorPosition(left, top);
        
        // Вычисление наибольшего общего делителя
        public static int Gcd(int a, int b) {
            return b == 0 ? a : Gcd(b, a % b);
        } // Gcd

        // Получение случайного числа
        // краткая форма записи метода - это не лямбда выражение
        public static int GetRand(int lo, int hi) => Random.Next(lo, hi + 1);
        public static double GetRand(double lo, double hi) => lo + (hi - lo)*Random.NextDouble();
        public static char GetRand(char min = (char)0, char max = (char)255) =>
            (char)Random.Next(min, max + 1);

        public static int GetRand(int hi) => Random.Next(hi + 1); // генерация int

        // сгенерировать фамилию
        public static string GenerateSurname()
        {
            string[] surnames = {
                "Андрусейко", "Гущин", "Корнейчук", "Князев", "Кононов",
                "Кабанов", "Лапин", "Кондратьев", "Кудрявцев", "Пахомов",
                "Палий", "Щукин", "Овчинников", "Мамонтов", "Кузьмин"
            };

            return surnames[GetRand(0, surnames.Length - 1)];
        }

        // сгенерировать имя
        public static string GenerateName()
        {
            // имена, фамилии и отчества для генератора ФИО
            string[] names = {
                "Иван", "Кирилл", "Фёдор", "Архип", "Назар",
                "Спартак", "Донат", "Тагил", "Лука", "Гавриил",
                "Елисей", "Степан", "Милан", "Геннадий", "Ростислав"
            };

            return names[GetRand(0, names.Length - 1)];
        }

        // сгенерировать город
        public static string GenerateCity()
        {
            // города
            string[] cities = {
                "Донецк", "Москва", "Киев", "Сочи", "Харьков",
                "Львов", "Лондон", "Анапа", "Томск", "Липецк",
                "Глазов", "Владимир", "Курган", "Шахты", "Котлас"
            };

            return cities[GetRand(0, cities.Length - 1)];
        }

        // сгенерировать отчество
        public static string GeneratePatronymic()
        {
            string[] patronymics = {
                "Владимирович", "Фёдорович", "Максимович", "Богданович", "Васильевич",
                "Борисович", "Максимович", "Станиславович", "Романович", "Петрович",
                "Алексеевич", "Григорьевич", "Данилович", "Брониславович", "Юхимович"
            };

            return patronymics[GetRand(0, patronymics.Length - 1)];
        }

        public static string GeneratePassport()
        {
            string[] passport = {
                "EO847290", "E1367900", "EO098977", "E0988666", "EO847250",
                "E0098755", "E8766543", "EO849876", "EO847290", "EO849776",

            };

            return passport[GetRand(0, passport.Length - 1)];
        }



        // сгенерировать ФИО
        public static string GenerateSNP() => $"{GenerateSurname()} {GenerateName()} {GeneratePatronymic()}";
        //иницыалы
        public static string GenerateSNPInitials() => $"{GenerateSurname()} {GenerateName()[0]}. {GeneratePatronymic()[0]}.";
        // получение случайной даты
        public static DateTime GenRandomDateTime(DateTime from, DateTime to, Random random = null)
        {
            if (from >= to)
            {
                throw new Exception("Параметр \"from\" должен быть меньше параметра \"to\"!");
            }
            if (random == null)
            {
                random = new Random();
            }
            TimeSpan range = to - from;
            var randts = new TimeSpan((long)(random.NextDouble() * range.Ticks));
            return from + randts;
        } // GenRandomDateTime

        // получение пункта отправления и пунка назначения
        public static (string, string) GetRandDestinationDepartureItems()
        {
            (string, string)[] destinationsDepartures = {
                ("Москва","Санкт-Петербург"),
                ("Москва","Санкт-Петербург"),
                ("Казань","Нижний Новгород"),
                ("Казань","Нижний Новгород"),
                ("Минск","Воронеж"),
                ("Минск","Воронеж"),
                ("Екатеринбург","Адлер"),
                ("Екатеринбург","Адлер")
            };

            return destinationsDepartures[GetRand(0, destinationsDepartures.Length - 1)];
        } // GetRandItems



        // вывод сообщения о разработке метода
        public static void UnderConstruction() {
            SetColor(ConsoleColor.Yellow, ConsoleColor.DarkYellow);
                        
            WritePos(8, 3, "┌──────────────────────────────────┐");
            WritePos(8, 4, "│    [К сведению]                  │");
            WritePos(8, 5, "│                                  │");
            WritePos(8, 6, "│    Метод в разработке            │");
            WritePos(8, 7, "│                                  │");
            WritePos(8, 8, "└──────────────────────────────────┘");
                        
            RestoreColor();
            Console.Write("\n\n\n\n\n");
        } // UnderConstruction

        // Установить текущий цвет символов и фона с сохранением
        // текущего цвета символов и фона
        private static (ConsoleColor Fore,  ConsoleColor Back) _storeColor;
        public static void SetColor(ConsoleColor fore, ConsoleColor back) {
            _storeColor = (Console.ForegroundColor, Console.BackgroundColor);
            Console.ForegroundColor = fore;
            Console.BackgroundColor = back;
        } // SetColor

        // Сохранить цвет
        public static void SaveColor() =>
            _storeColor = (Console.ForegroundColor, Console.BackgroundColor);

        // Восстановить сохраненный цвет
        public static void RestoreColor() =>
            (Console.ForegroundColor, Console.BackgroundColor) = _storeColor;


      /*  public static T GetRand<T>(T low, T high)
        {
            if (typeof(T) == typeof(double))
            {
                return (T)(object)GetRand((double)(object)low, (double)(object)high);
            } // if

            if (typeof(T) == typeof(int))
            {
                return (T)(object)GetRand((int)(object)low, (int)(object)high);
            } // if

            return (T)(object)GetRand((char)(object)low, (char)(object)high);
        } // GetRand*/

        // Пример чтения из текстового файла
        static void ReadText(string fileName)
        {
            // Класс StreamReader - чтение из текстовых файлов
            // File.OpenRead(fileName) - возвращает поток для чтения
            // Encoding.Default - кодировка текста в файле - Windows CP1251 (для 
            // русской локали Windows)
            StreamReader sr = new StreamReader(File.OpenRead(fileName), Encoding.Default);

            // string str = sr.ReadToEnd();
            // sr.Close();
            // 
            // Console.WriteLine(str);
            // Console.ReadKey();

            string str;
            while (!sr.EndOfStream)
            {       // пока не достигнут конец файла
                str = sr.ReadLine();       // читать строку
                Console.WriteLine(str);
            } // while
            sr.Close();                    // закрыть поток/файл ввода
        } // ReadText

       public static void ViewFile(string title, string fileName)
        {
            Console.WriteLine($"{title} {fileName}\n");
            using (StreamReader sr = new StreamReader(fileName, Encoding.UTF8))
                while (!sr.EndOfStream)
                    Console.WriteLine($"\t{sr.ReadLine()}");
                    Console.WriteLine("");
        } // ViewFile

        //Максимальная длинна слова вмасиве слов
        public static string MaxLenth(string[] str)
        {
            string wordMax = string.Empty;
           foreach (var item in str)
            {
                if (item.Length > wordMax.Length) wordMax = item;
            }//foreach
            return wordMax;

        }//MaxLenth

        public static string MinLenth(string[] str)
        {
            string wordMin = str[0];
            foreach (var item in str)
            {
                if (item.Length < wordMin.Length) wordMin = item;
            }//foreach
            return wordMin;

        }//MaxLenth


        // Подготовка строки к записи в файл - копировать символы
        // строки str в буферный массив байт для вывода
        public static byte[] ToArray(string str, int length)
        {
            byte[] buf = new byte[length];
            Encoding.UTF8
                .GetBytes(str)
                .CopyTo(buf, 0);

            return buf;
        } // ToArray

        //показать текст с выделенным цветом найденных слов
        public static void ShowColoredMaxMinWords(string[] str)
        {
            SaveColor();
            foreach (var item in str)
            {
                if (item.Length == MinLenth(str).Length) Console.ForegroundColor = ConsoleColor.Red;
                else if (item.Length == MaxLenth(str).Length) Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{item} ");
                RestoreColor();
            }
            Console.WriteLine("\n");

        }//ShowColoredMaxMinWords


    } // class Utils

}//HW13_Shvaiko
