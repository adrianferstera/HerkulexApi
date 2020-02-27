# HerkulexApi

This C# solution can be used to programmatically control the Smart Robot Servo HerkuleX DRS 0602. 
With some minor changes, it can be easily expanded to other versions of the HerkuleX motor family. 

## Getting Started
#1 Connect the servo according to the manual to the computer. Connect a 14.8V power supply to the system, although they mentioed in the manual, that 7.4V is enough, it is not. The servo will directly go into an error mode (red blinking light). 
#2 Connect the Serial interface to your computer. You do not need to buy the expensive and unhandy interface from Dongbu Robot. I just used the Adafruit CP2104 Friend. If you use this kind of interface, connect the RX line of the servo to the TX plug on the interface and vice versa. 
#3 Enjoy!
### Installing
#1 Download the solution 
#2 Open the solution in Visual Studio 
#3 Build the Solution
#4 Open the Unit Test Project



```
var myInterface = new HerkulexInterface("COM1", 112500);
var myServo = new HerkulexDrs0602(219, myInterface);
myServo.TorqueOn();
myServo.MoveServoPosition(-40, 500);
myServo.MoveServoPosition(0, 500);
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

### And coding style tests

Explain what these tests test and why

```
Give an example
```

## Deployment

Add additional notes about how to deploy this on a live system

## Built With

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning


## Authors

* **Billie Thompson** - *Initial work* - [PurpleBooth](https://github.com/PurpleBooth)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details


