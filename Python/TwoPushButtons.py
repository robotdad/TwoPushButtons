from time import sleep
import RPi.GPIO as GPIO
import ptvsd
ptvsd.enable_attach(secret=None)

GPIO.setmode(GPIO.BCM)

GPIO.setup(5, GPIO.IN)
GPIO.setup(6, GPIO.IN)
GPIO.setup(27, GPIO.OUT)
GPIO.setup(22, GPIO.OUT)

try:
	while True:

		# led button 1 is pressed
		if ( GPIO.input(5) == False ):
			GPIO.output(27, GPIO.LOW)
		else:
			GPIO.output(27, GPIO.HIGH)
		# led button 2 is pressed
		if ( GPIO.input(6) == False ):
			GPIO.output(22, GPIO.LOW)
		else:
			GPIO.output(22, GPIO.HIGH)

except KeyboardInterrupt:  
	GPIO.cleanup()       # clean up GPIO on CTRL+C exit  

GPIO.cleanup()      

