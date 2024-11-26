namespace EducaPro.Models
{
    public class CursoInscripcionViewModel
    {
        public int InscripcionId { get; set; }
        public int CursoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string HorarioSeleccionado { get; set; } // Campo editable para la vista
    }
}
