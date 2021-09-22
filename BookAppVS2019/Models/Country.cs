using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppVS2019.Models
{
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //Se cambia de Identity a None por errores con Author
        public int Id { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Country Name cannot be more than 50 characters")]
        public string Name { get; set; }
        public virtual ICollection<Author> Authors { get; set; } //Creandola de manera virtual, permita Lazy Load, permitiendo a Entity Framework, generando la instancia de esta coleccion
                                                                //solo cuando es necesaria
    }
}
