using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models {
    public class Flight {
        #region fileds
        [Key]
        [MaxLength(450)]
        public string Id { get; set; }

        /// <summary>
        /// Класс самолета относящегося к текущему рейсу
        /// </summary>
        public Airplane Airplane { get; set; }

        /// <summary>
        /// Время прибытия
        /// </summary>
        public DateTime DepartureTime { get; set; }

        /// <summary>
        /// Время отбытия
        /// </summary>
        public DateTime ArrivalTime { get; set; }

        /// <summary>
        /// Место отбытия(город)
        /// </summary>
        [MaxLength(450)]
        public string DeparturePoint { get; set; }

        /// <summary>
        /// Место отбытия(навзвание Аэропорта)
        /// </summary>
        [MaxLength(450)]
        public string DepartureAirport { get; set; }

        /// <summary>
        /// Точка отбытия
        /// </summary>
        [MaxLength(450)]
        public string ArrivalPoint { get; set; }

        /// <summary>
        /// Точка прибытия
        /// </summary>
        [MaxLength(450)]
        public string ArrivalAirport { get; set; }

        /// <summary>
        /// Статус рейса
        /// </summary>
        [MaxLength(450)]
        public char Status { get; set; }

        /// <summary>
        /// Количество свободных мест
        /// </summary>
        public int FreePlaces { get; set; }
        #endregion

        #region constructors
        public Flight() {

        }

        /// <summary>
        /// Конструктор :D
        /// </summary>
        /// <param name="ID">ID рейса</param>
        /// <param name="Airplane">Самолет текущего рейса</param>
        /// <param name="DepartureTime">Время отбытия</param>
        /// <param name="ArrivalTime">Время прибытия</param>
        /// <param name="DeparturePoint">Место отбытия</param>
        /// <param name="DepartureAirport">Аэропорт отбытия</param>
        /// <param name="ArrivalPoint">Место прибытия</param>
        /// <param name="ArrivalAirport">Аэропорт прибытия</param>
        /// <param name="status">Статус</param>
        /// <param name="FreePlaces">количество свободных мест</param>
        public Flight(string ID, Airplane Airplane, DateTime DepartureTime, DateTime ArrivalTime, string DeparturePoint, string DepartureAirport, string ArrivalPoint, string ArrivalAirport, char status, int FreePlaces) {
            this.ID = ID;
            this.Airplane = Airplane;
            this.DepartureTime = DepartureTime;
            this.ArrivalTime = ArrivalTime;
            this.DeparturePoint = DeparturePoint;
            this.DepartureAirport = DepartureAirport;
            this.ArrivalPoint = ArrivalPoint;
            this.ArrivalAirport = ArrivalAirport;
            this.Status = status;
            this.FreePlaces = FreePlaces;
        }
        #endregion
    }
}
