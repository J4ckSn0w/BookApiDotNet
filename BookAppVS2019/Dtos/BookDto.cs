using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Dtos
{
    public class BookDto
    {
        public int bookId { get; set; }
        public string Isbn { get; set; }
        public string title { get; set; }
        public DateTime? datePublished { get; set; }
    }
}
