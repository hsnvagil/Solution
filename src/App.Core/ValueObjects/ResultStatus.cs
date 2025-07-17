namespace App.Core.ValueObjects;

public record ResultStatus(int StatusCode) {
    public static readonly ResultStatus Success = new(1);
    public static readonly ResultStatus Failure = new(2);
}