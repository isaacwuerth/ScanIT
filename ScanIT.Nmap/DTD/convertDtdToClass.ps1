# Install Java
# choco install javaruntime -y
Write-Warning "Converting DTD to XSD"
Start-Process -FilePath "java" -ArgumentList "-jar trang.jar nmap.dtd Result.xsd" -Wait -WorkingDirectory "$pwd" -NoNewWindow
Write-Warning "Converting XSD to C#"
# Install Windows SDK
# choco install windows-sdk-10.0 -y

$matches = Get-ChildItem -Path "C:\Program Files (x86)\Microsoft SDKs\Windows" -Include "xsd.exe" -File -Recurse -ErrorAction SilentlyContinue
Start-Process -FilePath $matches[0].FullName -ArgumentList "Result.xsd /c /n:ScanIT.Nmap.Model /o:../Model/" -Wait -WorkingDirectory "$pwd" -NoNewWindow

Write-Warning "Remove XSD"
Remove-Item -Path "Result.xsd" -Force