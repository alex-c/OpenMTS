# EnvironmentReader

This IoT application connects to either or both temperature and humidity sensors, periodically reads values and writes the resulting data to Kafka.

## Configuration

The Kafka endpoint, Kafka topic and read interval can be set in the `appsettings.json` configuration file:

```json
{
  "Kafka": "<host>:<port>",
  "Topic": "openmts-environment-<storage_site.id>",
  "ReadIntervalInSeconds": 20,
  "Temperature": {
    "Provider": "MockTemperatureProvider",
    "Configuration": {}
  },
  "Humidity": {
    "Provider": "MockHumidityProvider",
    "Configuration": {}
  }
}
```

## Providers

The communication with specific IoT sensors is implemented through providers, which implement the `OpenMts.EnvironmentReader.IEnvironmentFactorProvider` interface. The configuration allows to specify providers for one or both of the enviropnmental factors by setting it's name in the `Provider` field, and add provider-specific configuration options in the matching `Configuration` object.

Currently available providers:

| Provider                  | Factor      | Device |
| ------------------------- | ----------- | ------ |
| `MockTemperatureProvider` | temperature | `none` |
| `MockHumidityProvider`    | humidity    | `none` |

### MockTemperatureProvider

Generates mock temperature data for testing purposes.

### MockHumidityProvider

Generates mock humidity data for testing purposes.
