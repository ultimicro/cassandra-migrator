# Cassandra Migrator
[![Docker Pulls](https://img.shields.io/docker/pulls/ultimicro/cassandra-migrator)](https://hub.docker.com/r/ultimicro/cassandra-migrator)

This is a tool for managing schema on Apache Cassandra. It is available as a .NET Tool, Docker image and a class library.

## Features

- Using raw CQL as a migration script so you have full control how to migrate your Cassandra.
- Supports multiple CQL statements per migration. Each statement will execute one by one.
- CQL syntax checking before execute the first statement so you don't end up invalid syntax after some statements already executed.

## Library usage

Install [CassandraMigrator](https://www.nuget.org/packages/CassandraMigrator/) then create an instance of `CassandraMigrator.Migrator` then invoke
`ExecuteAsync` method.

### Avaialble IConnection implementations

- `CassandraMigrator.DataStaxClient.Connection` from [CassandraMigrator.DataStaxClient](https://www.nuget.org/packages/CassandraMigrator.DataStaxClient).

## License

MIT
