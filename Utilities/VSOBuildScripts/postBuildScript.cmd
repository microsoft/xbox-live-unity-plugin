@echo on
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

set packageDir=%TFS_DropLocation%\Packages
set packagePath=%packageDir%\XboxLive.unitypackage
mkdir %packageDir%

set remoteAssetsPath=%TFS_DropLocation%\Source\Assets
set localAssetsPath=%TFS_SourcesDirectory%\Assets

echo [start] Copying output from other build agents locally from %remoteAssetsPath% to %localAssetsPath%
robocopy /NJS /NJH /MT:16 /S /NP %remoteAssetsPath% %localAssetsPath%
echo [done] Copying %remoteAssetsPath% to %localAssetsPath%

echo [start] Copying local sources to build share from %TFS_SourcesDirectory% to %TFS_DropLocation%\Source
robocopy /NJS /NJH /MT:16 /S /NP %TFS_SourcesDirectory% %TFS_DropLocation%\Source
echo [done] Copying %TFS_SourcesDirectory% to %TFS_DropLocation%\Source

set SRC_GAMESAVE=%localAssetsPath%\Xbox Live\GameSave
set SRC_GAMESAVE_PACKAGE=%localAssetsPath%\Xbox Live\GameSave\GameSave.unitypackage
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

"C:\Program Files\Unity\Editor\Unity.exe" -ea SilentlyContinue -batchmode -logFile "%TFS_DropLocation%\unity.log" -projectPath "%TFS_SourcesDirectory%" -exportPackage "Assets" "%packagePath%" -quit

rmdir /q /s "%SRC_GAMESAVE%"
robocopy /NJS /NJH /MT:16 /S /NP "%DEST_GAMESAVE%" "%SRC_GAMESAVE%"
rmdir /q /s "%DEST_GAMESAVE%"
del "%SRC_GAMESAVE%\GameSave.unitypackage"
del "%SRC_GAMESAVE%\GameSave.unitypackage.meta"

echo Running postBuildScript.cmd
echo on
if "%1" == "local" goto skipEmail
set MSGTITLE="BUILD: %BUILD_SOURCEVERSIONAUTHOR% %BUILD_DEFINITIONNAME% %BUILD_SOURCEBRANCH% = %agent.jobstatus%"
set MSGBODY="%TFS_DROPLOCATION%    https://microsoft.visualstudio.com/DefaultCollection/Xbox.Services/_build/index?buildId=%BUILD_BUILDID%&_a=summary"
call \\scratch2\scratch\jasonsa\tools\send-build-email.cmd %MSGTITLE% %MSGBODY% 

:skipEmail
echo.
echo Done postBuildScript.cmd
echo.
endlocal

:done
