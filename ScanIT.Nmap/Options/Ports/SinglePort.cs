namespace ScanIT.Nmap.Options.Ports;

public class SinglePort: IPort
{
    public SinglePort(int port)
    {
        Number = port;
    }
    public int Number { get; set; }
    public override string ToString()
    {
        return $"-p {Number}";
    }
}