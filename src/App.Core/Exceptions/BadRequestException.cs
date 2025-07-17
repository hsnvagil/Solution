namespace App.Core.Exceptions;

public class BadRequestException(string message = "Bad request") : ApplicationException(message);