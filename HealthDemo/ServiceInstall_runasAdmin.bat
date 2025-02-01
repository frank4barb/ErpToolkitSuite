@echo off
rem Memorizza il path completo del server
set NAME_SERVER=HealthDemo
set CURR_SERVER=%cd%\bin\Release\net8.0\%NAME_SERVER%.exe
rem Visualizza il server
echo Il server corrente è: %CURR_SERVER%
pause
rem carica il server come servizio windows
rem dotnet publish %NAME_SERVER%.csproj -p:Configuration=Release
rem sc create %NAME_SERVER%Service binPath=%CURR_SERVER% start=delayed-auto
SC DELETE %NAME_SERVER%Service 
pause