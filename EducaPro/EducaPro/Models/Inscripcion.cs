using System;
using System.ComponentModel.DataAnnotations;

namespace EducaPro.Models
{
    public class Inscripcion
    {
        public int Id { get; set; }
        public int CursoId { get; set; }
        public Curso Curso { get; set; }
        public int CarritoId { get; set; }
        public Carrito Carrito { get; set; }

        [Display(Name = "Horario Seleccionado")]
        public string HorarioSeleccionado { get; set; } = "No asignado"; // Valor predeterminado

        [Display(Name = "Fecha de Inscripción")]
        public DateTime FechaInscripcion { get; set; } = DateTime.Now;
    }

}
