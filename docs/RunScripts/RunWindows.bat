cd "bin\"
start dotnet "CryptoManager.dll"
timeout /t 5
start http://localhost:5000