using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web.Data;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GitHubStatsDbContext _db;

        public HomeController(ILogger<HomeController> logger, GitHubStatsDbContext db)
        {
            this._logger = logger;
            this._db = db;
        }

        public async Task<IActionResult> Index()
        {
            var repositories = await this._db.Repositories.ToListAsync();
            return this.View(repositories.Select(r => r.ToViewModel()).ToList());
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
