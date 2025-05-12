# Etapa 1: build da aplicação usando imagem Alpine
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /app

# Copia os arquivos de projeto e restaura as dependências
COPY Sprint1-API/Sprint1-API.csproj ./Sprint1-API/
RUN dotnet restore ./Sprint1-API/Sprint1-API.csproj

# Copia os demais arquivos e compila a aplicação
COPY Sprint1-API/ ./Sprint1-API/
WORKDIR /app/Sprint1-API
RUN dotnet publish -c Release -o /app/out

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app

# Cria um usuário sem privilégios
RUN adduser -D -g '' appuser

# Copia a aplicação compilada da etapa anterior
COPY --from=build /app/out . 

# Ajusta permissões para o usuário
RUN chown -R appuser /app

# Define o usuário não-root
USER appuser

# Porta da API
EXPOSE 5147

# Define o ambiente como "Development"
ENV ASPNETCORE_ENVIRONMENT=Development

# Comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "Sprint1-API.dll"]
