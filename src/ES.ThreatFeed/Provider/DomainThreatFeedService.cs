using System.Text;
using ES.ThreatFeed.Provider.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ES.DnsBlockFeed
{
    internal class DomainThreatFeedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IOptions<ThreatFeedsConfiguration> _feedOptions;
        private readonly IHttpClientFactory _httpClientFactory;
        private Action Exit { get; }

        public DomainThreatFeedService(ILogger<DomainThreatFeedService> logger,
            IHostApplicationLifetime lifetime,
            IOptions<ThreatFeedsConfiguration> feedOptions,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _feedOptions = feedOptions;
            _httpClientFactory = httpClientFactory;
            Exit = lifetime.StopApplication;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {

                await HandleDomainNameThreatFeeds(stoppingToken);
            }
            finally
            {
                Exit();
            }
        }

        private async Task HandleDomainNameThreatFeeds(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Processing domain name threat feeds");

            IReadOnlyList<string> ParseHostsFileFormat(string rawContent)
            {
                var result = new List<string>();

                var lines = rawContent.Replace("\r\n", "\n").Split("\n", StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var rawLine = line.Replace('\t', ' ');
                    if (rawLine.Contains("#"))
                        rawLine = rawLine.Substring(0, rawLine.IndexOf("#", StringComparison.Ordinal));
                    rawLine = rawLine.Trim();
                    if (string.IsNullOrWhiteSpace(rawLine)) continue;

                    if (!rawLine.StartsWith("0.0.0.0")) continue;

                    var lineParts = rawLine.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (!lineParts.Any()) continue;
                    var entry = lineParts.Last().ToLowerInvariant().Trim();



                    var uri = new Uri($"protocol://{entry}", UriKind.Absolute);
                    if (uri.IsLoopback) continue;
                    if (uri.HostNameType != UriHostNameType.Dns) continue;

                    result.Add(entry);
                }
                return result.ToList();
            }

            string BuildListFormatFile(List<string> entries)
            {
                var builder = new StringBuilder();
                foreach (var entry in entries)
                {
                    builder.AppendLine(entry);
                }

                return builder.ToString();
            }



            if (_feedOptions.Value.DomainThreatFeeds is null)
            {
                _logger.LogError("Could not load list of domain name threat feeds");
                return;
            }

            var entriesByCategory = new Dictionary<string, List<string>>();

            foreach (var feed in _feedOptions.Value.DomainThreatFeeds)
            {
                _logger.LogDebug("Processing {url}", feed.Url);
                var client = _httpClientFactory.CreateClient();
                var rawContent = await client.GetStringAsync(feed.Url, stoppingToken);

                var entries = ParseHostsFileFormat(rawContent);
                _logger.LogTrace("{url} contains {domains} records", feed.Url, entries.Count);

                foreach (var category in feed.Categories)
                {
                    if (!entriesByCategory.ContainsKey(category)) entriesByCategory.Add(category, new List<string>());
                    entriesByCategory[category].AddRange(entries);
                }
            }

            foreach (var category in entriesByCategory.Keys)
            {
                entriesByCategory[category] = entriesByCategory[category].Distinct().ToList();
            }


            _logger.LogDebug("Generating lists");

            var lists = new Dictionary<string, List<string>>();
            foreach (var pair in entriesByCategory.OrderBy(s=>s.Key))
            {
                lists.Add(pair.Key,pair.Value);
            }
            lists.Add("Generic", entriesByCategory.Values.SelectMany(s => s).ToList().Distinct().ToList());

            foreach (var category in lists)
            {
                _logger.LogDebug("List {category} contains {count} item(s)", category.Key, category.Value.Count);

                var listFormatContent = BuildListFormatFile(category.Value);

                Directory.CreateDirectory("Output");
                await File.WriteAllTextAsync(Path.Combine("Output", $"ThreatFeed.Domains.{category.Key}.txt"),
                    listFormatContent, stoppingToken);

            }

            var unified = entriesByCategory.Values.SelectMany(s => s).ToList().Distinct();




        }

    }
}
