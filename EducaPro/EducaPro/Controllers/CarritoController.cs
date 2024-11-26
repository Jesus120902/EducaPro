using EducaPro.Data;
using EducaPro.Models;
using EducaPro.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EducaPro.Controllers
{
    [Authorize(Roles = "Estudiante")]
    public class CarritoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public CarritoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // Muestra el carrito del usuario
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var carrito = await _context.Carritos
                .Include(c => c.Inscripciones)
                .ThenInclude(i => i.Curso)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (carrito == null)
            {
                carrito = new Carrito { UserId = userId };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }

            return View(carrito);
        }

        // Agregar un curso al carrito
        [HttpPost]
        public async Task<IActionResult> AddCurso(int cursoId)
        {
            var userId = _userManager.GetUserId(User);
            var carrito = await _context.Carritos
                .Include(c => c.Inscripciones)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (carrito == null)
            {
                carrito = new Carrito { UserId = userId };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }

            // Verificar si el curso ya está en el carrito antes de agregarlo
            if (!carrito.Inscripciones.Any(i => i.CursoId == cursoId))
            {
                carrito.Inscripciones.Add(new Inscripcion
                {
                    CursoId = cursoId,
                    HorarioSeleccionado = "No asignado",
                    FechaInscripcion = DateTime.Now
                });
                await _context.SaveChangesAsync();
                TempData["MensajeExito"] = "Curso agregado al carrito con éxito.";
            }
            else
            {
                TempData["MensajeError"] = "Este curso ya está en tu carrito.";
            }

            return RedirectToAction("Index");
        }

        // Vista de inscripción para confirmar cursos
        public async Task<IActionResult> Inscripcion()
        {
            var userId = _userManager.GetUserId(User);
            var carrito = await _context.Carritos
                .Include(c => c.Inscripciones)
                .ThenInclude(i => i.Curso)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (carrito == null || !carrito.Inscripciones.Any())
            {
                TempData["MensajeError"] = "Tu carrito está vacío.";
                return RedirectToAction("Index");
            }

            return View(carrito);
        }

        // POST: Realizar inscripción
        [HttpPost]
        public async Task<IActionResult> RealizarInscripcion(int carritoId, string metodoPago, string horarioSeleccionado)
        {
            var carrito = await _context.Carritos
                .Include(c => c.Inscripciones)
                .ThenInclude(i => i.Curso)
                .FirstOrDefaultAsync(c => c.Id == carritoId);

            if (carrito == null || !carrito.Inscripciones.Any())
            {
                TempData["MensajeError"] = "No se encontró el carrito o está vacío.";
                return RedirectToAction("Index");
            }

            // Procesar las inscripciones
            foreach (var inscripcion in carrito.Inscripciones)
            {
                inscripcion.HorarioSeleccionado = horarioSeleccionado;
                inscripcion.FechaInscripcion = DateTime.Now;
            }

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Obtener el usuario
            var userId = _userManager.GetUserId(User);
            var usuario = await _userManager.FindByIdAsync(userId);

            if (usuario == null)
            {
                TempData["MensajeError"] = "No se pudo obtener la información del usuario.";
                return RedirectToAction("Index");
            }

            // Enviar el correo de confirmación
            var emailContent = "Hola " + usuario.UserName + ",\n\n" +
    "Te confirmamos que te has inscrito satisfactoriamente en los siguientes cursos:\n\n" +
    string.Join("\n", carrito.Inscripciones.Select(i => $"- {i.Curso.Nombre} (Horario: {i.HorarioSeleccionado})")) +
    "\n\nGracias por confiar en EducaPro.";

            try
            {
                await _emailSender.SendEmailAsync(usuario.Email, "Confirmación de inscripción", emailContent);
                TempData["MensajeExito"] = "Inscripción realizada con éxito. Se ha enviado un correo de confirmación.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Inscripción realizada, pero hubo un problema al enviar el correo. " + ex.Message;
            }

            return RedirectToAction("MisCursos");
        }

        // Muestra los cursos inscritos del usuario
        public async Task<IActionResult> MisCursos()
        {
            var userId = _userManager.GetUserId(User);
            var inscripciones = await _context.Inscripciones
                .Include(i => i.Curso)
                .Where(i => i.Carrito.UserId == userId)
                .ToListAsync();

            if (inscripciones == null || !inscripciones.Any())
            {
                ViewBag.Mensaje = "No tienes cursos registrados.";
                return View();
            }

            return View(inscripciones);
        }
        [HttpPost]
        public async Task<IActionResult> EliminarCurso(int inscripcionId)
        {
            var inscripcion = await _context.Inscripciones.FirstOrDefaultAsync(i => i.Id == inscripcionId);

            if (inscripcion != null)
            {
                _context.Inscripciones.Remove(inscripcion);
                await _context.SaveChangesAsync();
                TempData["MensajeExito"] = "Curso eliminado del carrito con éxito.";
            }
            else
            {
                TempData["MensajeError"] = "No se pudo eliminar el curso. Intenta de nuevo.";
            }

            return RedirectToAction("Index");
        }

    }
}
