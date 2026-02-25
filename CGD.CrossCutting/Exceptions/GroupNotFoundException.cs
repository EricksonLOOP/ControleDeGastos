namespace CGD.CrossCutting.Exceptions;

public class GroupNotFoundException : Exception
{
    public GroupNotFoundException()
        : base("Grupo não encontrado")
    {
    }

    public GroupNotFoundException(string message)
        : base(message)
    {
    }

    public GroupNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}