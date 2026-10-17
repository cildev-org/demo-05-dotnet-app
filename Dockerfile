# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:f7e28a0a3f6f9235929ec19274fd0ddf8534e9159f99fac602ac73304f1ce99f AS build
WORKDIR /src
COPY LabApp.csproj ./
RUN dotnet restore
COPY Program.cs ./
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:1601380775673021a23ea5985669f1daf03719faaa684f839da1752fb48cfc7f
WORKDIR /app
COPY --from=build /app ./
# /data stands in for what a real app keeps next to its upload folder.
RUN mkdir -p /data/uploads && chown -R app:app /data
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "LabApp.dll"]
