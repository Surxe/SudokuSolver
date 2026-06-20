# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["Sudoku.Api.csproj", "./"]
RUN dotnet restore "Sudoku.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src"
RUN dotnet build "Sudoku.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Sudoku.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Sudoku.Api.dll"]
