param(
    [string]$VMName,
    [string]$GPUName,
    [string]$Hostname
)

$ErrorActionPreference = 'Stop'

If ($GPUName -eq "Auto") {
    $PartitionableGPUList = Get-CimInstance -Class Msvm_PartitionableGpu -Namespace root\virtualization\v2 
    $DevicePathName = $PartitionableGPUList.Name | Select-Object -First 1
    $GPU = Get-PnpDevice | Where-Object { ($_.DeviceID -like "*$($DevicePathName.Substring(8,16))*") -and ($_.Status -eq "OK") } | Select-Object -First 1
    $ActualGPUName = $GPU.Friendlyname
} Else {
    $ActualGPUName = $GPUName
}

Function Add-VMGpuPartitionAdapterFiles {
    param(
        [string]$Hostname,
        [string]$DriveLetter,
        [string]$TargetGPUName
    )

    If (!($DriveLetter -like "*:*")) {
        $DriveLetter = $DriveLetter + ":"
    }

    $GPU = Get-PnpDevice | Where-Object { ($_.Name -eq "$TargetGPUName") -and ($_.Status -eq "OK") } | Select-Object -First 1
    $GPUServiceName = $GPU.Service
    
    $Drivers = Get-CimInstance -ClassName Win32_PNPSignedDriver | Where-Object { $_.DeviceName -eq "$TargetGPUName" }
    New-Item -ItemType Directory -Path "$DriveLetter\windows\system32\HostDriverStore" -Force | Out-Null
    
    $servicePath = (Get-CimInstance -ClassName Win32_SystemDriver | Where-Object { $_.Name -eq "$GPUServiceName" }).Pathname
    $ServiceDriverDir = $ServicePath.split('\')[0..5] -join ('\')
    $ServiceDriverDest = ("$DriveLetter" + "\" + $($ServicePath.split('\')[1..5] -join ('\'))).Replace("DriverStore", "HostDriverStore")
    
    if (!(Test-Path $ServiceDriverDest)) {
        Copy-item -path "$ServiceDriverDir" -Destination "$ServiceDriverDest" -Recurse
    }

    foreach ($d in $drivers) {
        $DriverFiles = @()
        $ModifiedDeviceID = $d.DeviceID -replace "\\", "\\"
        $Antecedent = "\\" + $Hostname + "\ROOT\cimv2:Win32_PNPSignedDriver.DeviceID=`"$ModifiedDeviceID`""
        
        $DriverFiles += Get-CimInstance -ClassName Win32_PNPSignedDriverCIMDataFile | Where-Object { $_.Antecedent -eq $Antecedent }
        $DriverName = $d.DeviceName
        
        if ($DriverName -like "NVIDIA*") {
            New-Item -ItemType Directory -Path "$DriveLetter\Windows\System32\drivers\Nvidia Corporation\" -Force | Out-Null
        }
        
        foreach ($i in $DriverFiles) {
            $path = $i.Dependent.Split("=")[1] -replace '\\\\', '\'
            $path2 = $path.Substring(1, $path.Length - 2)
            
            If ($path2 -like "c:\windows\system32\driverstore\*") {
                $DriverDir = $path2.split('\')[0..5] -join ('\')
                $driverDest = ("$DriveLetter" + "\" + $($path2.split('\')[1..5] -join ('\'))).Replace("driverstore", "HostDriverStore")
                if (!(Test-Path $driverDest)) {
                    Copy-item -path "$DriverDir" -Destination "$driverDest" -Recurse
                }
            }
            Else {
                $ParseDestination = $path2.Replace("c:", "$DriveLetter")
                $Destination = $ParseDestination.Substring(0, $ParseDestination.LastIndexOf('\'))
                if (!$(Test-Path -Path $Destination)) {
                    New-Item -ItemType Directory -Path $Destination -Force | Out-Null
                }
                Copy-Item $path2 -Destination $Destination -Force
            }
        }
    }
}

$VM = Get-VM -VMName $VMName
$VHD = Get-VHD -VMId $VM.VMId

If ($VM.state -eq "Running") {
    [bool]$state_was_running = $true
}

if ($VM.state -ne "Off") {
    Write-Host "Attempting to shutdown VM..."
    Stop-VM -Name $VMName -Force
} 

While ($VM.State -ne "Off") {
    Start-Sleep -s 3
    Write-Host "Waiting for VM to shutdown - make sure there are no unsaved documents..."
}

Write-Host "Mounting Drive..."
$DiskNumber = (Mount-VHD -NoDriveLetter -Path $VHD.Path -PassThru | Get-Disk).Number
$PartitionNumber = (Get-Partition -DiskNumber $DiskNumber | Where-Object { $_.Type -eq "Basic" }).PartitionNumber
$UsedLetters = Get-CimInstance -ClassName Win32_LogicalDisk | Select-Object -ExpandProperty DeviceID | ForEach-Object { $_.ToString()[0] }
$DriveLetter = [char[]](67..90) | Where-Object { $_ -notin $UsedLetters } | Select-Object -First 1
Set-Partition -DiskNumber $DiskNumber -PartitionNumber $PartitionNumber -NewDriveLetter $DriveLetter

# Version Check
$DriveLetter = $DriveLetter + ":"
$HostDriver = Get-CimInstance -ClassName Win32_PNPSignedDriver | Where-Object { $_.DeviceName -eq $ActualGPUName } | Select-Object -First 1
$HostVersion = $HostDriver.DriverVersion

$VersionTrackerPath = "$DriveLetter\Windows\System32\HostDriverStore\FastGPUP_Version.txt"
$GuestVersion = "Unknown"

if (Test-Path $VersionTrackerPath) {
    $GuestVersion = (Get-Content $VersionTrackerPath).Trim()
}

if ($HostVersion -eq $GuestVersion) {
    Write-Host "SKIP_DRIVER_UPDATE"
    Write-Host "Driver versions match ($HostVersion). Skipping massive file copy."
} else {
    Write-Host "Version mismatch (Host: $HostVersion, Guest: $GuestVersion). Copying GPU Files..."
    Add-VMGPUPartitionAdapterFiles -Hostname $Hostname -DriveLetter $DriveLetter -TargetGPUName $ActualGPUName
    
    New-Item -ItemType Directory -Path "$DriveLetter\Windows\System32\HostDriverStore" -Force | Out-Null
    $HostVersion | Out-File -FilePath $VersionTrackerPath -Encoding UTF8 -Force
}

Write-Host "Dismounting Drive..."
Dismount-VHD -Path $VHD.Path

If ($state_was_running) {
    Write-Host "Previous State was running so starting VM..."
    Start-VM $VMName
}