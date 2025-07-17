namespace App.Application.UseCases.Queries.GetAvailableHomes;

public class GetAvailableHomesResponse {
    public long HomeId { get; set; }
    public string HomeName { get; set; }
    public List<DateOnly> AvailableSlots { get; set; }
}