namespace API_Background.Exceptions
{
    public sealed class RepositoryException(string mensagem, Exception excecao) : Exception(mensagem, excecao)
    {
    }
}
