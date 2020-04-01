-- Creates the tables needed for OpenMTS.

CREATE TABLE users (
    id varchar (32) PRIMARY KEY,
    name varchar (255) NOT NULL,
    password varchar (255) NOT NULL,
    salt bytea NOT NULL,
    role smallint NOT NULL,
    disabled boolean NOT NULL DEFAULT false
);