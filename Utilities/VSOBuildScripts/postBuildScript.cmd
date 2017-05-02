if "%1" == "local" goto testlocal
goto start

:testlocal
set TFS_DropLocation=c:\test
mkdir %TFS_DropLocation%
set TFS_VersionNumber=1701.10000
set TFS_SourcesDirectory=%CD%\..\..
goto ready

:start
if "%XES_SERIALPOSTBUILDREADY%" == "True" goto ready
goto done

:ready

set CSharpBinFolder=\\edge-svcs\release\XboxLiveSDK\Xbox_Live_Api_CSharp\Latest.tst
set exportPath=%TFS_SourcesDirectory%\Assets\Xbox Live
set projectPath=%TFS_DropLocation%\Packages\XboxLive.unitypackage
set libPath=%exportPath%\Libs

mkdir "%exportPath%\Libs"

robocopy /NJS /NJH /MT:16 /S /NP "%CSharpBinFolder%\Source\binaries\Layout\Release" "%libPath%"

mkdir %TFS_DropLocation%\Packages
"C:\Program Files\Unity\Editor\Unity.exe" -ea SilentlyContinue -batchmode -logFile "%TFS_DropLocation%\unity.log" -projectPath "%TFS_SourcesDirectory%" -exportPackage "Assets\Xbox Live" "%projectPath%" -quit
robocopy /NJS /NJH /MT:16 /S /NP "%exportPath%" "%TFS_DropLocation%\Assets\Xbox Live"

echo Running postBuildScript.cmd
echo on
if "%1" == "local" goto skipEmail
set MSGTITLE="BUILD: %BUILD_SOURCEVERSIONAUTHOR% %BUILD_DEFINITIONNAME% %BUILD_SOURCEBRANCH% = %agent.jobstatus%"
set MSGBODY="%TFS_DROPLOCATION%    https://microsoft.visualstudio.com/OS/_build/index?buildId=%BUILD_BUILDID%&_a=summary"
call \\scratch2\scratch\jasonsa\tools\send-build-email.cmd %MSGTITLE% %MSGBODY% 

:skipEmail
echo.
echo Done postBuildScript.cmd
echo.
endlocal

:done
