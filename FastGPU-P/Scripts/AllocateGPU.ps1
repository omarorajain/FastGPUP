param(
	[string]$VMName,
	[string]$InstancePath,
	[decimal]$GPUResourceAllocationPercentage
)

$ErrorActionPreference = 'SilentlyContinue'
Remove-VMGpuPartitionAdapter -VMName $VMName

$ErrorActionPreference = 'Stop'

# If InstancePath is empty, don't use the flag. Otherwise, use it.
if ([string]::IsNullOrWhiteSpace($InstancePath)) {
	$HostGpu = Get-VMHostPartitionableGpu | Select-Object -First 1
    Add-VMGpuPartitionAdapter -VMName $VMName
}
else {
	$HostGpu = Get-VMHostPartitionableGpu | Where-Object Name -eq $InstancePath
    Add-VMGpuPartitionAdapter -VMName $VMName -InstancePath $InstancePath
}

[double]$Multiplier = $GPUResourceAllocationPercentage / 100.0

[uint64]$vram    = [math]::Round($HostGpu.TotalVRAM * $Multiplier)
[uint64]$encode  = [math]::Round($HostGpu.TotalEncode * $Multiplier)
[uint64]$decode  = [math]::Round($HostGpu.TotalDecode * $Multiplier)
[uint64]$compute = [math]::Round($HostGpu.TotalCompute * $Multiplier)

Set-VMGpuPartitionAdapter -VMName $VMName -MinPartitionVRAM $vram -MaxPartitionVRAM $vram -OptimalPartitionVRAM $vram
Set-VMGpuPartitionAdapter -VMName $VMName -MinPartitionEncode $encode -MaxPartitionEncode $encode -OptimalPartitionEncode $encode
Set-VMGpuPartitionAdapter -VMName $VMName -MinPartitionDecode $decode -MaxPartitionDecode $decode -OptimalPartitionDecode $decode
Set-VMGpuPartitionAdapter -VMName $VMName -MinPartitionCompute $compute -MaxPartitionCompute $compute -OptimalPartitionCompute $compute

Set-VM -GuestControlledCacheTypes $true -VMName $VMName
Set-VM -LowMemoryMappedIoSpace 1Gb -VMName $VMName
Set-VM -HighMemoryMappedIoSpace 32GB -VMName $VMName