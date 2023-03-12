namespace ScanIT.Nmap.Model;

public class Scan
{
    public string Argument { get; set; }
    public string Version { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int HostsUp { get; set; }
    public int HostsDown { get; set; }
    public int HostsTotal { get; set; }
    public int VerboseLevel { get; set; }
    public int DebugLevel { get; set; }
    public float ElapsedTime { get; set; }
    public ICollection<Host> Hosts { get; set; }
    public string RunState { get; set; }
    public string Scanner { get; set; }
    public string XmlOutputVersion { get; set; }
    public ICollection<ScanInfo> ScanInfo { get; set; }
}

public class ScanInfo
{
    public string Type { get; set; }
    public string Protocol { get; set; }
    public int Numservices { get; set; }
    public string Services { get; set; }
}

public class Host
{
    public string Status { get; set; }
    public string StatusReason { get; set; }
    public DateTime? Lastboot { get; set; }
    public int Distance { get; set; }
    public OS OS { get; set; }
    public ICollection<Address> Addresses { get; set; }
    public ICollection<Hostname> Hostnames { get; set; }
    public ICollection<Port> Ports { get; set; }
}

public class Address
{
    public string Addr { get; set; }
    public string AddrType { get; set; }
    public string Vendor { get; set; }
}

public class OS
{
    public ICollection<OsMatch> OsMatches { get; set; }
    public ICollection<PortUsed> PortsUsed { get; set; }
}

public class OsMatch
{
    public string Name { get; set; }
    public string Accuracy { get; set; }
    public string Line { get; set; }
    public ICollection<OSClass> OsClasses { get; set; }
}

public class PortUsed
{
    public string State { get; set; }
    public string Protocol { get; set; }
    public int Number { get; set; }
}

public class OSClass
{
    public string Type { get; set; }
    public string Vendor { get; set; }
    public string Family { get; set; }
    public string Generation { get; set; }
    public ICollection<string> Cpe { get; set; }    
}

public class Hostname    
{
    public string Name { get; set; }
    public string Type { get; set; }
}

public class Service
{

    public string Name { get; set; }
    public string Product { get; set; }
    public string Version { get; set; }
    public string Extrainfo { get; set; }
    public string Method { get; set; }
    public string Conf { get; set; }
    public string[] Cpe { get; set; }
    public string Ostype { get; set; }
    public string Tunnel { get; set; }
}

public class Port
{
    public string Protocol { get; set; }
    public int Number { get; set; }
    public string State { get; set; }
    public string StateReason { get; set; }
    public ICollection<Script> Scripts { get; set; }
    public Service Service { get; set; }
}

public class Script
{
    public string Id { get; set; }
    public string Output { get; set; }
    public ICollection<Entry> Entries { get; set; }
}

public class Entry
{
    public string? Key { get; set; }
}
public class Table: Entry
{
    public ICollection<Entry> Entries { get; set; }
}
public class Element: Entry
{
    public string? Value { get; set; }
}