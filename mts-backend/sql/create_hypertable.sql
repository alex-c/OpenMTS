-- Creates the environment hypetable.

DROP INDEX IF EXISTS environment_by_location;
DROP TABLE IF EXISTS environment;

CREATE TABLE environment (
  timestamp timestamptz NOT NULL,
  site uuid NOT NULL,
  temperature double precision,
  humidity double precision
);

SELECT create_hypertable('environment', 'timestamp');

CREATE INDEX environment_by_location ON environment (site, timestamp DESC);