# HerkulexApi

This C# solution can be used to programmatically control the Smart Robot Servo HerkuleX DRS 0602. 
With some minor changes, it can be easily expanded to other versions of the HerkuleX motor family. 

## Getting Started
* 1 Connect the servo according to the manual to the computer. Connect a 14.8V power supply to the system, although they mentioed in the manual, that 7.4V is enough, it is not. The servo will directly go into an error mode (red blinking light). 
* 2 Connect the Serial interface to your computer. You do not need to buy the expensive and unhandy interface from Dongbu Robot. I just used the Adafruit CP2104 Friend. If you use this kind of interface, connect the RX line of the servo to the TX plug on the interface and vice versa. 
* 3 Enjoy!

### Installing
* 1 Download the solution 
* 2 Open the solution in Visual Studio 
* 3 Build the Solution
* 4 Open the Unit Test Project

## Using the software

Following code can be used to run the servos. 
If you select the wrong baud rate, the servo will run into an error mode (blinking red), and you need to unplug the power source and plug it in again.  
Very important: 
* The default baudRate of the servo is 112500
* The default id of the servo is 219

## Examples

```
// Default baud rate of the servos is 112500
var myInterface = new HerkulexInterface("COM1", 112500);
// Default id of the servos is 112500
var myServo = new HerkulexDrs0602(219, myInterface);

// Enable torque otherwise the servo wont move
myServo.TorqueOn();

// move the servo to position -40 deg in 500 ms
myServo.MoveServoPosition(-40, 500);
```



A step by step series of examples that tell you how to get a development env running

Say what the step will be

```
Give the example
```

And repeat

```
until finished
```

End with an example of getting some data out of the system or using it for a little demo

## Running the tests

Explain how to run the automated tests for this system

### Break down into end to end tests

Explain what these tests test and why

```
Give an example
```


## Built With

Thanks to Cesar Vandevelde [https://github.com/cesarvandevelde/HerkulexServo]. At some point the manual of the manufactorer was very unclear, so I used his Arduino Library to understand what the manual is trying to explain. 

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details


