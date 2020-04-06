-- Creates the tables needed for OpenMTS.

DROP TABLE storage_areas;
DROP TABLE storage_sites;
DROP TABLE plastics;
DROP TABLE users;
DROP TABLE api_keys_rights;
DROP TABLE api_keys;
DROP TABLE configuration;

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