using Microsoft.AspNetCore.Mvc;

namespace Sample.AspnetMvc.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return "Hello, World!";
        }
    }
}