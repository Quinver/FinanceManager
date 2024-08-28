using System.ComponentModel.DataAnnotations;

namespace FinanceManager.Models;

public class Expense
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Amount { get; set; }
    public string? Description { get; set; }
    public DateTime DateTime { get; set; }
}
