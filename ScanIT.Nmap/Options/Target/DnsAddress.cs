namespace ScanIT.Nmap.Options.Target;

public class DnsAddress: ITarget
{
    public DnsAddress(string dns)
    {
        Dns = dns;
    }    
    public string Dns { get; set; }
    public override string ToString()
    {
        return Dns;
    }
}