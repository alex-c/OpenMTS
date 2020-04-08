# mts-backend

## SQL Scripts

To set-up your PostrgeSQL database for OpenMTS, follow these steps:

- Login to the `postgres` database with `psql` using an administrator user (eg. `postgres`)
- Change the password in `setup_db.sql`
- Execute `setup_db.sql` script: `\i path/to/setup_db.sql`

You know have an `openmts` user, owner of the `openmts` database. Continue:

- Login to the `openmts` database with `psql` using the `openmts` user
- Execute the `create_tables.sql` script: `\i path/to/create_tables.sql`

You know have all necessary tables and an administrator user to login to OpenMTS!
