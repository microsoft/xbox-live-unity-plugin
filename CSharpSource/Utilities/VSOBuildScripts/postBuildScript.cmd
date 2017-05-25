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

robocopy /NJS /NJH /MT:16 /S /NP %TFS_SourcesDirectory% %TFS_DropLocation%\Source

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
