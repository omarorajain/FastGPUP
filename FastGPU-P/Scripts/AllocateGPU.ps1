param(
	[string]$VMName,
	[string]$InstancePath,
	[decimal]$GPUResourceAllocationPercentage
)

$ErrorActionPreference = 'Stop'

# If InstancePath is empty, don't use the flag. Otherwise, use it.
if ([string]::IsNullOrWhiteSpace($InstancePath)) {
	Add-VMGpuPartitionAdapter -VMName $VMName
}
else {
	Add-VMGpuPartitionAdapter -VMName $VMName -InstancePath $InstancePath
}

[float]$divider = [math]::Round($(100 / $GPUResourceAllocationPercentage), 2)

Set-VMGpuPartitionAdapter -VMName $VMName -MinPartitionVRAM ([math]::round($(1000000000 / $divider))) -MaxPartitionVRAM ([math]::round($(1000000000 / $divider))) -OptimalPartitionVRAM ([math]::round($(1000000000 / $divider)))
Set-VMGpuPartitionAdapter -VMName $VMName -MinPartitionEncode ([math]::round($(18446744073709551615 / $divider))) -MaxPartitionEncode ([math]::round($(18446744073709551615 / $divider))) -OptimalPartitionEncode ([math]::round($(18446744073709551615 / $divider)))
Set-VMGpuPartitionAdapter -VMName $VMName -MinPartitionDecode ([math]::round($(1000000000 / $divider))) -MaxPartitionDecode ([math]::round($(1000000000 / $divider))) -OptimalPartitionDecode ([math]::round($(1000000000 / $divider)))
Set-VMGpuPartitionAdapter -VMName $VMName -MinPartitionCompute ([math]::round($(1000000000 / $divider))) -MaxPartitionCompute ([math]::round($(1000000000 / $divider))) -OptimalPartitionCompute ([math]::round($(1000000000 / $divider)))

Set-VM -GuestControlledCacheTypes $true -VMName $VMName
Set-VM -LowMemoryMappedIoSpace 1Gb -VMName $VMName
Set-VM -HighMemoryMappedIoSpace 32GB -VMName $VMName