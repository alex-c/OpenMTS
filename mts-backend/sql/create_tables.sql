-- Creates the tables needed for OpenMTS.

DROP TABLE users;
DROP TABLE configuration;

CREATE TABLE configuration (
  allowGuestLogin boolean DEFAULT false NOT NULL
);

INSERT INTO configuration (allowGuestLogin) VALUES (false);

CREATE TABLE users (
    id varchar (32) PRIMARY KEY,
    name varchar (255) NOT NULL,
    password varchar (255) NOT NULL,
    salt bytea NOT NULL,
    role smallint NOT NULL,
    disabled boolean DEFAULT false NOT NULL
);