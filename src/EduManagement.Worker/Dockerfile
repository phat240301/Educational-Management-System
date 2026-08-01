FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/EduManagement.Worker/EduManagement.Worker.csproj", "src/EduManagement.Worker/"]
COPY ["src/EduManagement.Core/EduManagement.Core.csproj", "src/EduManagement.Core/"]
COPY ["src/EduManagement.Infrastructure/EduManagement.Infrastructure.csproj", "src/EduManagement.Infrastructure/"]
RUN dotnet restore "src/EduManagement.Worker/EduManagement.Worker.csproj"

COPY src/EduManagement.Worker/ src/EduManagement.Worker/
COPY src/EduManagement.Core/ src/EduManagement.Core/
COPY src/EduManagement.Infrastructure/ src/EduManagement.Infrastructure/

WORKDIR /src/src/EduManagement.Worker
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EduManagement.Worker.dll"]
