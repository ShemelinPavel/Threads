using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NumbersContainer;

namespace Threads
{
    class Program
    {
        /// <summary>
        /// верх. граница факториала
        /// </summary>
        private static ulong f_limit;
        
        /// <summary>
        /// результат расчета факториала
        /// </summary>
        private static ulong f_result = 1;

        /// <summary>
        /// массив хранилищ с цифрами разбитые для потоков
        /// </summary>
        private static Container[] factorialParts;

        static void Main ( string[] args )
        {
            Console.Write ( "Введите положительное число для расчета факториала: " );
            string f_limitAsString = Console.ReadLine ();

            bool result = ulong.TryParse ( f_limitAsString, out f_limit );

            if (!(result))
            {
                Console.WriteLine ( "Ошибка ввода числа!" );
                Console.WriteLine ( "Для выхода нажмите любую клавишу." );
                Console.ReadKey ();
                return;
            }
            else if (f_limit <= 0)
            {
                Console.WriteLine ( "Число должно быть положительным и больше нуля!" );
                Console.WriteLine ( "Для выхода нажмите любую клавишу." );
                Console.ReadKey ();
                return;
            }

            #region вычисление факториала без потоков

            //время начала расчета
            DateTime dateStart = DateTime.Now;

            for (ulong i = 1; i <= f_limit; i++)
            {
                f_result *= i;
            }

            //время конца расчета
            DateTime dateStop = DateTime.Now;

            TimeSpan time = dateStop - dateStart;

            Console.WriteLine ( $"Результат {f_result}. Время работы: {time}" );

            #endregion вычисление факториала без потоков

            #region вычисление факториала c потоками

            f_result = 1;

            // количество потоков
            const ushort threadsNumber = 5;

            Thread[] f_threads = new Thread[threadsNumber];
            factorialParts = new Container[threadsNumber];

            //вместимость одного хранилища исходя из количества потоков
            ulong capacityList = f_limit / threadsNumber;

            //в последний поток добавляем все хвосты
            ulong capacityLastList = capacityList + f_limit % threadsNumber;

            //счетчик для генерации цифр
            ulong counter = 1;


            //заполнение хранилищ для работы потоков
            for (ushort i = 0; i < threadsNumber; i++)
            {
                bool lastList = i == threadsNumber - 1;

                Container curFactorialPart = new Container ();
                curFactorialPart.Fill ( (lastList ? capacityLastList : capacityList), ref counter );

                factorialParts[i] = curFactorialPart;
            }

            //время начала расчета
            dateStart = DateTime.Now;

            //активация потоков
            for (ushort i = 0; i < threadsNumber; i++)
            {
                Thread cur_thread = new Thread ( new ParameterizedThreadStart ( CalculateFactorialPart ) );
                cur_thread.Start ( i );

                f_threads[i] = cur_thread;
            }

            //ждем пока потоки завершатся
            bool exitFlag = false;

            while (!(exitFlag))
            {
                foreach (Thread item in f_threads)
                {
                    if (item.IsAlive)
                    {
                        exitFlag = false;
                        break;
                    }
                    else
                    {
                        exitFlag = true;
                    }
                }
            }

            for (ushort i = 0; i < threadsNumber; i++)
            {
                f_result *= factorialParts[i].Result;
            }

            //время конца расчета
            dateStop = DateTime.Now;

            time = dateStop - dateStart;

            Console.WriteLine ( $"Результат {f_result}. Время работы: {time}" );

            #endregion вычисление факториала c потоками

            Console.WriteLine ( "Для выхода нажмите любую клавишу." );
            Console.ReadKey ();

            return;
        }

        /// <summary>
        /// метод считающий часть факториала из цифр одного из хранилища
        /// </summary>
        /// <param name="threadNumber">индекс хранилища</param>
        public static void CalculateFactorialPart ( object threadNumber )
        {

            Container curFactorialPart = factorialParts[(ushort)threadNumber];
            curFactorialPart.GetMulti ();
        }
    }
}