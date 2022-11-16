using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemFinance.DAL;
using SemFinance.Model;
using SemFinance.Model.DTOs;

namespace SemFinance.Web.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionsApiController : Controller
    {
        private ClientManagerDbContext _dbContext;

        public TransactionsApiController(ClientManagerDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] int? id)
        {
            if (id == null)
            {
                var transactions = _dbContext
                .Transactions
                .Include(p => p.Currency)
                .Include(p => p.Account)
                .ToList()
                .Select(t => new TransactionDTO(t));

                return new ObjectResult(transactions);
            }

            var transaction = _dbContext
                .Transactions
                .Include(p => p.Currency)
                .Include(p => p.Account)
                .FirstOrDefault(p => p.ID == id);

            if (transaction != null)
            {
                return new ObjectResult(new TransactionDTO(transaction));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{loanId}")]
        public IActionResult GetByLoan(int loanId)
        {
            var transactions = _dbContext
                .Transactions
                .Include(p => p.Currency)
                .Include(p => p.Account)
                .Where(t => t.LoanID == loanId)
                .ToList()
                .Select(t => new TransactionDTO(t));

            return new ObjectResult(transactions);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Transaction model)
        {
            if(ModelState.IsValid)
            {
                _dbContext.Transactions.Add(model);
                _dbContext.SaveChanges();
            }
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Edit(int id, [FromBody] Transaction model)
        {
            model.ID = id;
            _dbContext.Update(model);

            if (ModelState.IsValid)
            {
                _dbContext.SaveChanges();
                return Ok();
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var transaction = _dbContext.Transactions.FirstOrDefault(t => t.ID == id);
            if(transaction == null)
            {
                return NotFound();
            }
            else
            {
                _dbContext.Remove(transaction);
                _dbContext.SaveChanges();
                return Ok();
            }
        }
    }
}
