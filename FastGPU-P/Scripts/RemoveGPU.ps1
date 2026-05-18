param(
	[string]$VMName
)

$ErrorActionPreference = 'Stop'

Remove-VMGpuPartitionAdapter -VMName $VMName