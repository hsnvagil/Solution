namespace App.Core.Entities.Core;

public class Home {
    public long Id { get; set; }
    public string Name { get; set; }
    public List<DateOnly> AvailableSlots { get; set; }
    
    public override bool Equals(object? obj) =>
        obj is Home other && Id == other.Id;

    public override int GetHashCode() =>
        Id.GetHashCode();
}