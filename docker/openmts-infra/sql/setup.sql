-- Creates the tables needed for OpenMTS.

-- Configuration

CREATE TABLE configuration (
  allow_guest_login boolean DEFAULT false NOT NULL
);

INSERT INTO configuration (allow_guest_login) VALUES (false);

-- API Keys

CREATE TABLE api_keys (
  id uuid PRIMARY KEY,
  name varchar (255) NOT NULL,
  enabled boolean DEFAULT false NOT NULL
);

CREATE TABLE api_keys_rights (
  api_key_id uuid REFERENCES api_keys (id) NOT NULL,
  right_id varchar (32) NOT NULL
);

-- Users

CREATE TABLE users (
  id varchar (32) PRIMARY KEY,
  name varchar (255) NOT NULL,
  password varchar (255) NOT NULL,
  salt bytea NOT NULL,
  role smallint NOT NULL,
  disabled boolean DEFAULT false NOT NULL
);

-- Plastics

CREATE TABLE plastics (
  id varchar (16) PRIMARY KEY,
  name varchar (255) NOT NULL
);

-- Storage Locations

CREATE TABLE storage_sites (
  id uuid PRIMARY KEY,
  name varchar (255) NOT NULL
);

CREATE TABLE storage_areas (
  id uuid PRIMARY KEY,
  name varchar (255) NOT NULL,
  site_id uuid REFERENCES storage_sites (id) NOT NULL
);

-- Materials & Custom Props

CREATE TABLE materials (
  id SERIAL UNIQUE,
  name varchar (255) NOT NULL,
  manufacturer varchar (255) NOT NULL,
  manufacturer_specific_id varchar (255) NOT NULL,
  type varchar (16) REFERENCES plastics (id) NOT NULL
);

CREATE TABLE material_props (
  id uuid PRIMARY KEY,
  name varchar (255) NOT NULL,
  type smallint NOT NULL
);

CREATE TABLE text_material_prop_values (
  material_id integer REFERENCES materials (id) NOT NULL,
  prop_id uuid REFERENCES material_props (id) NOT NULL,
  value text NOT NULL,
  UNIQUE (material_id, prop_id)
);

CREATE TABLE file_material_prop_values (
  material_id integer REFERENCES materials (id) NOT NULL,
  prop_id uuid REFERENCES material_props (id) NOT NULL,
  file_path varchar (1024) NOT NULL,
  UNIQUE (material_id, prop_id)
);

-- Batches & Custom Props

CREATE TABLE batches (
  id uuid PRIMARY KEY,
  material_id integer REFERENCES materials (id) NOT NULL,
  area_id uuid REFERENCES storage_areas (id) NOT NULL,
  batch_number bigint NOT NULL,
  expiration_date timestamptz NOT NULL, 
  quantity double precision NOT NULL,
  is_locked boolean NOT NULL,
  is_archived boolean NOT NULL
);

CREATE TABLE batch_props (
  id uuid PRIMARY KEY,
  name varchar (255) NOT NULL
);

CREATE TABLE batch_prop_values (
  batch_id uuid REFERENCES batches (id) NOT NULL,
  prop_id uuid REFERENCES batch_props (id) NOT NULL,
  value text NOT NULL,
  UNIQUE (batch_id, prop_id)
);

-- Transactions

CREATE TABLE transactions (
  id uuid PRIMARY KEY,
  batch_id uuid REFERENCES batches (id) NOT NULL,
  quantity double precision NOT NULL,
  timestamp timestamptz NOT NULL,
  user_id varchar (32) REFERENCES users (id) NOT NULL
);

-- Creates the environment hypetable.

CREATE TABLE environment (
  timestamp timestamptz NOT NULL,
  site uuid NOT NULL,
  temperature real,
  humidity real
);

SELECT create_hypertable('environment', 'timestamp');

CREATE INDEX environment_by_location ON environment (site, timestamp DESC);