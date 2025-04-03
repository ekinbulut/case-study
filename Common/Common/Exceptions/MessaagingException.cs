namespace Common.Exceptions;

public class MessaagingException : Exception
{
    public MessaagingException(string message, Exception ex) :base(message, ex)
    {
        
    }
}