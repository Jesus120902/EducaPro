using EducaPro.Services;
using Microsoft.AspNetCore.Mvc;

namespace EducaPro.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly IChatbotService _chatbotService;

        public ChatbotController(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetResponse(string mensaje)
        {
            string respuesta = _chatbotService.ProcesarMensaje(mensaje.ToLower());
            return Json(new { respuesta });
        }
    }
}
