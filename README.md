# OpenMTS

A material tracking system (MTS) for the plastics industry. Built for the [Institute of Plastics Processing in Industry and the Skilled Crafts](https://www.ikv-aachen.de/) (IKV) during the author's Master's thesis at [FH Aachen University of Applied Sciences](https://www.fh-aachen.de/). OpenMTS allows to track the materials inventory accross multiple locations and material types and allows to record the environmental temperature and humidity of storage locations through IoT integrations.

## Project Structure

The [mts-backend](./mts-backend) directory contains the .NET Core backend service of OpenMTS.

The [mts-frontend](./mts-frontend) directory contains the Vue.js frontend of OpenMTS.

The [mts-iot-apps](./mts-iot-apps) directory contains IoT apps that use the OpenMTS backend API and have been built to simplify processes at the IKV.

The [docs](./docs) directory contains the API documentation of the OpenMTS backend.

## Installation & Configuration

### Database

OpenMTS requries a [PostgreSQL](https://www.postgresql.org/) server with version 11.4 or newer. Make sure to check the requirements of TimescaleDB on the [installation page](https://docs.timescale.com/latest/getting-started/installation/) to make sure your PostgreSQL version is compatible. Download and install the matching TimescaleDB version. Please note that OpenMTS can use either the same PostgreSQL database for calssic tables and TmescaleDB hypertables, or two different databases.

#### Setup: Database and User

To set-up your PostrgeSQL database for OpenMTS, follow these steps:

- Login to the `postgres` database with `psql` using an administrator user (eg. `postgres`)
- Change the password in `setup_db.sql`
- Execute `setup_db.sql` script: `\i path/to/setup_db.sql`

You know have an `openmts` user, owner of the `openmts` database.

#### Setup: Tables

To create the OpenMTS tables, follow these steps:

- Login to the `openmts` database with `psql` using the `openmts` user
- Execute the `create_tables.sql` script: `\i path/to/create_tables.sql`

You know have all OpenMTS tables.

#### Setup: Hypertable

To setup the `environment` hypertable for environmental data, you need a TimescaleDB installation. It can either be the same PostgreSQL host as the normal tables, or a different host. If you are on a different host, repeat the database setup steps from above.

To create the hypertable, follow these steps:

- Login to the `openmts` database with `psql` using the `openmts` user
- Execute the `create_hypertable.sql` script: `\i path/to/create_tables.sql`

You know have the `environment` hypertable.

### Backend

The OpenMTS server requires the [.NET Core Runtine version 2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1) (or alternatively the .NET Core SDK version 2.1). Move the compiled server to the desired host, configure it through the `appsettings.json` file and run the `OpenMTS.dll` with `dotnet`.

#### Server

The server will expose it's API at `0.0.0.0:5000` by default. You can change this in the following section of the `appsettings.json` file:

```json
{
  "Kestrel": {
    "EndPoints": {
      "Http": {
        "Url": "http://0.0.0.0:5000"
      }
    }
  }
}
```

#### Database Configuration

You can configure the database(s) to use in the following section of the `appsettings.json` file:

```json
{
  "Database": {
    "ConnectionString": "Host=localhost;Database=openmts;Username=openmts;Password="
  },
  "TimescaleDB": {
    "ConnectionString": "Host=localhost;Database=openmts;Username=openmts;Password="
  }
}
```

If you are using only one database, just add the same connection string for both entries.

#### Kafka

You can configure the Kafka instance to read environmental data from in the `appsettings.json` file:

```json
{
  "Kafka": "localhost:9092"
}
```

#### Frontend

For information on the local development of the frontend and building for production, please refer to [/mts-frontend](./mts-frontend/README.md). The frontend should preferably be hosted by a different web server, but can be hosted by the OpenMTS server itself, if needed (see below).

When building the frontend, set the backend API endpoint in the `.env` file, if it differs from `localhost:5000`:

```
VUE_APP_SERVER_ENDPOINT=http://localhost:5000
```

Set the frontend's URL in the server's `appsettings.json` configuration file to enable CORS:

```json
{
  "ExternalFrontendUrl": "http://localhost:8080"
}
```

If the frontend isn't hosted on its host's web server root, set the public path in the `vue.config.js` file when building. If for example, the frontend is hosted at `http://192.168.0.55/openmts-frontend`, set the public path to `./openmts-frontend`:

```js
module.exports = {
  publicPath:
    process.env.NODE_ENV === "production" ? "./openmts-frontend" : "/",
};
```

##### Self-Hosting

If you want the OpenMTS server to host the frontend, add the built website to the `wwwroot` directory in the server's main directory (next to `OpenMTS.dll`), and leave the `ExternalFrontendUrl` option in the `appsettings.json` configuration file empty:

```json
{
  "ExternalFrontendUrl": ""
}
```
