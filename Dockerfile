FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/MermaidEditor/MermaidEditor.csproj", "MermaidEditor/"]
RUN dotnet restore "MermaidEditor/MermaidEditor.csproj"
COPY src/MermaidEditor/ MermaidEditor/
WORKDIR "/src/MermaidEditor"
RUN dotnet build "MermaidEditor.csproj" -c Release -o /app/build
RUN dotnet publish "MermaidEditor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]