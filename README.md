# Threat Feed
This repository contains a multi-format feed of threat sources (Advertising, Malware, Phishing, etc.) that can be imported in applications or appliances to filter or block traffic.

[![Pipeline](https://github.com/emberstack/threat-feed/actions/workflows/pipeline.yaml/badge.svg)](https://github.com/emberstack/threat-feed/actions/workflows/pipeline.yaml)
[![license](https://img.shields.io/github/license/emberstack/threat-feed.svg?style=flat-square)](LICENSE)

## Support
If you need help, want to ask a question or submit and idea, please join the [Discussions](https://github.com/emberstack/threat-feed/discussions) on GitHub.
If you have a bug please feel free to open an [Issue](https://github.com/emberstack/threat-feed/issues) on GitHub.  

## Deployment

Updated lists can be found in the `Feed` directory and are grouped by format and category.

Current formats:
- `List` - Simple list of threat sources. Compatible with applications that can import lists with one item per line. (Example: FortiOS)
- `HostsFile` (`Work in progress`) - Hosts file containing DNS mappings for threats to a non-routable meta-address (`0.0.0.0`)

Categories are dynamic (for example `Advertising` - for Ad blockers), but there is also a `Generic` category which includes all threats.

You can download these files from GitHub directly or use a raw link to have applications download the latest version periodically (link format is https://raw.githubusercontent.com/emberstack/threat-feed/main/Feed/ `<format>` / `<file>`).

For example, to import the latest Generic (unified list of all categories) into FortiOS, use https://raw.githubusercontent.com/emberstack/threat-feed/main/Feed/List/ThreatFeed.Domains.Generic.txt

## Threat Sources

The sources for the feeds are either externally stable lists (lists that are maintained or generally accepted as containing little or no false data) and the curated lists provided by the EmberStack team or contributors.



We give credit and our respect to all parties involved in providing and maintaining external sources of threats.

You can view the sources in (`src/ES.ThreatFeed/threat-feeds.json`).
Below is a list of sources and a description for each:
| Source                               | Type and Category                                | Description                                             |
| ------------------------------------ | ------------------------------------------------ | ------------------------------------------------------- |
| Steven Black's Hosts File            | Type: Domain threats. Categories: `Advertising`  | Source Home: https://github.com/StevenBlack/hosts <br/> This reputable source is used in a lot of applications, including `Pi-hole`. |
| EmberStack's collections            | Type: Domain threats. Categories: `Advertising`  | Source Home: https://github.com/emberstack/threat-feed <br/> This collection of threats is manually curated by the ES team and validated against multiple threat mitigating systems. |
| AdAway default blocklist            | Type: Domain threats. Categories: `Advertising`  | Source Home: https://github.com/AdAway/adaway.github.io/ |
| The Firebog            | Type: Domain threats. Categories: `Advertising`  | Source Home: https://firebog.net/ |
| Ad filter list by Disconnect            | Type: Domain threats. Categories: `Advertising`  | Source Home: https://disconnect.me/ |

### Update Frequency

All feeds are updated at least once a day (the nightly build), if there are changes either to the external sources or ES's collections.
If you're using these feeds from applications that automatically update based on URL, we suggest setting an update interval of one hour.

## Contributing

If you wish to contribute to this repository, please let us know in the [Discussions](https://github.com/emberstack/threat-feed/discussions) tab on GitHub what your idea is, before opening an issue. We're still working on establishing a good `Contributing Guide`.