using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EducaPro.Models
{
    public class Carrito
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } // Relaciona el carrito con el usuario

        public List<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}
