using ScanIT.Nmap;
using ScanIT.Nmap.Model;

namespace ScanITTest.NmapTest;

public class SerializerTest
{
    private static string NmapScanResultFile { get; } = "./Resources/NmapScanResult.xml";
    
    [Fact]
    public void SerializeNmapScanResult()
    {
        var nmapScanResult = Nmap.ConvertResult(NmapScanResultFile);
        Assert.NotNull(nmapScanResult);
        var scan = ConvertResultToScan.ToScan(nmapScanResult);
        Assert.NotNull(scan);
    }
    

}