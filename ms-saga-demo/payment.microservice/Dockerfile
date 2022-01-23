#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["payment.microservice/payment.microservice.csproj", "payment.microservice/"]
RUN dotnet restore "payment.microservice/payment.microservice.csproj"
COPY . .
WORKDIR "/src/payment.microservice"
RUN dotnet build "payment.microservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "payment.microservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "payment.microservice.dll"]