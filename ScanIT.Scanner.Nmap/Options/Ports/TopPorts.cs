namespace ScanIT.Scanner.Nmap.Options.Ports;

public class TopPorts: IPort
{
    public TopPorts(int number)
    {
        Number = number;
    }
    public int Number { get; set; }
    public override string ToString()
    {
        return $"–top-ports {Number}";
    }
}