param (
    [int]$bn = 0,
    [string]$branch = "develop",
    [string]$progetUser = "",
    [string]$progetPwd = "",
	[string]$source = "http://proget.regery.com/nuget/backcore"
)

$currentFolder = Get-Location
$buildNumber = ("{0:D5}" -f $bn)
$nugetSource = $source
$nugetOutput = "$currentFolder\nugets"
$versionSuffixTemp = "$branch-$buildNumber"
$versionSuffix = $versionSuffixTemp.Split('/') | Select-Object -Last 1
$versionPrefix = Get-Content version.txt
$buildConf = "Debug"
if ("$branch" -eq "master"){
	$buildConf =  "Release"
	$versionSuffix = "ztm-$buildNumber"
}

Write-Host "currentFolder is:" $currentFolder -foreground "Yellow"
Write-Host "nugetoutput is:" $nugetOutput -foreground "Yellow"
Write-Host "buildConf is:" $buildConf -foreground "Yellow"
Write-Host "versionPrefix is:" $versionPrefix -foreground "Yellow"
Write-Host "versionSuffix is:" $versionSuffix -foreground "Yellow"
Write-Host "nugetSource is:" $nugetSource -foreground "Yellow"

if (Test-Path $nugetOutput){
     Remove-Item $nugetOutput\*.* -Force -Recurse -ErrorAction SilentlyContinue
     Start-Sleep -s 1
}

mkdir $nugetOutput -Force -confirm:$false -ErrorAction SilentlyContinue

Push-Location
iex 'dotnet pack /p:VersionPrefix=$versionPrefix --version-suffix "$versionSuffix" --configuration $buildConf --output "$nugetOutput"'
Pop-Location	