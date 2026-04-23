FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["DotanBooks.sln", "."]
COPY ["DotanBooks/DotanBooks.csproj", "DotanBooks/"]
COPY ["Service/Service.csproj", "Service/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["DTOs/DTOs.csproj", "DTOs/"]
COPY ["Entitiys/Entities.csproj", "Entitiys/"]
COPY ["Utils/Utils.csproj", "Utils/"]

RUN dotnet restore "DotanBooks.sln"

COPY . .
RUN dotnet publish "DotanBooks/DotanBooks.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "DotanBooks.dll"]
