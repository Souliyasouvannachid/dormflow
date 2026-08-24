# Dockerfile ของ DormFlow API
# ต้อง build จากรากของ repository เพื่อให้ Directory.Build.props และ Directory.Packages.props ถูกคัดลอกไปด้วย
#   docker build -f src/DormFlow.Api/Dockerfile -t dormflow-api .

# ---------- ขั้นตอน restore ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /source

# คัดลอกเฉพาะไฟล์ที่กำหนดเวอร์ชันแพ็กเกจก่อน
# เพื่อให้ layer cache ของ restore ถูกใช้ซ้ำได้เมื่อโค้ดเปลี่ยนแต่รายการแพ็กเกจไม่เปลี่ยน
COPY Directory.Build.props Directory.Packages.props global.json ./
COPY src/Directory.Build.props src/
COPY src/DormFlow.Domain/DormFlow.Domain.csproj src/DormFlow.Domain/
COPY src/DormFlow.Contracts/DormFlow.Contracts.csproj src/DormFlow.Contracts/
COPY src/DormFlow.Application/DormFlow.Application.csproj src/DormFlow.Application/
COPY src/DormFlow.Infrastructure/DormFlow.Infrastructure.csproj src/DormFlow.Infrastructure/
COPY src/DormFlow.Api/DormFlow.Api.csproj src/DormFlow.Api/

RUN dotnet restore src/DormFlow.Api/DormFlow.Api.csproj

# ---------- ขั้นตอน publish ----------
FROM restore AS publish
COPY src/ src/
RUN dotnet publish src/DormFlow.Api/DormFlow.Api.csproj \
    --no-restore \
    --configuration Release \
    --output /app/publish

# ---------- อิมเมจสำหรับรันจริง ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    Https__EnableRedirection=false

# curl จำเป็นสำหรับ HEALTHCHECK ของ docker compose เพราะอิมเมจ aspnet ไม่ได้ติดตั้งมาให้
RUN apt-get update \
    && apt-get install --no-install-recommends -y curl \
    && rm -rf /var/lib/apt/lists/*

# โฟลเดอร์เก็บไฟล์ Log ต้องเขียนได้โดยผู้ใช้ที่ไม่ใช่ root
RUN mkdir -p /app/logs && chown -R $APP_UID:$APP_UID /app

COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .

# รันด้วยผู้ใช้ที่ไม่มีสิทธิ์ root ตามค่ามาตรฐานของอิมเมจ .NET
USER $APP_UID

EXPOSE 8080

ENTRYPOINT ["dotnet", "DormFlow.Api.dll"]
