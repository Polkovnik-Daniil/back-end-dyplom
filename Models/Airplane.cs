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
        public Airplane(string ID, string name, int places, string creator) {
            this.ID = ID;
            this.Name = name;
            this.Places = places;
            this.Creator = creator;
        }
        #endregion
    }
}