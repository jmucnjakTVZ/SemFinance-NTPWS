using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SemFinance.DAL;
using SemFinance.Model;
using SemFinance.Model.DTOs;

namespace SemFinance.Web.Controllers
{
    public class AccountsController : Controller
    {
        private ClientManagerDbContext _dbContext;
        private UserManager<AppUser> _userManager;

        public AccountsController(ClientManagerDbContext dbContext, UserManager<AppUser> userManager)
        {
            this._dbContext = dbContext;
            this._userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            var accounts = _dbContext.Accounts
                .Include(prop => prop.Currency)
                .Select(p => new AccountDTO(p))
                .ToList();

            return View(accounts);
        }

        [Authorize]
        public IActionResult Index()
        {
            var userId = this._userManager.GetUserId(base.User);
            var accounts = _dbContext.Accounts
                .Include(prop => prop.Currency)
                .Where(p => p.userId == userId)
                .ToList();
            var transactions = _dbContext.Transactions
                .Where(prop => prop.DateOfTransaction <= DateTime.UtcNow)
                .Where(p => p.userId == userId)
                .Include(prop => prop.Currency)
                .OrderBy(p => p.DateOfTransaction)
                .ThenBy(p => p.Amount)
                .Take(3)
                .ToList();
            var futureTransactions = _dbContext.Transactions
                .Where(prop => prop.DateOfTransaction > DateTime.UtcNow)
                .Where(p => p.userId == userId)
                .Include(prop => prop.Currency)
                .OrderBy(p => p.DateOfTransaction)
                .ThenBy(p => p.Amount)
                .Take(3)
                .ToList();

            var model = new AccountsIndexDTO(accounts, transactions, futureTransactions);

            return View(model);
        }

        [Authorize]
        [Route("Accounts/Details/{id:int}")]
        public IActionResult AccountDetails(int id)
        {
            var userId = this._userManager.GetUserId(base.User);
            var account = _dbContext.Accounts
                .Include(p => p.Currency)
                .Include(p => p.Transactions)
                .Where(p => p.userId == userId)
                .FirstOrDefault(p => p.ID == id);

            var transactions = account?.Transactions?
                .Where(p => p.userId == userId)
                .OrderByDescending(p => p.DateOfTransaction)
                .Select(p =>
                {
                    p.Currency = account.Currency;
                    return p;
                }).ToList() ?? new List<Transaction>();
            var model = new AccountDetailsDTO(account, transactions);
            ViewBag.DataPoints = GenerateDataPoints(transactions);

            return View(model);
        }

        [Authorize]
        [Route("Accounts/Create")]
        public IActionResult CreateAccount()
        {
            FillDropdownCurrencyValues();
            return View();
        }

        [Authorize]
        [HttpPost]
        [Route("Accounts/Create")]
        public IActionResult CreateAccount(Account model)
        {
            var userId = this._userManager.GetUserId(base.User);
            model.userId = userId;
            if (ModelState.IsValid)
            {
                _dbContext.Accounts.Add(model);
                _dbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                FillDropdownCurrencyValues();
                return View();
            }
        }

        [Authorize]
        [Route("Accounts/Edit/{id:int}")]
        public IActionResult EditAccount(int id)
        {
            var userId = this._userManager.GetUserId(base.User);
            var model = _dbContext.Accounts
                .Include(p => p.Currency)
                .Where(p => p.userId == userId)
                .FirstOrDefault(p => p.ID == id);
            FillDropdownCurrencyValues();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [Route("Accounts/Edit/{id:int}")]
        public async Task<IActionResult> EditAccount(Account model)
        {
            var userId = this._userManager.GetUserId(base.User);
            var account = _dbContext.Accounts
                .Where(p => p.userId == userId)
                .Include(p => p.Currency)
                .Single(p => p.ID == model.ID);
            var ok = await TryUpdateModelAsync(account);
            if (ok && ModelState.IsValid)
            {
                if (account.CurrencyId != model.CurrencyId)
                {
                    var transactions = _dbContext.Transactions.Where(p => p.AccountID == model.ID)
                        .Where(p => p.userId == userId)
                        .Include(p => p.Currency);
                    foreach (var transaction in transactions)
                    {
                        transaction.CurrencyId = model.CurrencyId;
                        _dbContext.Update(transaction);
                    }
                }
                _dbContext.SaveChanges();

                return RedirectToAction(nameof(AccountDetails), new { id = model.ID });
            }

            FillDropdownCurrencyValues();
            return View();
        }

        [Authorize]
        [Route("Accounts/Delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var userId = this._userManager.GetUserId(base.User);
            var account = _dbContext.Accounts.Where(p => p.userId == userId).Single(p => p.ID == id);
            _dbContext.Accounts.Remove(account);

            var accountTransactions = _dbContext.Transactions.Where(p => p.userId == userId).Where(p => p.AccountID == id);
            foreach (var transaction in accountTransactions)
            {
                transaction.AccountID = null;
                _dbContext.Transactions.Update(transaction);
            }

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        [Route("Accounts/Details/{id:int}/Transactions")]
        public IActionResult TransactionsAjax(int id)
        {
            var userId = this._userManager.GetUserId(base.User);
            var account = _dbContext.Accounts
                .Include(p => p.Currency)
                .Include(p => p.Transactions)
                .Where(p => p.userId == userId)
                .FirstOrDefault(p => p.ID == id);

            var transactions = account?.Transactions?
                .Where(p => p.userId == userId)
                .OrderByDescending(p => p.DateOfTransaction)
                .Select(p =>
                {
                    p.Currency = account.Currency;
                    return p;
                }).ToList() ?? new List<Transaction>();

            return PartialView("_TransactionList", transactions.Select(p => new TransactionDTO(p)).ToList());
        }

        private void FillDropdownCurrencyValues()
        {
            var selectItems = new List<SelectListItem>();

            foreach (var category in _dbContext.Currency)
            {
                var listItem = new SelectListItem(category.Name, category.ID.ToString());
                selectItems.Add(listItem);
            }

            ViewBag.PossibleCurrencies = selectItems;
        }


        private string GenerateDataPoints(List<Transaction> transactions)
        {
            var lastTransactions = transactions
                .TakeWhile(t => t.DateOfTransaction
                    .Subtract(t.DateOfTransaction.AddMonths(1))
                    .TotalDays < 30);
            var dataPoints = lastTransactions
                .GroupBy(it => it.DateOfTransaction.Day)
                .Select(it => new DataPoint(
                    it.First()
                    .DateOfTransaction
                    .Day
                    .ToString(),
                    it.ToList()
                    .Average(it => it.Amount)))
                .OrderBy(p => p.Label);
            return JsonConvert.SerializeObject(dataPoints);
        }
    }
}
