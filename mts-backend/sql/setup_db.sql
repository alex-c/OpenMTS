-- This sets up the 'openmts' database and user. Change the password before executing.

DROP DATABASE IF EXISTS openmts;
DROP ROLE IF EXISTS openmts;
CREATE ROLE openmts LOGIN PASSWORD 'password'; --change 'password' to desired password
CREATE DATABASE openmts OWNER openmts;