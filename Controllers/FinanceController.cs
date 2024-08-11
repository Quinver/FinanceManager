using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FinanceManager.Models;

namespace FinanceManager.Controllers;

public class FinanceController : Controller
{
    private static List<Expense> expenses = new List<Expense>(); 

    private readonly ILogger<FinanceController> _logger;

    public FinanceController(ILogger<FinanceController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
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
        if(ModelState.IsValid)
        {
            expense.Id = expenses.Count + 1;
            expenses.Add(expense);
            return RedirectToAction("Index");
        }
        return View(expense);
    }
}
