param(
	[string]$GPUName
)

$ErrorActionPreference = 'Stop'

$PartitionableGPUList = Get-CimInstance -Class Msvm_PartitionableGpu -Namespace root\virtualization\v2 
$DeviceID = ((Get-CimInstance -ClassName Win32_PNPSignedDriver | Where-Object { $_.DeviceName -eq $GPUName }).HardwareID).Split('\')[1]
$DevicePathName = ($PartitionableGPUList | Where-Object Name -like "*$DeviceID*").Name

$DevicePathName