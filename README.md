# HerkulexApi

This C# solution can be used to programmatically control the Smart Robot Servo HerkuleX DRS 0602. 
With some minor changes, it can be easily expanded to other versions of the HerkuleX motor family. 

## Getting Started
* Connect the servo according to the manual to the computer. Connect a 14.8V power supply to the system, although they mentioed in the manual, that 7.4V is enough, it is not. The servo will directly go into an error mode (red blinking light). 
* Connect the Serial interface to your computer. You do not need to buy the expensive and unhandy interface from Dongbu Robot. I just used the Adafruit CP2104 Friend. If you use this kind of interface, connect the RX line of the servo to the TX plug on the interface and vice versa. 
* All the example code below can be found in the Unit Tests.

### Installing
*  Download the solution 
*  Open the solution in Visual Studio 
*  Build the Solution
*  Open the Unit Test Project

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

// Default id of the servos is 219
var myServo = new HerkulexDrs0602(219, myInterface);

// Enable torque otherwise the servo wont move
myServo.TorqueOn();

// move the servo to position -40 deg in 500 ms
myServo.MoveServoPosition(-40, 500);

// Close the interface. 
myInterface.Close();
```

## Step by Step Guide


Initialize an instance of the interface the servo and computer are going to communicate through: 
```
// Default baud rate of the servos is 112500
var myInterface = new HerkulexInterface("COM1", 112500);
```

Initialize the servo with its unique ID and the interface to which the servo is connected to: 

```
// Default id of the servos is 219
var myServo = new HerkulexDrs0602(219, myInterface);
```
Enable the torque, otherwise the servo won't move at all: 
```
myServo.TorqueOn();
```
Move the servo to its desired position between -159 deg and 159 deg. (See P.25 in the manual):
```
myServo.MoveServoPosition(-40, 500);
```
Close the interface. Otherwise another application cannot access it. 
```
// Close the interface. 
myInterface.Close();
```
## Change Baud Rate

In order to change the servos baud rate, run the following example. After you have run it, the servo runs into an error mode (red blinking led). Just unplug the battery and plug it in again and reconnect the interface with the new baud rate. 

```
// Default baud rate of the servos is 112500
var myInterface = new HerkulexInterface("COM1", 112500);

// Default id of the servos is 219
var myServo = new HerkulexDrs0602(219, myInterface);

//Change baud rate.
myServo.ChangeBaudRate(HerkulexBaudRate.RATE57600);

// Close the interface. 
myInterface.Close();
```

## Change Servo ID
To change the servo's id, run the following example. After you have successfully run this code sequence, reconnect the servo with new id. 

```
// Default baud rate of the servos is 112500
var myInterface = new HerkulexInterface("COM1", 112500);

// Default id of the servos is 219
var myServo = new HerkulexDrs0602(219, myInterface);

//Change id (between 1 and 255).
 var success = myServo.ChangeId(7);
 
 if (success) Console.WriteLine("Successfully changed servo Id");
 else Console.WriteLine("Upppps, an error occured"); 
 
 // Close the interface. 
myInterface.Close();
 
```


## Acknowledgement

Thanks to Cesar Vandevelde [https://github.com/cesarvandevelde/HerkulexServo]. At some point the manual of the manufactorer was very unclear, so I used his Arduino Library to understand what the manual is trying to explain. 

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details


