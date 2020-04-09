# mts-backend

## Database Setup

### Database

To set-up your PostrgeSQL database for OpenMTS, follow these steps:

- Login to the `postgres` database with `psql` using an administrator user (eg. `postgres`)
- Change the password in `setup_db.sql`
- Execute `setup_db.sql` script: `\i path/to/setup_db.sql`

You know have an `openmts` user, owner of the `openmts` database.

### Tables

To create the OpenMTS tables, follow these steps:

- Login to the `openmts` database with `psql` using the `openmts` user
- Execute the `create_tables.sql` script: `\i path/to/create_tables.sql`

You know have all OpenMTS tables.

### Hypertable

To setup the `environment` hypertable for environmental data, you need a TimescaleDB installation. It can either be the same PostgreSQL host as the normal tables, or a different host. If you are on a different host, repeat the database setup steps from above.

To create the hypertable, follow these steps:

- Login to the `openmts` database with `psql` using the `openmts` user
- Execute the `create_hypertable.sql` script: `\i path/to/create_tables.sql`

You know have the `environment` hypertable.
