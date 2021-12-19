namespace ES.ThreatFeed.Provider.Configuration
{
    public record DomainThreatFeedSource
    {
        public string? Url { get; set; }
        public string? Description { get; set; }

        public string? Format { get; set; }
        public string[]? Categories { get; set; }
    }
}