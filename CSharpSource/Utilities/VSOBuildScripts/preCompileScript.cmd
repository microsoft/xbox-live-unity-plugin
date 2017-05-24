echo Running preCompileScript.cmd

call %TFS_SourcesDirectory%\Utilities\VSOBuildScripts\setBuildVersion.cmd
for /f "tokens=2 delims==" %%G in ('wmic os get localdatetime /value') do set datetime=%%G

set DATETIME_YEAR=%datetime:~0,4%
set DATETIME_SHORT_YEAR=%datetime:~2,4%
set DATETIME_MONTH=%datetime:~4,2%
set DATETIME_DAY=%datetime:~6,2%

rem format release numbers
FOR /F "TOKENS=1 eol=/ DELIMS=. " %%A IN ("%TFS_VersionNumber%") DO SET SDK_POINT_NAME_YEARMONTH=%%A
FOR /F "TOKENS=2 eol=/ DELIMS=. " %%A IN ("%TFS_VersionNumber%") DO SET SDK_POINT_NAME_DAYVER=%%A
set SDK_POINT_NAME_YEAR=%DATETIME_YEAR%
set SDK_POINT_NAME_MONTH=%DATETIME_MONTH%
set SDK_POINT_NAME_DAY=%DATETIME_DAY%
set SDK_POINT_NAME_VER=%SDK_POINT_NAME_DAYVER:~2,9%

set SDK_RELEASE_NAME=%SDK_RELEASE_YEAR:~2,2%%SDK_RELEASE_MONTH%
set LONG_SDK_RELEASE_NAME=%SDK_RELEASE_NAME%-%SDK_POINT_NAME_YEAR%%SDK_POINT_NAME_MONTH%%SDK_POINT_NAME_DAY%-%SDK_POINT_NAME_VER%
set NUGET_VERSION_NUMBER=%SDK_RELEASE_YEAR%.%SDK_RELEASE_MONTH%.%SDK_POINT_NAME_YEAR%%SDK_POINT_NAME_MONTH%%SDK_POINT_NAME_DAY%.%SDK_POINT_NAME_VER%

rem 2017.05.1705.19005
set DLL_VERSION_NUMBER=%SDK_RELEASE_YEAR%.%SDK_RELEASE_MONTH%.%DATETIME_SHORT_YEAR%%SDK_POINT_NAME_MONTH%.%SDK_POINT_NAME_DAY%%SDK_POINT_NAME_VER%

set BUILD_VERSION_FILE=%TFS_SourcesDirectory%\Source\Microsoft.Xbox.Services.UWP.CSharp\Properties\AssemblyInfo.cs

%TFS_SourcesDirectory%\Utilities\VSOBuildScripts\FindAndReplace.exe %BUILD_VERSION_FILE% "1.0.0.0" "%DLL_VERSION_NUMBER%"

echo Done preCompileScript.cmd
:done