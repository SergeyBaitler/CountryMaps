namespace CountryMaps.TerminalsLoader.Entities;
public class Office
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int CityCode { get; set; }
    public string? Uuid { get; set; }
    public OfficeType? Type { get; set; }
    public string CountryCode { get; set; } = null!;
    public Coordinates Coordinates { get; set; } = null!;
    public Address Address { get; set; } = null!;
    public string WorkTime { get; set; } = null!;
    public List<Phone> Phones { get; set; } = new();
}

