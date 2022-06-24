FROM mcr.microsoft.com/dotnet/runtime:6.0

WORKDIR /opt/cassandra-migrator

COPY . ./

RUN useradd -d /opt/cassandra-migrator -M -s /usr/sbin/nologin -U migrator
USER migrator

# override settings
ENV Logging__Console__FormatterName=Simple

ENTRYPOINT ["dotnet", "cassandra-migrator.dll"]
