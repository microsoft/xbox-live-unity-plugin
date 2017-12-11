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

echo [start] Copying %TFS_SourcesDirectory% to %TFS_DropLocation%\Source
robocopy /NJS /NJH /MT:16 /S /NP %TFS_SourcesDirectory% %TFS_DropLocation%\Source
echo [done] Copying %TFS_SourcesDirectory% to %TFS_DropLocation%\Source

echo [start] Binplacing c dlls into Plugins folder
set PLUGIN_BUILD_SHARE_x86=%TFS_DropLocation%\Release\x86
set PLUGIN_BUILD_SHARE_x64=%TFS_DropLocation%\Release\x64
set PLUGIN_BIN_SHARE_x86=%TFS_DropLocation%\Source\Assets\Plugins\x86
set PLUGIN_BIN_SHARE_x64=%TFS_DropLocation%\Source\Assets\Plugins\x64
copy %PLUGIN_BUILD_SHARE_x86%\Microsoft.Xbox.Services.140.UWP.C.dll %PLUGIN_BIN_SHARE_x86%\Microsoft.Xbox.Services.140.UWP.C.dll
copy %PLUGIN_BUILD_SHARE_x64%\Microsoft.Xbox.Services.140.UWP.C.dll %PLUGIN_BIN_SHARE_x64%\Microsoft.Xbox.Services.140.UWP.C.dll
echo [done] Binplacing c dlls into Plugins folder

set exportPath=%TFS_DropLocation%\Source\Assets
set projectPath=%TFS_DropLocation%\Packages\XboxLive.unitypackage
set libPath=%exportPath%\Xbox Live\Libs

mkdir "%exportPath%\Xbox Live\Libs"

robocopy /NJS /NJH /MT:16 /S /NP "%TFS_DropLocation%\UWP" "%libPath%\UWP"
robocopy /NJS /NJH /MT:16 /S /NP "%TFS_DropLocation%\UnityEditor" "%libPath%\UnityEditor"

mkdir %TFS_DropLocation%\Packages

set SRC_GAMESAVE=%exportPath%\Xbox Live\GameSave
set SRC_GAMESAVE_PACKAGE=%exportPath%\Xbox Live\GameSave\GameSave.unitypackage
set DEST_GAMESAVE=%TFS_DropLocation%\TempGameSave

"C:\Program Files\Unity\Editor\Unity.exe" -ea SilentlyContinue -batchmode -logFile "%TFS_DropLocation%\gamesave-unity.log" -projectPath "%TFS_DropLocation%\Source" -exportPackage "Assets\Xbox Live\GameSave" "%SRC_GAMESAVE_PACKAGE%" -quit

rmdir /q /s "%DEST_GAMESAVE%"
robocopy /NJS /NJH /MT:16 /S /NP "%SRC_GAMESAVE%" "%DEST_GAMESAVE%"
rmdir /q /s "%SRC_GAMESAVE%"
del "%SRC_GAMESAVE%"
del "%SRC_GAMESAVE%.meta"
mkdir "%SRC_GAMESAVE%"
copy "%DEST_GAMESAVE%\readme.txt" "%SRC_GAMESAVE%"
copy "%DEST_GAMESAVE%\GameSave.unitypackage" "%SRC_GAMESAVE%"


set DEST_SCENES=%TFS_DropLocation%\TempScenes
rmdir /q /s "%DEST_SCENES%"
mkdir "%DEST_SCENES%"
robocopy /NJS /NJH /MT:16 /S /NP "%exportPath%\Scenes" "%DEST_SCENES%\Scenes"
copy "%exportPath%\Scenes.meta" "%DEST_SCENES%"
rmdir /q /s "%exportPath%\Scenes"
del "%exportPath%\Scenes"
del "%exportPath%\Scenes.meta"

"C:\Program Files\Unity\Editor\Unity.exe" -ea SilentlyContinue -batchmode -logFile "%TFS_DropLocation%\unity.log" -projectPath "%TFS_DropLocation%\Source" -exportPackage "Assets" "%projectPath%" -quit

rmdir /q /s "%SRC_GAMESAVE%"
robocopy /NJS /NJH /MT:16 /S /NP "%DEST_GAMESAVE%" "%SRC_GAMESAVE%"
rmdir /q /s "%DEST_GAMESAVE%"
del "%SRC_GAMESAVE%\GameSave.unitypackage"
del "%SRC_GAMESAVE%\GameSave.unitypackage.meta"

robocopy /NJS /NJH /MT:16 /S /NP "%DEST_SCENES%\Scenes" "%exportPath%\Scenes" 
copy "%DEST_SCENES%\Scenes.meta" "%exportPath%\Scenes.meta"
rmdir /q /s "%DEST_SCENES%"


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
