namespace CGD.CrossCutting.Exceptions;

public class InvalidTransactionForMinorException : Exception
{
    public InvalidTransactionForMinorException()
        : base("Usuários menores de idade não podem registrar receitas.")
    {
    }
}
