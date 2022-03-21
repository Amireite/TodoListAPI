FROM mcr.microsoft.com/dotnet/sdk:3.1-bionic AS build
WORKDIR /source

COPY *.sln .
COPY TodoListAPI/*.csproj ./TodoListAPI/
RUN dotnet restore

COPY TodoListAPI/. ./TodoListAPI/
WORKDIR /source/TodoListAPI
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:3.1-bionic
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "TodoListAPI.dll"]