using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models {
    public class Man {
        #region fileds
        /// <summary>
        /// ID человека(UUID)
        /// </summary>
        [Key]
        [MaxLength(450)]
        public string Id { get; set; }
        
        /// <summary>
        /// Паспортный номер
        /// </summary>
        [MaxLength(450)]
        public string PassportNumber { get; set; }   
        
        /// <summary>
        /// Имя человека
        /// </summary>
        [MaxLength(450)]
        public string Name { get; set; }            
        
        /// <summary>
        /// Фамилия человека
        /// </summary>
        [MaxLength(450)]
        public string Surname { get; set; }
        
        /// <summary>
        /// Удоли это говно
        /// </summary>
        public char Sex { get; set; }
        #endregion

        #region contructors
        public Man() {

        }

        public Man(string Id, string PassportNumber, string Name, string Surname, char Sex) {
            this.Id = Id;
            this.PassportNumber = PassportNumber;
            this.Name = Name;
            this.Surname = Surname;
            this.Sex = Sex;
        }
        #endregion
    }
}
