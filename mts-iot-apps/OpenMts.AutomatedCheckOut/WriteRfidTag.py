import sys
import RPi.GPIO as GPIO
sys.path.append('/home/pi/MFRC522-python')
from mfrc522 import SimpleMFRC522

reader = SimpleMFRC522()

try:
  batchId = input('Insert batch ID: ')
  print("Please hold the RFID tag next to the reader...")
  id, batchId = reader.write(batchId) 
  print("Successfully set the batch ID.")
except KeyboardInterrupt:
  GPIO.cleanup()
except Exception as exception:
  print(exception)
  GPIO.cleanup()
  raise
