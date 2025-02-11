# Recipe Book

An online application for storing Recipes.

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Node.js](https://nodejs.org)

## Getting Started

### Startup [PostgreSQL](https://www.postgresql.org/)

Make sure Docker Desktop is running, then execute the following command to start the PostgreSQL server container:

```bash
docker-compose up -d
```
### RecipeBook Web API

#### Restore, Build, and Run

```bash
dotnet restore RecipeBook.sln
dotnet build RecipeBook.sln --no-restore
```

#### Running Tests

Integration tests utilize [Testcontainers](https://dotnet.testcontainers.org/) to create a PostgreSQL database for all integration tests. To run the tests, use the following command:

```bash
dotnet test RecipeBook.sln
```

### Recipe Book Web Client

#### Restore, Build, and Run

```bash
cd src/recipebook.web
npm ci
npm run start
```

#### Running Tests

```bash
npm run test
```

## Contributing

Contributions are welcome to this repository. It's generally a good idea to [log an issue](https://github.com/andhernand/recipebook/issues/new/choose) first to discuss any idea before sending a pull request.

## Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](https://contributor-covenant.org) to clarify expected behavior in our community. For more information, see the [Code of Conduct](CODE_OF_CONDUCT.md).

## License

Copyright &copy; 2025 Andres Hernandez. Recipe Book is licensed under the [MIT License](LICENSE).
