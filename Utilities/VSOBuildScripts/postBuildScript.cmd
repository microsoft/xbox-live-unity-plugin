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

set exportPath=%TFS_SourcesDirectory%\Assets\Xbox Live
set projectPath=%TFS_DropLocation%\Packages\XboxLive.unitypackage
set libPath=%exportPath%\Libs

mkdir "%exportPath%\Libs"

robocopy /NJS /NJH /MT:16 /S /NP "%TFS_SourcesDirectory%\Source\CSharpSource\binaries\Layout\Release" "%libPath%"

mkdir %TFS_DropLocation%\Packages

set SRC_GAMESAVE=%exportPath%\GameSave
set SRC_GAMESAVE_PACKAGE=%exportPath%\GameSave\GameSave.unitypackage
set DEST_GAMESAVE=%TFS_DropLocation%\TempGameSave

"C:\Program Files\Unity\Editor\Unity.exe" -ea SilentlyContinue -batchmode -logFile "%TFS_DropLocation%\gamesave-unity.log" -projectPath "%TFS_SourcesDirectory%" -exportPackage "Assets\Xbox Live\GameSave" "%SRC_GAMESAVE_PACKAGE%" -quit

rmdir /q /s "%DEST_GAMESAVE%"
robocopy /NJS /NJH /MT:16 /S /NP "%SRC_GAMESAVE%" "%DEST_GAMESAVE%"
rmdir /q /s "%SRC_GAMESAVE%"
del "%SRC_GAMESAVE%"
del "%SRC_GAMESAVE%.meta"
mkdir "%SRC_GAMESAVE%"
copy "%DEST_GAMESAVE%\readme.txt" "%SRC_GAMESAVE%"
copy "%DEST_GAMESAVE%\GameSave.unitypackage" "%SRC_GAMESAVE%"

"C:\Program Files\Unity\Editor\Unity.exe" -ea SilentlyContinue -batchmode -logFile "%TFS_DropLocation%\unity.log" -projectPath "%TFS_SourcesDirectory%" -exportPackage "Assets\Xbox Live" "%projectPath%" -quit

rmdir /q /s "%SRC_GAMESAVE%"
robocopy /NJS /NJH /MT:16 /S /NP "%DEST_GAMESAVE%" "%SRC_GAMESAVE%"
rmdir /q /s "%DEST_GAMESAVE%"
del "%SRC_GAMESAVE%\GameSave.unitypackage"
del "%SRC_GAMESAVE%\GameSave.unitypackage.meta"
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
