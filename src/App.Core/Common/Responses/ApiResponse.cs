using App.Core.ValueObjects;

namespace App.Core.Common.Responses;

public class ApiResponse<T> {
    public ApiResponse(T data, ResultStatus resultStatus) {
        Data = data;
        ResultStatus = resultStatus;
    }

    public ApiResponse(string errorMessage) {
        ResultStatus = ResultStatus.Failure;
        ErrorMessage = errorMessage;
    }

    public ApiResponse() {
    }

    public T Data { get; set; } = default!;

    public string ErrorMessage { get; set; } = null!;

    public ResultStatus ResultStatus { get; set; } = null!;
}