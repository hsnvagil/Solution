namespace App.Core.Common.LogModels;

public class RequestResponseLog {
    public DateTimeOffset? RequestDate { get; set; }
    public string RequestPath { get; set; } = null!;
    public string RequestQuery { get; set; } = null!;
    public List<KeyValuePair<string, string>> RequestQueries { get; set; } = null!;
    public string RequestMethod { get; set; } = null!;
    public string RequestScheme { get; set; } = null!;
    public string RequestHost { get; set; } = null!;
    public Dictionary<string, string> RequestHeaders { get; set; } = null!;
    public string RequestBody { get; set; } = null!;
    public string? RequestContentType { get; set; }
    public DateTimeOffset? ResponseDate { get; set; }
    public string ResponseStatus { get; set; } = null!;
    public Dictionary<string, string> ResponseHeaders { get; set; } = null!;
    public string ResponseBody { get; set; } = null!;
    public string? ResponseContentType { get; set; }
    public string ExceptionMessage { get; set; } = null!;
    public string? ExceptionStackTrace { get; set; }
}