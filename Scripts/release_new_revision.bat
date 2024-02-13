::TODO
:: - Find out the current version
:: - Increment it by build +1
:: - Change the latest_version.txt file to new version
:: - Change the AssemblyVersion version to the same new version
:: - Change ReadMe download link to point to the new version
:: - Build the project in Release.
:: - Zip up the DofusSwap.exe and name it {new version}.zip
:: - Move this zip into Downloadables folder
:: - git commit and push latest_version, readme and .zip file with commit message Version {new version} and tag this commit with {new version}

@echo off

:: NEW VERSION VALUE ? 
set /p newversion=< ..\latest_version.txt
echo %newversion%

for /F "tokens=1,2,3 delims=." %%a in ("%newversion%") do (
   set Major=%%a
   set Minor=%%b
   set Revision=%%c
)
set /A NewRevision = Revision + 1

echo Major: %Major%, Minor: %Minor%, Revision: %NewRevision%

set /A NextVersion = %Major%.%Minor%.%NewRevision%
echo Next Version: %NextVersion%


:: Update latest_version.txt 


set /p DUMMY=Hit ENTER to continue...