using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OrderFood.Web.Models;
using OrderFood.Web.Services;

namespace OrderFood.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class BaseController : Controller
    {
        public BaseController(WebBaseManager webBaseManager)
        {
            WebBaseManager = webBaseManager;
        }

        public WebBaseManager WebBaseManager { get; }

        public void SetFlash(FlashMessageType type, string message)
        {
            var msg = new FlashMessage
            {
                Type = type.ToString().ToLower(),
                Message = message
            };

            TempData["Message"] = JsonSerializer.Serialize(msg);
        }
    }
}
