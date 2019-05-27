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
        /// верх. граница суммы чисел
        /// </summary>
        private static ulong s_limit;

        /// <summary>
        /// результат расчета суммы чисел
        /// </summary>
        private static ulong s_result = 0;

        /// <summary>
        /// массив хранилищ с цифрами разбитые для потоков
        /// </summary>
        private static Container[] summaParts;

        static void Main ( string[] args )
        {
            Console.Write ( "Введите положительное число для расчета суммы чисел: " );
            string s_limitAsString = Console.ReadLine ();

            bool result = ulong.TryParse ( s_limitAsString, out s_limit );

            if (!(result))
            {
                Console.WriteLine ( "Ошибка ввода числа!" );
                Console.WriteLine ( "Для выхода нажмите любую клавишу." );
                Console.ReadKey ();
                return;
            }
            else if(s_limit <= 0)
            {
                Console.WriteLine ( "Число должно быть положительным и больше нуля!" );
                Console.WriteLine ( "Для выхода нажмите любую клавишу." );
                Console.ReadKey ();
                return;
            }

            #region вычисление cуммы без потоков

            //время начала расчета
            DateTime dateStart = DateTime.Now;

            for (ulong i = 1; i <= s_limit; i++)
            {
                s_result += i;
            }

            //время конца расчета
            DateTime dateStop = DateTime.Now;

            TimeSpan time = dateStop - dateStart;

            Console.WriteLine ( $"Результат {s_result}. Время работы: {time}" );

            #endregion вычисление суммы чисел без потоков

            #region вычисление суммы чисел c потоками

            s_result = 0;

            // количество потоков
            const ushort threadsNumber = 5;

            Thread[] s_threads = new Thread[threadsNumber];
            summaParts = new Container[threadsNumber];

            //вместимость одного хранилища исходя из количества потоков
            ulong capacityList = s_limit / threadsNumber;

            //в последний поток добавляем все хвосты
            ulong capacityLastList = capacityList + s_limit % threadsNumber;

            //счетчик для генерации цифр
            ulong counter = 1;

            //заполнение хранилищ для работы потоков
            for (ushort i = 0; i < threadsNumber; i++)
            {
                bool lastList = i == threadsNumber - 1;
                Container summaPart = new Container ();

                summaPart.Fill ( lastList ? capacityLastList : capacityList, ref counter );

                summaParts[i] = summaPart;
            }

            //время начала расчета
            dateStart = DateTime.Now;

            //активация потоков
            for (ushort i = 0; i < threadsNumber; i++)
            {
                Thread cur_thread = new Thread ( new ParameterizedThreadStart ( CalculateSummaPart ) );
                cur_thread.Start ( i );

                s_threads[i] = cur_thread;
            }

            //ждем пока потоки завершатся
            bool exitFlag = false;

            while (!(exitFlag))
            {
                foreach (Thread item in s_threads)
                {
                    if(item.IsAlive)
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
                s_result += summaParts[i].Result;
            }

            //время конца расчета
            dateStop = DateTime.Now;

            time = dateStop - dateStart;

            Console.WriteLine ( $"Результат {s_result}. Время работы: {time}" );

            #endregion вычисление суммы чисел c потоками

            Console.WriteLine ( "Для выхода нажмите любую клавишу." );
            Console.ReadKey ();

            return;
        }

        /// <summary>
        /// метод считающий часть факториала из цифр одного из хранилища
        /// </summary>
        /// <param name="threadNumber">индекс хранилища</param>
        public static void CalculateSummaPart ( object threadNumber )
        {

            Container curSummaPart = summaParts[(ushort)threadNumber];
            curSummaPart.GetSumm ();
        }
    }
}