using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models {
    public class Ticket {
        #region fields
        /// <summary>
        /// ID билета(UUID)
        /// </summary>
        [Key]
        [MaxLength(450)]
        public string Id { get; set; }

        public Flight Flight { get; set; }
        
        /// <summary>
        /// Класс об человеке
        /// </summary>
        public Man Man { get; set; }

        /// <summary>
        /// Почта
        /// </summary>
        [MaxLength(450)]
        public string? Email { get; set; }
        #endregion

        #region constructors
        public Ticket() {

        }

        public Ticket(string Id, Flight Flight, Man Man, string? Email) {
            this.Id = Id;
            this.Flight = Flight;
            this.Man = Man;
            this.Email = Email;
        }
        #endregion
    }
}
