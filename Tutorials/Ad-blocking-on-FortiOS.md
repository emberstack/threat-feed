# Using Threat Feed for Ad-blocking in FortiOS
This tutorial is meant to guide you into setting up the threat-feed on a FortiGate to block threat sources via DNS Filter.


### 1. Add External Connector (`external-resource`) to the Feed

#### GUI
Using the GUI, navigate to `External Connectors`, create a new `Domain Name` Threat Feed:  

`Name`: EmberStack Domain Threat Feed  
`URL`: https://raw.githubusercontent.com/emberstack/threat-feed/main/Feed/List/ThreatFeed.Domains.Generic.txt  
`HTTP basic authentication`: Disabled  
`Refresh Rate`: 60  
`Status`: Enabled  


#### CLI
Using the CLI (web management or SSH)
```
config system external-resource
    edit "EmberStack Domain Threat Feed"
        set type domain
        set category 193
        set resource "https://raw.githubusercontent.com/emberstack/threat-feed/main/Feed/List/ThreatFeed.Domains.Generic.txt"
        set refresh-rate 60
    next
end    
```


### 2. Configure DNS Filter Profile

#### GUI
Using the GUI, navigate to `Security Profiles`->`DNS Filter`. Select the profile you want to edit (if you have multiple profiles enabled). 

Enable `FortiGuard Category Based Filter` and in the table, under the category `Remote Categories` find `EmberStack Domain Threat Feed`. Set this to `Redirect to Block Portal`.  

> Note: We recommend also setting `Advertising` to `Redirect to Block Portal` for a more comprehensive ad-block solution.

#### CLI
Using the CLI (web management or SSH)
```
config dnsfilter profile
    edit "default"                      << ---- edit the profile you want
        config ftgd-dns
            config filters
                edit 1                  << ---- edit a new entry
                    set category 193    << ---- the feed category from step 1
                    set action block
                next
            end
        end
    end
end  
```

### 3. Set up DNS Server to use DNS Filter profile

#### Using the GUI
Using the GUI make sure you have `DNS Database` feature enabled under `System`-> `Feature Visibility`.  
Navigate to `Network`-> `DNS Servers`.  
Create a new `DNS Service on Interface` (for example for `internal`) or edit an existing one. Enable `DNS Filter` and select the profile you configured (if you have multiple profiles enabled).


#### Using the CLI
Using the CLI (web management or SSH):
```
config system dns-database
    edit "internal"
        ...
        set dnsfilter-profile "<your filter or default>"
    next
end
```
