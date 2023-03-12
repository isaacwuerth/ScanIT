using System.Globalization;
using ScanIT.Nmap.Model;
using ScanIT.Scanner.Nmap.Model;

namespace ScanIT.Common.Converter;

public static class ConvertResultToScan
{
    public static Scan ToScan(nmaprun nmapRun)
    {
        return new Scan
        {
            Argument = nmapRun.args,
            Version = nmapRun.version,
            StartTime = UnixTimeStampToDateTime(double.Parse(nmapRun.start)),
            EndTime = UnixTimeStampToDateTime(double.Parse(nmapRun.runstats.finished.time)),
            HostsUp = int.Parse(nmapRun.runstats.hosts.up),
            HostsDown = int.Parse(nmapRun.runstats.hosts.down),
            HostsTotal = int.Parse(nmapRun.runstats.hosts.total),
            VerboseLevel = int.Parse(nmapRun.verbose.level),
            DebugLevel = int.Parse(nmapRun.debugging.level),
            ElapsedTime = float.Parse(nmapRun.runstats.finished.elapsed),
            RunState = nmapRun.runstats.finished.exit.ToString(),
            Scanner = nmapRun.scanner.ToString(),
            XmlOutputVersion = nmapRun.xmloutputversion,
            Hosts = ToHosts(nmapRun),
            ScanInfo = ToScanInfo(nmapRun.scaninfo)
        };
    }

    private static ICollection<ScanInfo> ToScanInfo(IEnumerable<scaninfo> nmapRunScanInfo)
    {
        return nmapRunScanInfo.Select(scanInfo => 
            new ScanInfo
            {
                Type = scanInfo.type.ToString(),
                Protocol = scanInfo.protocol.ToString(), 
                Numservices = int.Parse(scanInfo.numservices), 
                Services = scanInfo.services
            }).ToList();
    }


    private static ICollection<Host> ToHosts(nmaprun nmapRun)
    {
        var convertedHosts = new List<Host>();
        var hosts = nmapRun.Items.Where(o => o is host).Cast<host>().ToList();
        foreach (var host in hosts)
        {
            var lastBootTime = host.Items.Where(o => o is uptime).Cast<uptime>().FirstOrDefault();
            var newHost = new Host
            {
                Status = host.status.state.ToString(),
                StatusReason = host.status.reason,
                Lastboot = lastBootTime != null ? ConvertNmapDateTimeToDateTime(lastBootTime.lastboot) : null,
                Addresses = ToAddresses(host.address),
                Hostnames = ToHostnames(ExtractItems<hostname>(host.Items)),
                OS = ToOs(ExtractItems<os>(host.Items)),
                Ports = ToPorts(ExtractItems<ports>(host.Items)),
            };
            HostItemsToProperties(host, newHost);
            convertedHosts.Add(newHost);
        }

        return convertedHosts;
    }

    private static ICollection<Port> ToPorts(ICollection<ports> extractItems)
    {
        var convertedPorts = new List<Port>();
        if (extractItems.Count == 0)
            return convertedPorts;
        convertedPorts.AddRange(from ports in extractItems
            from port in ports.port
            where port != null
            select new Port
            {
                Protocol = port.protocol.ToString(),
                Number = int.Parse(port.portid),
                State = port.state.state1,
                StateReason = port.state.reason,
                Scripts = ToScripts(port.script),
                Service = new Service
                {
                    Name = port.service.name,
                    Product = port.service.product,
                    Version = port.service.version,
                    Extrainfo = port.service.extrainfo,
                    Method = port.service.method.ToString(),
                    Conf = port.service.conf.ToString(),
                    Cpe = port.service.cpe,
                    Ostype = port.service.ostype
                },
            });
        return convertedPorts;
    }

    private static ICollection<Script> ToScripts(script[]? portScript)
    {
        if (portScript == null)
            return new List<Script>();
        return portScript.Select(script => new Script
        {
            Id = script.id,
            Output = script.output,
            Entries = ToScriptEntries(script.Items)
        }).ToList();
    }

    private static ICollection<Entry> ToScriptEntries(object[]? scriptItems)
    {
        var tableEntries = new List<Entry>();
        if (scriptItems == null)
            return tableEntries;
        foreach (var scriptItem in scriptItems)
        {
            switch (scriptItem)
            {
                case table item:
                {
                    if (item == null)
                        throw new Exception("Entries key or items is null");
                    tableEntries.Add(new Table
                    {
                        Key = item.key,
                        Entries = ToScriptEntries(item.Items)
                    });
                    break;
                }
                case elem elem:
                {
                    if (elem == null)
                        throw new Exception("Elem key or value is null");
                    switch (elem.Text.Length)
                    {
                        case 0:
                            throw new Exception("Elem value is empty");
                        case > 1:
                            throw new Exception("Elem value has more than one element");
                    }

                    tableEntries.Add(new Element
                    {
                        Key = elem.key,
                        Value = elem.Text[0]
                    });
                    break;
                }
            }
        }

        return tableEntries;
    }

    private static OS ToOs(IEnumerable<os> nmapHostOss)
    {
        var os = new OS();
        foreach (var nmapHostOs in nmapHostOss)
        {
            os.OsMatches = ConvertOsMatches(nmapHostOs.osmatch);
            os.PortsUsed = ToPortUsed(nmapHostOs.portused);
        }

        return os;
    }

    private static ICollection<OsMatch> ConvertOsMatches(osmatch[]? nmapOsMatches)
    {
        var osMatches = new List<OsMatch>();
        if (nmapOsMatches == null)
            return osMatches;
        osMatches.AddRange(
            nmapOsMatches.Select(nmapOsMatch =>
                new OsMatch
                {
                    Name = nmapOsMatch.name,
                    Accuracy = nmapOsMatch.accuracy,
                    Line = nmapOsMatch.line,
                    OsClasses = ToOsClasses(nmapOsMatch.osclass)
                }));

        return osMatches;
    }

    private static ICollection<PortUsed> ToPortUsed(IEnumerable<portused> osPortsUsed)
    {
        return osPortsUsed.Select(port =>
            new PortUsed
            {
                State = port.state,
                Protocol = port.proto.ToString(),
                Number = int.Parse(port.portid)
            }).ToList();
    }

    private static ICollection<OSClass> ToOsClasses(IEnumerable<osclass> osclass)
    {
        return osclass.Select(os => new OSClass
            {
                Type = os.type,
                Vendor = os.vendor,
                Family = os.osfamily,
                Generation = os.osgen,
                Cpe = os.cpe
            })
            .ToList();
    }

    private static ICollection<Hostname> ToHostnames(IEnumerable<hostname> hosts)
    {
        return hosts.Select(host =>
            new Hostname
            {
                Name = host.name, 
                Type = host.type.ToString()
            }).ToList();
    }

    private static ICollection<Address> ToAddresses(IEnumerable<address> addresses)
    {
        return addresses.Select(address => 
            new Address
            {
                Addr = address.addr, 
                AddrType = address.addrtype.ToString(), 
                Vendor = address.vendor
            }).ToList();
    }

    private static void HostItemsToProperties(host host, Host newHost)
    {
        foreach (var item in host.Items)
        {
            switch (item)
            {
                case distance distance:
                {
                    var value = distance.value;
                    if (value != null)
                        newHost.Distance = int.Parse(value);
                    break;
                }
                case uptime uptime:
                    newHost.Lastboot = ConvertNmapDateTimeToDateTime(uptime.lastboot);
                    break;
            }
        }
    }

    private static DateTime ConvertNmapDateTimeToDateTime(string nmapDateTime)
    {
        nmapDateTime = nmapDateTime.Replace("  ", " ");
        return DateTime.ParseExact(nmapDateTime, "ddd MMM d HH:mm:ss yyyy", CultureInfo.InvariantCulture);
    }

    private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }

    private static ICollection<T> ExtractItems<T>(IEnumerable<object> items)
    {
        return items.OfType<T>().ToList();
    }
}