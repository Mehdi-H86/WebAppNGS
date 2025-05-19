using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAppNGS.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userName = User.Identity.Name ?? "inconnu";
                var roles = string.Join(", ", User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value));

                _logger.LogInformation("Connexion à Dashboard - {Date} - Utilisateur: {User} - Rôles: {Roles}",
                    DateTime.Now, userName, roles);
            }

            return View();
        }
    }
}
