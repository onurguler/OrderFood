using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OrderFood.Web.Models;

namespace OrderFood.Web.Controllers
{

    public class BaseController : Controller
    {
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
