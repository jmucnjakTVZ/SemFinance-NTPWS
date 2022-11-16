using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemFinance.DAL;

namespace SemFinance.Web.Controllers
{
    public class LoanApiController : Controller
    {
        private ClientManagerDbContext _dbContext;

        public LoanApiController(ClientManagerDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}
