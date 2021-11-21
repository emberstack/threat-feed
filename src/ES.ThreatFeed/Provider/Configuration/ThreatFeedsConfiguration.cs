namespace ES.ThreatFeed.Provider.Configuration;

public record ThreatFeedsConfiguration
{
    public List<DomainThreatFeedSource>? DomainThreatFeeds { get; set; }
}