#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
COPY ["ABCPdfDemo/ABCPdf.Demo.csproj", "ABCPdfDemo/"]
COPY ["ABCPdfDemo/ABCpdf.dll", "/usr/local/lib/"]
COPY ["ABCPdfDemo/ABCpdf12-64.dll", "/usr/local/lib/"]
EXPOSE 80
EXPOSE 443


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /

RUN dotnet restore "ABCPdfDemo/ABCPdf.Demo.csproj"
COPY . .
WORKDIR "/ABCPdfDemo"
RUN dotnet build "ABCPdf.Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ABCPdf.Demo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ABCPdf.Demo.dll"]