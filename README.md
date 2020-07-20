# OpenMTS

A material tracking system (MTS) for the plastics industry. Built for the [Institute of Plastics Processing in Industry and the Skilled Crafts](https://www.ikv-aachen.de/) (IKV) during the author's Master's thesis at [FH Aachen University of Applied Sciences](https://www.fh-aachen.de/). OpenMTS allows to track the materials inventory accross multiple locations and material types and allows to record the environmental temperature and humidity of storage locations through IoT integrations.

**Table of Contents**

- [Features & Tech Stack](#features-&-tech-stack)
- [Project Structure](#project-structure)
- [Installation & Configuration](#installation-&-configuration)
  - [Database](#database)
  - [Backend](#backend)
  - [Frontend](#frontend)
- [Docker](#docker)
- [IoT Integrations](#iot-integrations)

## Features & Tech Stack

OpenMTS comes with a .NET Core backend server and a [Vue.js](https://vuejs.org/) UI, and uses [PostgreSQL](https://www.postgresql.org/) for data persistence. All functionality is available both throught he UI and through a HTTP API.

OpenMTS allows to manage users, API keys for IoT integrations, plastics and materials, storage locations, and finally the materials inventory. Through an IoT app that connects to sensors, OpenMTS tracks the temperature and humidity of storage locations and provides a history diagram as well as extremum values over any given time frame. This environment time-series data is transmitted through [Apache Kafa](https://kafka.apache.org/) and stored in [TimescaleDB](https://docs.timescale.com/latest/main).

|                 Inventory                  |              Environmental Data               |
| :----------------------------------------: | :-------------------------------------------: |
| ![Inventory](./docs/openmts-inventory.PNG) | ![Environmental Data](./docs/openmts-env.PNG) |

## Project Structure

This project has the following structure:

- [docker](./docker): contains [docker-compose](https://docs.docker.com/compose/) files
- [mts-backend](./mts-backend): contains the .NET Core backend service
  - [mts-backend/sql](./mts-backend/sql): contains SQL scripts to set up the PostgreSQL and TimescaleDB database(s)
  - [mts-backend/docs](./mts-backend/docs): containes an export of an [Insomnia](https://insomnia.rest/) workspace, which serves as an API documentation and allows for easy API testing
- [mts-frontend](./mts-frontend): contains the Vue.js frontend
- [mts-iot-apps](./mts-iot-apps): contains IoT apps that use the OpenMTS backend API and have been built to simplify processes at the IKV

## Installation & Configuration

### Database

OpenMTS requries a PostgreSQL server with version 11.4 or newer. Make sure to check the requirements of TimescaleDB on the [installation page](https://docs.timescale.com/latest/getting-started/installation/) to make sure your PostgreSQL version is compatible. Download and install the matching TimescaleDB version. Please note that OpenMTS can use either the same PostgreSQL database for calssic tables and TmescaleDB hypertables, or two different databases.

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

##### User Account

On the first time running the server, you might want to have it create an administrator user account. To do so, fill out the following section in the `appsettings.json` file:

```json
{
  "EnsureAdmin": {
    "Id": "",
    "Name": "",
    "Password": ""
  }
}
```

This section can later be emptied. Please make sure to change your password after logging in.

##### JWT Signing

Set a secret for signing JWTs in the following section of the `appsettings.json` file. Please note that the secret needs to be at least 16 characters long. You can also configure the JWT life time (in minutes).

```json
{
  "Jwt": {
    "Secret": "",
    "Issuer": "OpenMTS",
    "LifetimeInMinutes": 120
  }
}
```

##### Files

OpenMTS allows users to upload files. You can configure the location to persist these files in the following block of the `appsettings.json` file:

```json
{
  "Files": {
    "Path": "C:\\Users\\Alex\\Desktop\\OpenMtsFiles"
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

### Frontend

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

If you want the OpenMTS server to host the frontend, add the built website to the `wwwroot` directory in the server's content root (next to `OpenMTS.dll`), and leave the `ExternalFrontendUrl` option in the `appsettings.json` configuration file empty:

```json
{
  "ExternalFrontendUrl": ""
}
```

## Docker

Several dockerfiles and docker-compose setups are available for easy deployment.

### Building the Backend

To build the OpenMTS backend image, navigate to `./mts-backend/src/OpenMTS` and execute `docker build . -t openmts-backend`. Please note that an image will have the `latest` tag by default, if no tag was provided. A specific version can also be set: `docker build . -t openmts-backend:0.1.0`. To add a tag to an existing image: `docker tag openmts-backend:0.1.0 openmts-backend:latest`.

### Building the Frontend

To build the OpenMTS frontend image, navigate to `./mts-frontend` and execute `docker build . -t openmts-frontend`. Please note that the server endpoint first needs to be set in the `.env` file (see [Frontend Configuration](#frontend)). The frontend image contains [nginx](https://nginx.org/) in order to serve the UI.

### Self-Hosting

It is recommended to separate the OpenMTS backend and frontend (eg. by using the two separate images mentioned above), but the backend server can be used to serve the frontend itself. To do that, execute `docker build . -t openmts` in the main directory of the repository. In docker compose files, replace the `openmts-backend` images with `openmts` and remove the `openmts-frontend` containers.

### Docker Compose

Two different docker compose setups are available in the `./docker` directory. `./docker/openmts-complete` contains a docker compose file which will run all prerequisites as well as OpenMTS itself. This will run Zookeeper, Kafka, TimescaleDB, the OpenMTS backend and the OpenMTS frontend. The `./docker/openmts-infra` directory contains a docker compose file that will run only the prerequisites for OpenMTS: Zookeeper, Kafka and TimescaleDB. The `./docker/openmts-app` directory contains a docker compose file that will run only OpenMTS itself.

#### openmts-complete

Please note that when running the `openmts-complete` app the first time, the database will not be set up yet and the `openmts-backend` container will therefore terminate. This is expected. See the section [Database Setup](#database-setup) below. Once the database has been set up, the `openmts-backend` container can be started again with `docker start`.

### Database setup

To set up the database in the context of Docker, execute `psql` in the `openmts-db` container, and import the `./sql/setup.sql` script:

```bash
docker exec -it <container_id> psql -U openmts

# In psql:
\i ./sql/setup.sql
```

#### Logging

To activate various logging options in PostgreSQL, add the following line to the `openmts-db` service in the docker compose file:

```bash
command: ["postgres", "-c", "log_connections=on", "-c", "log_statement=all", "-c", "log_disconnections=on", "-c", "log_lock_waits=on", "-c", "log_duration=off", "-c", "log_min_duration_statement=-1", "-c", "log_error_verbosity=VERBOSE", "-c", "log_destination=stderr"]
```

Alternatively, mount a customized `postgresql.conf` by adding the following section to the `openmts-db` service's volumes section in the docker compose file:

```bash
- ./config/postgresql.conf:/var/lib/postgresql/data
```

## IoT Integrations

OpenMTS is designed to allow easy IoT integrations.

### API Keys

Administrators can create and configure API keys through the UI. Using these keys, applications can authenticate through the backend API in order to obtain JWTs with access rights. Atomic rights can be granted, which allow access to specific functionalities. The `users.create` access right for example allows the use of the `[POST] api/users` endpoint to create new OpenMTS users. An API key needs to be enabled before it can be used.

![API Keys Administration](./docs/openmts-apikeys.PNG)

### Authentication Flow

Using the supplied API key, an application first needs to authenticate. To do so, it needs to send a HTTP POST request to `http://{server}:{port}/api/auth?method=apiKey` with the API key in the request JSON body:

```json
{
  "apiKey": "3fdebf97-e76a-425d-a027-f5b3d5db827a"
}
```

A successful request will contain a JSON web token in the body:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzZmRlYmY5Ny1lNzZhLTQyNWQtYTAyNy1mNWIzZDVkYjgyN2EiLCJyaWdodHMiOiJwbGFzdGljcy5jcmVhdGUiLCJleHAiOjE1ODUxNDE1MzksImlzcyI6Ik9wZW5NVFMifQ.rQU5hLtwxM2Vd5yIW8C21nZ9-_n_VMYOBOZ6qdld51g"
}
```

Subsequent API calls will need to carry this token in the `Authorization` header with the `Bearer` prefix.

### Enviornment Reader

The [mts-iot-apps/OpenMts.EnvironmentReader](./mts-iot-apps/OpenMts.EnvironmentReader/README.md) directory contains a .NET Core app to read environmental data. An instance can read both the temperature and humidity or either one of those.

### Automated Check-Out

The [mts-iot-apps/OpenMts.AutomatedCheckOut](./mts-iot-apps/OpenMts.AutomatedCheckOut/README.md) directory contains a Python app to perform check-out transactions from a Raspberry Pi with a `KERN EOC30K-4` scale and an RFID reader.
