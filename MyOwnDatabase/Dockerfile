# Step 1: Use the official .NET SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/publish --no-restore

# Step 2: Create the final runtime image (smaller)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port your API listens on (change if you use something else)
EXPOSE 8080

# Set the entrypoint
ENTRYPOINT ["dotnet", "MyOwnDatabase.dll"]