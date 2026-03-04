namespace CountryMaps.TerminalsLoader.Entities
{
    public sealed class Phone
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public string? Comment { get; set; }
    }
}
