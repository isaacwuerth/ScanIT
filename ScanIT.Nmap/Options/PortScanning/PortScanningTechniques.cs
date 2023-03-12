namespace ScanIT.Nmap.Options.PortScanning;

public enum PortScanningTechniques
{
    TcpSynScan,
    TcpConnectScan,
    TcpAckScan,
    TcpNullScan,
    TcpFinScan,
    TcpXmasScan,
    TcpWindowScan,
    TcpMaimonScan,
    UdpScan,
    SctpInitScan,
    IpProtocolScan,
}