param (
    [int]$bn = 8,
    [string]$branch = "rc3",
	[string]$nugetApiKey = "",
	[string]$nugetSource = "http://nuget.regery.net/v3/index.json"
)

$currentFolder = Get-Location
$buildNumber = ("{0:D5}" -f $bn)
$nugetOutput = "$currentFolder/nugets"
$versionSuffixTemp = "$branch-$buildNumber"
$versionSuffix = $versionSuffixTemp.Split('/') | Select-Object -Last 1
$versionPrefix = Get-Content version.txt
$buildConf = "Release"

if ("$branch" -eq "master"){
	$versionSuffix = "ztm-$buildNumber"
}

Write-Host "currentFolder is:" $currentFolder -foreground "Yellow"
Write-Host "nugetoutput is:" $nugetOutput -foreground "Yellow"
Write-Host "buildConf is:" $buildConf -foreground "Yellow"
Write-Host "versionPrefix is:" $versionPrefix -foreground "Yellow"
Write-Host "versionSuffix is:" $versionSuffix -foreground "Yellow"
Write-Host "nugetSource is:" $nugetSource -foreground "Yellow"

if (Test-Path $nugetOutput){
     Remove-Item $nugetOutput/*.* -Force -Recurse -ErrorAction SilentlyContinue
     Start-Sleep -s 1
}

New-Item $nugetOutput -Force -confirm:$false -ErrorAction SilentlyContinue -ItemType directory


Push-Location
iex 'dotnet msbuild "/t:Restore;Pack" /p:VersionSuffix=$versionSuffix /p:VersionPrefix=$versionPrefix /p:Configuration=$buildConf'
Get-ChildItem "src" -Filter *.csproj -Recurse | 
    Foreach-Object { 
        $projectPath = Split-Path $_.FullName -Parent 
        $projectName = Split-Path $projectPath -Leaf
        Set-Location $projectPath
		Set-Location bin/$buildConf
		Write-Host $projectName
		Write-Host 'dotnet nuget push *$versionSuffix.nupkg -source $nugetSource'
		iex 'dotnet nuget push *$versionSuffix.nupkg -s $nugetSource -k "$nugetApiKey"'
    }
Pop-Location	