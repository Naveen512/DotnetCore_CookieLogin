using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CookieAuth.Web.Controllers
{
    [Route("pages")]
    public class PagesController : Controller
    {
        [Route("admin")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View();
        }

        [Route("viewer")]
        [HttpGet]
        [Authorize(Roles = "Admin, Viewer")]
        public IActionResult Viewer()
        {
            return View();
        }

        [Route("guest")]
        [HttpGet]
        public IActionResult Guest()
        {
            return View();
        }
    }
}