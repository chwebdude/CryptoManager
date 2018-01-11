@echo off

IF EXIST data (
 SET wait=2
) ELSE (
 SET wait=10
 ECHO Firststart: Wait till initialization...
)

cd bin
start CryptoManager.exe
timeout /t %Wait%
start http://localhost:5000