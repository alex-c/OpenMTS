import sys
import time
import configparser
import json
import requests
import serial
import RPi.GPIO as GPIO
sys.path.append('/home/pi/MFRC522-python')
from mfrc522 import SimpleMFRC522

# Connects to the scale using the serial port
def connect_to_scale(port):
  ser = serial.Serial(port, timeout=1)
  return ser

# Get the quantity from a scale response
def convert_value(scale_response):
  low = int(scale_response.find(' ')+1)
  high = int(scale_response.find('k'))
  quantity = float(scale_response[low:high])
  return quantity

# Authenticate the IoT app with the OpenMTS server
def authenticate(server, apiKey):
  api_url = '{0}/api/auth?method=ApiKey'.format(server)
  headers = {'Content-Type': 'application/json'}
  response = requests.post(api_url, headers=headers, json={"apiKey": apiKey})
  if response.status_code == 200:
    return json.loads(response.content.decode('utf-8'))
  else:
    return None

# Perform a transaction
def performTransaction(server, token, userId, batchId, weight):
  api_url = '{0}/api/inventory/{1}/log'.format(server, batchId)
  headers = {'Content-Type': 'application/json', 'Authorization': 'Bearer {0}'.format(token)}
  response = requests.post(api_url, headers=headers, json={"isCheckout": True, "quantity": weight, "userId": userId})
  if response.status_code == 200:
    return json.loads(response.content.decode('utf-8'))
  else:
    return None

# Get config
config = configparser.ConfigParser()
config.read('./config.ini')
print(config.sections())
server = config["Server"]["Endpoint"]
apiKey = config["Server"]["ApiKey"]
scalePort = config["Scale"]["UsbPort"]

# Output config
print("Configuration:")
print(" + Server: " + server)
print(" + API-Key: " + apiKey)
print(" + Scale USB port: " + scalePort)

# Start reading
reader = SimpleMFRC522()
try:
  while True:
    # Get user ID
    # TODO: get user ID from an RFID tag or similar
    userId = "alex"

    # Get batch ID
    print("Please hold the batch-card next to the reader...")
    id, batchId = reader.read()
    print(" - Batch ID:" + batchId)

    # Get weight
    ser = connect_to_scale(scalePort)
    raw_decode = bytes('S')
    ser.write(raw_decode)
    scale_response = ser.readline()
    quantity = convert_value(str(scale_response))
    print(" - Quantity: " + str(quantity) + "kg")

    # Authenticate
    response = authenticate(server, apiKey)
    if response is not None:
      token = response["token"]

      # Perform transaction
      response = performTransaction(server, token, userId, batchId, quantity)
      if response is not None:
        print("Transaction successfully performed!")
      else:
        print("[Error] Transaction failed.")
    else:
      print("[Error] Authentication failed.")
    time.sleep(1)
except KeyboardInterrupt:
  GPIO.cleanup()
except Exception as exception:
  print(exception)
  GPIO.cleanup()
  raise
