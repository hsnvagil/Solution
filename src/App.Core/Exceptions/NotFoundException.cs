namespace App.Core.Exceptions;

public class NotFoundException(string message = "Not found") : ApplicationException(message);