using System.ComponentModel.DataAnnotations;

namespace Models {
    public class Airplane {
        #region fileds
        /// <summary>
        /// ID самолета
        /// </summary>
        [Key]
        [MaxLength(450)]
        public string Id { get; set; }

        /// <summary>
        /// Название самолета
        /// </summary>
        [MaxLength(450)]
        public string Name { get; set; }

        /// <summary>
        /// Количество мест в самолете
        /// </summary>
        public int Places { get; set; }

        /// <summary>
        /// Комания производитель
        /// </summary>
        [MaxLength(450)]
        public string Creator { get; set; }
        #endregion

        #region constructors
        public Airplane() {

        }
        
        public Airplane(string Id, string Name, int Places, string Creator) {
            this.Id = Id;
            this.Name = Name;
            this.Places = Places;
            this.Creator = Creator;
        }
        
        #endregion
    }
}