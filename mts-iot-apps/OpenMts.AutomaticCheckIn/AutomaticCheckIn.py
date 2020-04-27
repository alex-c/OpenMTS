import time
import serial
import RPi.GPIO as GPIO
import sys
import configparser
sys.path.append('/home/pi/MFRC522-python')
from mfrc522 import SimpleMFRC522

def connect_to_scale(port):
  ser = serial.Serial("/dev/ttyUSB0", timeout=1)
  return ser

def convert_value(scale_response):
  low = int(scale_response.find(' ')+1)
  high = int(scale_response.find('k'))
  quantity = float(scale_response[low:high])
  return quantity

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
print("Pleace hold material-card next to the reader")
try:
  while True:
    id, matnr_card = reader.read()
    ser = connect_to_scale(scalePort)

    # Get weight
    raw_decode = bytes('S','utf-8')
    ser.write(raw_decode)
    scale_response = ser.readline()
    print(scale_response)
    quantity = convert_value(str(scale_response))
    weight_formated = '{0:0.2f}'.format(quantity)
    print(" - Read weight: " + str(quantity) + )
    time.sleep(1)
    print("Pleace hold material-card next to the reader")
except KeyboardInterrupt:
  GPIO.cleanup()
except Exception as exception:
  print(exception)
  GPIO.cleanup()
