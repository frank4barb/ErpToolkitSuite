@echo off
echo ============================================
echo   INSTALLAZIONE SERVIZIO WINDOWS: HealthDemo
echo ============================================

REM *** PERCORSO DEL TUO ESEGUIBILE ***
SET ServicePath=C:\erp\HealthDemo\HealthDemo.exe

REM *** NOME DEL SERVIZIO ***
SET ServiceName=HealthDemo

echo.
echo Verifica se il servizio esiste gia'...

sc query %ServiceName% >nul 2>&1
IF %ERRORLEVEL% EQU 0 (
    echo Il servizio esiste gia'. Lo rimuovo...
    sc stop %ServiceName% >nul 2>&1
    sc delete %ServiceName% >nul 2>&1
    timeout /t 2 >nul
)

echo.
echo Installazione in corso...

sc create %ServiceName% binPath= "%ServicePath%" start= delayed-auto DisplayName= "Health Demo Service"

echo.
echo Avvio del servizio...

sc start %ServiceName%

echo.
echo Installazione e avvio completati!
pause