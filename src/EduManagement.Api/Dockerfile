FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/EduManagement.Api/EduManagement.Api.csproj", "src/EduManagement.Api/"]
COPY ["src/EduManagement.Core/EduManagement.Core.csproj", "src/EduManagement.Core/"]
COPY ["src/EduManagement.Infrastructure/EduManagement.Infrastructure.csproj", "src/EduManagement.Infrastructure/"]
RUN dotnet restore "src/EduManagement.Api/EduManagement.Api.csproj"

COPY src/EduManagement.Api/ src/EduManagement.Api/
COPY src/EduManagement.Core/ src/EduManagement.Core/
COPY src/EduManagement.Infrastructure/ src/EduManagement.Infrastructure/

WORKDIR /src/src/EduManagement.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Npgsql probes for GSSAPI (Kerberos) support during connection negotiation; without
# this lib it logs a harmless "cannot open shared object file" warning on every connect.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "EduManagement.Api.dll"]
