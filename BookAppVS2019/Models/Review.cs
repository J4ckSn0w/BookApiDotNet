using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(200,MinimumLength = 10, ErrorMessage = "The Headline must be between 10 an 200 characters ")]
        public string Headline { get; set; }
        [Required]
        [StringLength(2000,MinimumLength = 50, ErrorMessage = "Review Text mus be between 50 and 2000 characters")]
        public string ReviewText { get; set; }
        [Required]
        [Range(1,5,ErrorMessage = "Rating must be between 1 and 5 stars")]
        public int Rating { get; set; }
        public virtual Reviewer Reviewer { get; set; }
        public virtual Book Book { get; set; }
    }
}
