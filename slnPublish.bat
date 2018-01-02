@echo off

rem  Running a .bat file as administrator - Correcting current directory 
@setlocal enableextensions
@cd /d "%~dp0"

powershell .\_slnPublish.ps1

SET /p keyCode="Press enter key to exit"
