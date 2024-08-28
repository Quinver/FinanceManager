using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FinanceManager.Models;
using FinanceManager.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Controllers;

public class FinanceController : Controller
{
    private readonly ILogger<FinanceController> _logger;
    private readonly ApplicationDbContext _context;

    public FinanceController(ILogger<FinanceController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // On loading Index page
    public async Task<IActionResult> Index()
    {
        var expenses = await _context.Expenses.ToListAsync();
        foreach (var expense in expenses)
        {
            _logger.LogInformation("Retrieved expense ID: {Id}, Date: {DateTime}", expense.Id, expense.DateTime);
        }
        expenses = expenses.OrderBy(e => e.DateTime).Reverse().ToList();
        return View(expenses);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // View adding page
    [HttpGet]
    public IActionResult AddExpense()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("AddExpense")]
    public async Task<IActionResult> AddExpenseAsync(Expense expense)
    {
        if (ModelState.IsValid)
        {
            CheckDate(expense);

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(expense);
    }

    [HttpPost]
    [ActionName("RemoveExpense")]
    public async Task<IActionResult> RemoveExpenseAsync(int Id)
    {
        var expense = await _context.Expenses.FindAsync(Id);
        if (expense != null)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
    [HttpGet]
    [ActionName("EditExpense")]
    public async Task<IActionResult> EditExpenseAsync(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
            return NotFound();

        return View(expense);
    }

    [HttpPost]
    [ActionName("EditExpense")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditExpenseAsync(Expense expense)
    {
        if (ModelState.IsValid)
        {
            CheckDate(expense);

            var existingExpense = await _context.Expenses.FindAsync(expense.Id);

            if (existingExpense != null)
            {

                existingExpense.Name = expense.Name;
                existingExpense.Amount = expense.Amount;
                existingExpense.Description = expense.Description;
                existingExpense.DateTime = expense.DateTime;

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
        return View(expense);
    }

    [HttpPost]
    [ActionName("ResetExpenses")]
    public async Task<IActionResult> ResetExpensesAsync()
    {
        await _context.ClearDatabaseAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ActionName("DuplicateExpense")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DuplicateExpenseAsync(int Id)
    {
        var existingExpense = await _context.Expenses.FindAsync(Id);
        if (existingExpense != null)
        {
            var newExpense = new Expense
            {
                Name = existingExpense.Name,
                Amount = existingExpense.Amount,
                Description = existingExpense.Description,
                DateTime = existingExpense.DateTime
            };

            _context.Expenses.Add(newExpense);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }

    private static void CheckDate(Expense expense)
    {
        if (expense.DateTime.Kind == DateTimeKind.Unspecified)
        {
            expense.DateTime = TimeZoneInfo.ConvertTimeToUtc(expense.DateTime);
        }
        else if (expense.DateTime.Kind == DateTimeKind.Local)
        {
            expense.DateTime = TimeZoneInfo.ConvertTimeToUtc(expense.DateTime);
        }
    }
}
