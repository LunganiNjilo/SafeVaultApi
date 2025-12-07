FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY SafeVaultApi.sln .
COPY SafeVaultApi/SafeVaultApi.csproj SafeVaultApi/
COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY SafeVaultApi.Tests/SafeVaultApi.Tests.csproj SafeVaultApi.Tests/
COPY tests/SafeVault.FunctionalTests/SafeVault.FunctionalTests.csproj tests/SafeVault.FunctionalTests/

RUN dotnet restore SafeVaultApi.sln

COPY . .
RUN dotnet build tests/SafeVault.FunctionalTests/SafeVault.FunctionalTests.csproj -c Release

WORKDIR /app/tests/SafeVault.FunctionalTests

CMD ["dotnet", "test", "--no-build", "--logger:console;verbosity=normal"]

