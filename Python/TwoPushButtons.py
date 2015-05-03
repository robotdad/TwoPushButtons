from time import sleep
import RPi.GPIO as GPIO
import ptvsd

GPIO.setmode(GPIO.BCM)

GPIO.setup(5, GPIO.OUT)
GPIO.setup(6, GPIO.OUT)
GPIO.setup(27, GPIO.IN)
GPIO.setup(22, GPIO.IN)

try:
	while True:

	    # led button 1 is pressed
		if ( GPIO.input(27) == False ):
			GPIO.output(5, GPIO.LOW)
		else
			GPIO.output(5, GPIO.HIGH)
		# led button 2 is pressed
		if ( GPIO.input(22) == False ):
			GPIO.output(6, GPIO.LOW)
		else
			GPIO.output(6, GPIO.HIGH)

except KeyboardInterrupt:  
	GPIO.cleanup()       # clean up GPIO on CTRL+C exit  

GPIO.cleanup()      

