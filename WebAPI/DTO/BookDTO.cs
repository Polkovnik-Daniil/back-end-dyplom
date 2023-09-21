using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class BookDTO
    {
        public string Title { get; set; }

        public DateTime? Realise { get; set; }

        public int? NumberOfBooks { get; set; }

        public int? NumberOfPage { get; set; }
    }
}
