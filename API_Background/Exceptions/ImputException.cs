namespace API_Background.Exceptions
{
    public sealed class ImputException(string mensagem) : ArgumentException(mensagem)
    {
    }
}
