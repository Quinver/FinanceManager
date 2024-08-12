using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FinanceManager.Models;

namespace FinanceManager.Controllers;

public class FinanceController : Controller
{
    private static List<Expense> expenses = new();

    private readonly ILogger<FinanceController> _logger;

    public FinanceController(ILogger<FinanceController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        expenses = expenses.OrderBy(e => e.DateTime).Reverse().ToList();
        return View(expenses);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public IActionResult AddExpense()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddExpense(Expense expense)
    {
        if (ModelState.IsValid)
        {
            expense.Id = expenses.Count + 1;
            expenses.Add(expense);
            return RedirectToAction("Index");
        }
        return View(expense);
    }

    [HttpPost]
    public IActionResult RemoveExpense(int Id)
    {
        var expense = expenses.FirstOrDefault(e => e.Id == Id);
        if (expense != null)
        {
            expenses.Remove(expense);
        }
        return RedirectToAction("Index");
    }
    [HttpGet]
    public IActionResult EditExpense(int id)
    {
        var expense = expenses.FirstOrDefault(e => e.Id == id);
        if (expense == null)
            return NotFound();

        return View(expense);
    }

    [HttpPost]
    public IActionResult EditExpense(Expense expense)
    {
        if (ModelState.IsValid)
        {
            var existingExpense = expenses.FirstOrDefault(e => e.Id == expense.Id);
            if (existingExpense != null)
            {
                existingExpense.Name = expense.Name;
                existingExpense.Amount = expense.Amount;
                existingExpense.Description = expense.Description;
                existingExpense.DateTime = expense.DateTime;
            }
            return RedirectToAction("Index");
        }
        return View(expense);
    }

    [HttpPost]
    public IActionResult ResetExpenses()
    {
        if (expenses.Count != 0)
        {
            expenses.Clear();
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DuplicateExpense(int Id)
    {
        var expense = expenses.FirstOrDefault(e => e.Id == Id);
        if (expense != null)
        {
            var newExpense = new Expense
            {
                Id = expenses.Count + 1,
                Name = expense.Name,
                Amount = expense.Amount,
                Description = expense.Description,
                DateTime = expense.DateTime
            };

            expenses.Add(newExpense);
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }

    public IActionResult ExpenseChartDate()
    {
        // Group expenses by date and sum them up
        var expenseData = expenses
            .GroupBy(e => e.DateTime.Date)
            .OrderByDescending(g => g.Key) // Group by date only (ignore time)
            .Select(g => new
            {
                Date = g.Key.ToString("yyyy-MM-dd"), // Format the date
                TotalAmount = g.Sum(e => e.Amount) // Sum the amounts
            })
            .ToList();

        return Json(expenseData); // Return data as JSON
    }
}