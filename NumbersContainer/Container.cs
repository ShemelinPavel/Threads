using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumbersContainer
{
    /// <summary>
    /// контейнер чисел
    /// </summary>
    public class Container
    {
        private ulong res;


        /// <summary>
        /// результат расчета части факториала
        /// </summary>
        public ulong Result => res;

        /// <summary>
        /// цифры для расчета факториала
        /// </summary>
        public List<ulong> NumbersList { get; set; }

        /// <summary>
        /// сумма чисел в контейнере
        /// </summary>
        /// <returns>сумма</returns>
        public void GetSumm ()
        {
            ulong tmpresult = 0;
            foreach (ulong item in NumbersList)
            {
                tmpresult += item;
            }

            res = tmpresult;
        }

        /// <summary>
        /// произведение чисел
        /// </summary>
        /// <returns>произведение</returns>
        public void GetMulti ()
        {
            ulong tmpresult = 1;
            foreach (ulong item in NumbersList)
            {
                tmpresult *= item;
            }

            res = tmpresult;
        }

        /// <summary>
        /// заполнение контейнера
        /// </summary>
        /// <param name="capacity">емкость</param>
        /// <param name="counter">начальное значение</param>
        public void Fill ( ulong capacity, ref ulong counter )
        {
            for (ulong i = 0; i < capacity; i++)
            {
                NumbersList.Add ( counter );
                counter++;
            }

            res = 0;
        }

        public Container ()
        {
            NumbersList = new List<ulong> ();
            res = 0;
        }
    }
}