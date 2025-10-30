namespace F_F.Core.Exceptions;

public class TokenExpiredException : Exception
{
    public TokenExpiredException() { }
    public TokenExpiredException(string message) : base(message) { }
    public TokenExpiredException(string message, Exception inner) : base(message, inner) { }
}