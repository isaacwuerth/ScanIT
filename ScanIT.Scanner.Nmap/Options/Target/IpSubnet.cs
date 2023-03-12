namespace ScanIT.Scanner.Nmap.Options.Target;

public class IpSubnet: ITarget
{
    public IpSubnet(string ip, int subnet)
    {
        Ip = ip;
        Subnet = subnet;
    }
    public string Ip { get; set; }
    public int Subnet { get; set; }
    public override string ToString()
    {
        return $"{Ip}/{Subnet}";
    }
}