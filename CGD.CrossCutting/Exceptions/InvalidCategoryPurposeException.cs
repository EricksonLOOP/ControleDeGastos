namespace CGD.CrossCutting.Exceptions;

public class InvalidCategoryPurposeException : Exception
{
    public InvalidCategoryPurposeException()
        : base("A categoria selecionada não é compatível com o tipo de transação.")
    {
    }
}
