# Cassandra Migrator

This is a tool for managing schema on Apache Cassandra. It is available as a .NET Tool, Docker image and a class library.

## Library usage

Install [CassandraMigrator](https://www.nuget.org/packages/CassandraMigrator/) then create an instance of `CassandraMigrator.Migrator` then invoke
`ExecuteAsync` method.