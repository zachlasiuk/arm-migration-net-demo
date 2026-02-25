# syntax=docker/dockerfile:1

# Intentionally force x86/amd64 for demo
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

# Intentionally publish for linux-x64 (x86-only)
RUN dotnet restore
RUN dotnet publish -c Release -r linux-x64 --self-contained false -o /out

FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["./SimdHashDemo"]