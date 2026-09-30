param(
    [string]$VMName,
    [string]$PhysicalAdapter,
    [string]$WhitelistIPs
)

$ErrorActionPreference = 'Stop'
$ConfirmPreference = 'None'

$SwitchName = "External-Net"
$PublicNIC = "Public-NIC"
$PrivateNIC = "Private-NIC"
$TCPPorts = @(47984, 47989, 48010)
$UDPPorts = @(47998, 47999, 48000, 48002, 48010)
$Directions = @("Inbound", "Outbound")

Write-Host "Binding switch to $PhysicalAdapter..."
Set-VMSwitch -Name $SwitchName -NetAdapterName $PhysicalAdapter -ErrorAction Stop

# Clear Old Rules
Get-VMNetworkAdapterAcl -VMName $VMName -ErrorAction SilentlyContinue | Remove-VMNetworkAdapterAcl
$existingRules = Get-VMNetworkAdapterExtendedAcl -VMName $VMName -ErrorAction SilentlyContinue
if ($existingRules) {
    foreach ($rule in $existingRules) {
        Remove-VMNetworkAdapterExtendedAcl -VMName $VMName -Weight $rule.Weight -Direction $rule.Direction -ErrorAction SilentlyContinue
    }
}

# Public-NIC Rules
$PubWeight = 200
if (-not [string]::IsNullOrWhiteSpace($WhitelistIPs)) {
    $IPArray = $WhitelistIPs -split ","
    foreach ($IP in $IPArray) {
        foreach ($Port in $TCPPorts) { Add-VMNetworkAdapterExtendedAcl -VMName $VMName -VMNetworkAdapterName $PublicNIC -Action Allow -Direction Inbound -RemoteIPAddress $IP -LocalPort $Port -Protocol TCP -Weight $PubWeight -Stateful $true; $PubWeight++ }
        foreach ($Port in $UDPPorts) { Add-VMNetworkAdapterExtendedAcl -VMName $VMName -VMNetworkAdapterName $PublicNIC -Action Allow -Direction Inbound -RemoteIPAddress $IP -LocalPort $Port -Protocol UDP -Weight $PubWeight -Stateful $true; $PubWeight++ }
    }
}

Add-VMNetworkAdapterExtendedAcl -VMName $VMName -VMNetworkAdapterName $PublicNIC -Action Allow -Direction Outbound -LocalPort 68 -RemotePort 67 -Protocol UDP -Weight 190
Add-VMNetworkAdapterExtendedAcl -VMName $VMName -VMNetworkAdapterName $PublicNIC -Action Allow -Direction Inbound -LocalPort 68 -RemotePort 67 -Protocol UDP -Weight 191

$DenyWeight = 10
foreach ($Dir in $Directions) {
    foreach ($DenyIP in @("192.168.0.0/16", "10.0.0.0/8", "172.16.0.0/12", "::/0")) {
        Add-VMNetworkAdapterExtendedAcl -VMName $VMName -VMNetworkAdapterName $PublicNIC -Action Deny -Direction $Dir -RemoteIPAddress $DenyIP -Weight $DenyWeight; $DenyWeight++
    }
}

# Private-NIC Rules
$PrivWeight = 300
foreach ($Port in $TCPPorts) { Add-VMNetworkAdapterExtendedAcl -VMName $VMName -VMNetworkAdapterName $PrivateNIC -Action Allow -Direction Inbound -LocalPort $Port -Protocol TCP -Weight $PrivWeight -Stateful $true; $PrivWeight++ }
foreach ($Port in $UDPPorts) { Add-VMNetworkAdapterExtendedAcl -VMName $VMName -VMNetworkAdapterName $PrivateNIC -Action Allow -Direction Inbound -LocalPort $Port -Protocol UDP -Weight $PrivWeight -Stateful $true; $PrivWeight++ }

$AbsDenyWeight = 5
foreach ($Dir in $Directions) {
    Add-VMNetworkAdapterExtendedAcl -VMName $VMName -VMNetworkAdapterName $PrivateNIC -Action Deny -Direction $Dir -RemoteIPAddress "0.0.0.0/0" -Weight $AbsDenyWeight; $AbsDenyWeight++
    Add-VMNetworkAdapterExtendedAcl -VMName $VMName -VMNetworkAdapterName $PrivateNIC -Action Deny -Direction $Dir -RemoteIPAddress "::/0" -Weight $AbsDenyWeight; $AbsDenyWeight++
}
Write-Host "Sandbox Applied!"
