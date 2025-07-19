namespace App.Infrastructure.Models;

public class RawHomeJsonModel {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartOffset { get; set; }
    public int DayCount { get; set; }
}