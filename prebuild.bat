@echo off
setlocal enabledelayedexpansion

call :dequote %1
set solution=%ret%
call :dequote %2
set toolkitver=%ret%

echo Checking for .git\HEAD...
set rev=nongit
if exist "%solution%\.git\HEAD" (
	echo Reading .git\HEAD...
	set /p head=<"%solution%\.git\HEAD"
	if "!head:~0,4!" == "ref:" (
		echo Reading .git\!head:~5!...
		if exist "%solution%\.git\!head:~5!" set /p commit=<"%solution%\.git\!head:~5!"
	) else (
		set commit=!head!
	)
	if not "!commit!" == ""	(
		echo Found commit: !commit!
		set rev=!commit:~0,8!
	) else echo Unable to find commit
)

echo Reading ToolkitVersion.cs_dist...
set origstr=00000000
for /f "tokens=* delims=" %%i in ('type "%toolkitver%_dist"') do (
	set str=%%i
	set newstr=!str:%origstr%=%rev%!
	echo !newstr! >> tempfile.txt
)
echo Writing ToolkitVersion.cs...
move /y tempfile.txt "%toolkitver%"
echo Done
exit /b 0

:dequote
set remquote=%~1
set ret=%remquote%
