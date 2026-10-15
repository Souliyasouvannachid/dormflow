# Dockerfile ຂອງ DormFlow API
# ຕ້ອງ build ຈາກຮາກຂອງ repository ດັ່ງນັ້ນ Directory.Build.props ແລະ Directory.Packages.props ໄດ້ຖືກຄັດລອກເຊັ່ນກັນ
#   docker build -f src/DormFlow.Api/Dockerfile -t dormflow-api .

# ---------- ຂັ້ນຕອນ restore ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /source

# ສຳເນົາສະເພາະໄຟລ໌ທີ່ເປັນເວີຊັນຂອງແພັກເກດກ່ອນ
# ດັ່ງນັ້ນ layer cache ຂອງ restore ສາມາດໃຊ້ຄືນໃຝ່ໄດ້ເມື່ອລະຫັດປ່ຽນແປງແຕ່ລາຍການຊຸດບໍ່ໄດ້
COPY Directory.Build.props Directory.Packages.props global.json .editorconfig ./
COPY src/Directory.Build.props src/
COPY src/DormFlow.Domain/DormFlow.Domain.csproj src/DormFlow.Domain/
COPY src/DormFlow.Contracts/DormFlow.Contracts.csproj src/DormFlow.Contracts/
COPY src/DormFlow.Application/DormFlow.Application.csproj src/DormFlow.Application/
COPY src/DormFlow.Infrastructure/DormFlow.Infrastructure.csproj src/DormFlow.Infrastructure/
COPY src/DormFlow.Api/DormFlow.Api.csproj src/DormFlow.Api/

RUN dotnet restore src/DormFlow.Api/DormFlow.Api.csproj

# ---------- ຂັ້ນຕອນ publish ----------
FROM restore AS publish
COPY src/ src/
RUN dotnet publish src/DormFlow.Api/DormFlow.Api.csproj \
    --no-restore \
    --configuration Release \
    --output /app/publish

FROM node:24-alpine AS web-build
WORKDIR /web
COPY web/package.json web/package-lock.json ./
RUN npm ci
COPY web/ ./
RUN npm run build

# ---------- ຮູບພາບການແລ່ນຕົວຈິງ ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    Https__EnableRedirection=false

# curl ຈໍາເປັນສໍາລັບ HEALTHCHECK ຂອງ docker compose ເນື່ອງຈາກວ່າຮູບພາບ aspnet ບໍ່ໄດ້ຕິດຕັ້ງ
RUN apt-get update \
    && apt-get install --no-install-recommends -y curl \
    && rm -rf /var/lib/apt/lists/*

# ໂຟນເດີໄຟລ໌ Log ຈະຕ້ອງຂຽນໄດ້ໂດຍຜູ້ທີ່ບໍ່ແມ່ນຜູ້ໃຊ້ root
RUN mkdir -p /app/logs && chown -R $APP_UID:$APP_UID /app

COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .
COPY --from=web-build --chown=$APP_UID:$APP_UID /web/dist ./wwwroot

# ດໍາເນີນການກັບຜູ້ໃຊ້ທີ່ບໍ່ມີສິດພິເສດ root ອີງຕາມຄ່າມາດຕະຖານຂອງຮູບພາບ .NET
USER $APP_UID

EXPOSE 8080

ENTRYPOINT ["dotnet", "DormFlow.Api.dll"]
