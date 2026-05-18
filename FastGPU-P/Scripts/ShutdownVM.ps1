param(
	[string]$VMName
)

$ErrorActionPreference = 'Stop'

$VM = Get-VM -Name $VMName
if ($VM.State -eq "Running") {
	Stop-VM -Name $VMName -Force
    
	$timeout = 300
	while ((Get-VM -Name $VMName).State -eq "Running" -and $timeout -gt 0) {
		Start-Sleep -s 1
		$timeout--
	}
    
	if ((Get-VM -Name $VMName).State -eq "Running") {
		Stop-VM -Name $VMName -TurnOff -Force
	}
}