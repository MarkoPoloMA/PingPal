namespace PingPal.Exceptions;

public abstract class ValidationException : Exception
{
	public ValidationException(string message) : base(message) { }
}