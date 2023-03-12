namespace ScanIT.Nmap.Options.Target;

public class IpAddress: ITarget
{
    public IpAddress(string ip)
    {
        Ip = ip;
    }
    public string Ip { get; set; }
    public override string ToString()
    {
        return Ip;
    }
}