FROM mcr.microsoft.com/dotnet/runtime:5.0

WORKDIR /opt/cassandra-migrator

COPY . ./

RUN useradd -d /opt/cassandra-migrator -M -s /usr/sbin/nologin -U migrator
USER migrator

ENTRYPOINT ["dotnet", "cassandra-migrator.dll"]
