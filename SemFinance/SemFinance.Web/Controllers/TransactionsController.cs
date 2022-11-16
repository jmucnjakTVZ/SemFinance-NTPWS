using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SemFinance.DAL;
using SemFinance.Model;
using SemFinance.Model.DTOs;

namespace SemFinance.Web.Controllers
{
    public class TransactionsController : Controller
    {

        private ClientManagerDbContext _dbContext;
        private UserManager<AppUser> _userManager;

        public TransactionsController(ClientManagerDbContext dbContext, UserManager<AppUser> userManager)
        {
            this._dbContext = dbContext;
            this._userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            var model = _dbContext.Transactions
                .Include(p => p.Currency)
                .OrderBy(p => p.DateOfTransaction)
                .Select(p => new TransactionDTO(p))
                .ToList();

            return View(model);
        }

        [Authorize]
        public IActionResult Index()
        {
            var userId = this._userManager.GetUserId(base.User);
            var model = _dbContext.Transactions
                .Include(p => p.Currency)
                .OrderBy(p => p.DateOfTransaction)
                .Where(p => p.userId == userId)
                .Select(p => new TransactionDTO(p))
                .ToList();

            return View(model);
        }

        [Authorize]
        public IActionResult Loans()
        {
            var model = new List<Loan>() { MockLoan() }.Select(l => new LoanDTO(l)).ToList();

            return View(model);
        }

        [Authorize]
        [Route("Transactions/Create")]
        public IActionResult CreateTransaction([FromQuery] int? accountId)
        {
            FillDropdownCurrencyValues();
            StoreDateFormatsToViewBag();
            FillDropdownAccountsValues();
            if (accountId == null)
            {
                return View();
            }

            var model = new Transaction() { AccountID = accountId };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [Route("Transactions/Create")]
        public IActionResult CreateTransaction(Transaction model)
        {
            if (ModelState.IsValid)
            {
                var userId = this._userManager.GetUserId(base.User);
                model.userId = userId;

                if (model.AccountID != null)
                {
                    var account = _dbContext.Accounts.FirstOrDefault(p => p.ID == model.AccountID);
                    if (account != null)
                    {
                        account.Amount = model.TransactionType == TransactionType.Income ? account.Amount + model.Amount : account.Amount - model.Amount;
                        _dbContext.Accounts.Update(account);
                        model.CurrencyId = account.CurrencyId;
                    }
                }
                _dbContext.Transactions.Add(model);
                _dbContext.SaveChanges();

                if (model.AccountID != null)
                {
                    return RedirectToAction("Details", "Accounts", new { id = model.AccountID });
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                FillDropdownCurrencyValues();
                StoreDateFormatsToViewBag();
                FillDropdownAccountsValues();
                return View();
            }
        }

        [Authorize]
        [Route("Transactions/Edit/{id:int}")]
        public IActionResult EditTransaction(int id)
        {
            var userId = this._userManager.GetUserId(base.User);
            var model = _dbContext.Transactions.Where(p => p.userId == userId).Include(p => p.Currency).Include(p => p.Account).FirstOrDefault(p => p.ID == id);
            FillDropdownCurrencyValues();
            StoreDateFormatsToViewBag();
            FillDropdownAccountsValues();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [Route("Transactions/Edit/{id:int}")]
        public async Task<IActionResult> EditTransaction(Transaction model)
        {
            var userId = this._userManager.GetUserId(base.User);
            var transaction = _dbContext.Transactions
                .Where(p => p.userId == userId)
                .Include(p => p.Currency)
                .Include(p => p.Account)
                .Single(p => p.ID == model.ID);

            var ok = await TryUpdateModelAsync(transaction);
            if (ok && ModelState.IsValid)
            {
                _dbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            FillDropdownCurrencyValues();
            StoreDateFormatsToViewBag();
            FillDropdownAccountsValues();
            return View();
        }

        [Authorize]
        [HttpPost]
        [Route("Transactions/Ajax")]
        public IActionResult TransactionsAjax()
        {
            var userId = this._userManager.GetUserId(base.User);
            var model = _dbContext.Transactions
               .Where(p => p.userId == userId)
               .Include(p => p.Currency)
               .OrderBy(p => p.DateOfTransaction)
               .Select(p => new TransactionDTO(p))
               .ToList();

            return PartialView("_TransactionList", model);
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

        private void FillDropdownAccountsValues()
        {
            var userId = this._userManager.GetUserId(base.User);
            var selectItems = new List<SelectListItem>();

            //Polje je opcionalno
            var listItem = new SelectListItem
            {
                Text = "- odaberite -",
                Value = ""
            };
            selectItems.Add(listItem);

            foreach (var category in _dbContext.Accounts.Where(p => p.userId == userId))
            {
                listItem = new SelectListItem(category.Name, category.ID.ToString());
                selectItems.Add(listItem);
            }

            ViewBag.PossibleAccounts = selectItems;
        }

        public void StoreDateFormatsToViewBag()
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            ViewBag.datePickerPattern = culture.DateTimeFormat.ShortDatePattern.ToLower();
            ViewBag.dateFieldPattern = culture.DateTimeFormat.ShortDatePattern;
        }

        private Transaction MockTransaction(string name, double amount, TransactionType transactionType) => new Transaction()
        {
            Name = name,
            Amount = amount,
            Currency = new Model.Currency()
            {
                Name = "Kuna",
                CurrencyCode = "HRK",
                KonversionValue = 1,
                ID = 1
            },
            CurrencyId = 1,
            DateOfTransaction = DateTime.Now,
            Description = "This was a transaction",
            ID = 1,
            TransactionType = transactionType
        };

        private Loan MockLoan() => new Loan()
        {
            ID = 1,
            CreatedOn = DateTime.Now,
            Name = "Borrowed some money",
            Amount = 2000,
            Transactions = new List<Transaction>() { MockTransaction("test", 2000, TransactionType.Expense) }
        };
    }
}
