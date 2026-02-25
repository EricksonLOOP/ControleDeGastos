namespace CGD.CrossCutting.Exceptions;

public class ExpenseNotFoundException : Exception
{
    public ExpenseNotFoundException()
        : base("Expense not found.")
    {
    }

    public ExpenseNotFoundException(string message)
        : base(message)
    {
    }

    public ExpenseNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
