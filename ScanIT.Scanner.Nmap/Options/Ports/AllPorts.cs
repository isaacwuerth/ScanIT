namespace ScanIT.Scanner.Nmap.Options.Ports;

public class AllPorts: IPort
{
    public override string ToString()
    {
        return "-p-";
    }
}