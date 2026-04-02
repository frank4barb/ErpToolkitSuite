@echo off
echo ============================================
echo   RIMOZIONE SERVIZIO WINDOWS: HealthDemo
echo ============================================

SET ServiceName=HealthDemo

echo Fermando il servizio...
sc stop %ServiceName% >nul 2>&1

echo Eliminando il servizio...
sc delete %ServiceName% >nul 2>&1

echo.
echo Servizio rimosso!
pause
