# Cassandra Migrator
[![Docker Pulls](https://img.shields.io/docker/pulls/ultimicro/cassandra-migrator)](https://hub.docker.com/r/ultimicro/cassandra-migrator)

This is a tool for managing schema on Apache Cassandra. It is available as a .NET Tool, Docker image and a class library. This tool has been used on [Cloudsumé](https://cloudsume.com) from the beginning so it is production ready.

| Package                          | Version                                                                                                                                      |
| -------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| .NET Tool                        | [![Nuget](https://img.shields.io/nuget/v/cassandra-migrator)](https://www.nuget.org/packages/cassandra-migrator)                             |
| CassandraMigrator                | [![Nuget](https://img.shields.io/nuget/v/CassandraMigrator)](https://www.nuget.org/packages/CassandraMigrator)                               |
| CassandraMigrator.CqlParser      | [![Nuget](https://img.shields.io/nuget/v/CassandraMigrator.CqlParser)](https://www.nuget.org/packages/CassandraMigrator.CqlParser)           |
| CassandraMigrator.DataStaxClient | [![Nuget](https://img.shields.io/nuget/v/CassandraMigrator.DataStaxClient)](https://www.nuget.org/packages/CassandraMigrator.DataStaxClient) |
| CassandraMigrator.Provider       | [![Nuget](https://img.shields.io/nuget/v/CassandraMigrator.Provider)](https://www.nuget.org/packages/CassandraMigrator.Provider)             |

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
