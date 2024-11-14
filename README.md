# HumanResourceManager

Simple .NET (8.0) project connected to postgreSQL using EntityFramework;

## Development server

To run you will need to configure the PostgreSQL and create this table : 

`CREATE TABLE tb_user ( Id SERIAL PRIMARY KEY, "name" VARCHAR(255) NOT NULL, "birthdate" TIMESTAMPTZ NOT NULL DEFAULT NOW(), "email" VARCHAR(255) NOT NULL, "password" VARCHAR(255) NOT NULL, CreatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW());`

## Running unit tests
Run `cd HumanResourceManager.Tests` and then `dotnet test`
