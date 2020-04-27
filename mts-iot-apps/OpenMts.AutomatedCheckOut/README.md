# AutomaticCheckIn

This IoT application is written in Python and is designed to run on a Raspberry Pi. It connects to an RFID reader and a KERN EOC30K-4 scale. This was tested on a Raspberry Pi 4 Model B, but should work on previous models too.

## Flow

The app wait for an RFID signal. Once an RFID tag with a material batch ID has been scanned, the app reads the material quantity from the scale and performs a check-out transaction in OpenMTS. The `WriteRfidTag.py` script can be used to write batch IDs to RFID tags.

## Installation

### Devices

Connect the Raspberry Pi to the RFID reader using GPIO. Connect the scale to the Raspberry Pi using one of the available USB ports. Set the used USB port in the `config.ini` file (eg. `/dev/ttyUSB0`).

### Python Packages

Install on Raspberry Pi:

- [MFRC522-python](https://github.com/pimylifeup/MFRC522-python)

Install on Raspberry Pi with `pip3`:

- `pyserial`

## Configuration

Set the server endpoint and API key in the following section of the `config.ini` file:

```ini
[Server]
Endpoint = http://169.254.154.27:5000
ApiKey = 5c792aa7-c36a-4393-b317-501cee8c8a5b
```

Please note that the API key needs to be created in the OpenMTS system. It requires the `inventory.perform_transaction` access right and has to be enabled.

## Run

Run the app with `python ./AutomatedCheckOut.py`. Cancel at any point with `Ctrl+C`.
