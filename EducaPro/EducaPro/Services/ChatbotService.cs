using System.Collections.Generic;

namespace EducaPro.Services
{
    public class ChatbotService : IChatbotService
    {
        private static readonly Dictionary<string, string> Respuestas = new()
        {
            {
                "curso",
                "Estos son algunos de los cursos que ofrecemos:\n\n" +
                "- **Matemáticas Avanzadas**: Cálculo diferencial e integral. Profesor: Profesor A\n" +
                "- **Física Moderna**: Teorías físicas como relatividad y mecánica cuántica. Profesor: Profesor B\n" +
                "- **Historia Universal**: Estudio de eventos históricos clave. Profesor: Profesor C\n" +
                "- **Programación Básica**: Introducción a la programación. Profesor: Profesor D\n" +
                "- **Diseño Gráfico**: Herramientas y principios del diseño gráfico. Profesor: Profesor E\n" +
                "\n¡Dime si te interesa más información sobre algún curso!"
            },
            {
                "horario",
                "Ofrecemos horarios flexibles para que elijas el que más te convenga:\n" +
                "- **Mañana**: 8:00 AM - 12:00 PM\n" +
                "- **Tarde**: 2:00 PM - 6:00 PM\n" +
                "- **Noche**: 6:00 PM - 10:00 PM\n" +
                "\nPuedes seleccionar el horario que prefieras según tu disponibilidad."
            },
            {
                "inscripcion",
                "Para inscribirte en cualquier curso, sigue estos pasos:\n" +
                "1. Presentar tu documento de identidad\n" +
                "2. Completar el formulario de inscripción\n" +
                "3. Realizar el pago mediante **transferencia bancaria** o **tarjeta de débito/crédito**\n" +
                "\nCon estos pasos podrás comenzar tu curso."
            },
            {
                "profesor",
                "Nuestros profesores tienen una amplia experiencia en sus campos:\n" +
                "- **Profesor A**: Experto en cálculo avanzado.\n" +
                "- **Profesor B**: Especialista en física moderna.\n" +
                "- **Profesor C**: Historiador con años de experiencia.\n" +
                "- **Profesor D**: Programador con experiencia en varios lenguajes.\n" +
                "- **Profesor E**: Diseñador gráfico con una carrera profesional sólida.\n" +
                "\nTodos tienen más de 5 años de experiencia en la enseñanza."
            },
            {
                "matricula",
                "El proceso de matrícula es simple:\n" +
                "1. Completa el formulario de inscripción.\n" +
                "2. Realiza el pago por tarjeta o transferencia.\n" +
                "3. Recibirás una confirmación por correo con los detalles del curso.\n" +
                "\n¡Es muy fácil inscribirse!"
            }
        };

        public string ProcesarMensaje(string mensaje)
        {
            foreach (var respuesta in Respuestas)
            {
                if (mensaje.Contains(respuesta.Key))
                {
                    return respuesta.Value;
                }
            }

            return "No entiendo tu pregunta. Puedes preguntarme sobre cursos, precios, horarios, inscripción o profesores.";
        }
    }
}
