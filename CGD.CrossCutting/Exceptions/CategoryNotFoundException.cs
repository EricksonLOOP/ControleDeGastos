namespace CGD.CrossCutting.Exceptions;

public class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException()
        : base("Categoria não encontrada.")
    {
    }
}
