namespace CountryMaps.TerminalsLoader.Entities
{
    public sealed class Address
    {
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string House { get; set; } = null!;
    }
}
