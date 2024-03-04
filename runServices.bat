start /B azurite --location C:\AzuriteData --skipApiVersionCheck
cd Services
cd AnswerMe/AnswerMe.Api
start /B dotnet run --urls "https://localhost:7156;http://localhost:5177"
cd ..\..
cd IdentityServer/IdentityServer.Api
start /B dotnet run --urls "https://localhost:7216;http://localhost:5154"
cd ..\..
cd ObjectStorage/ObjectStorage.Api
start /B dotnet run --urls "https://localhost:7205;http://localhost:5238"

