namespace ScanIT.Nmap.Options.Ports;

public class FastPortScan: IPort
{
    public override string ToString()
    {
        return "-F";
    }
}