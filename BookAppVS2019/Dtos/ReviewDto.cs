using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Dtos
{
    public class ReviewDto
    {
        public int reviewId { get; set; }
        public string headline { get; set; }
        public string reviwerText { get; set; }
        public int rating { get; set; }
    }
}
