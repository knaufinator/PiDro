# PiDro

Raspberry pi 3 Hydronics monitor and controller

Mono app, using square panels to represent monitor/control items,

Still early in dev,  porting this from a javafx app that I am abandoning due to lack of support on arm from JavaFx

Upnp port forwarding, for web service, ssh, vnc 

WAMPv2 sharp web service to feed android monitor app for real time feed on the go.

Dymanic adding of modules such as 
-PH monitor - analog voltage from a/d (PCF8591) to PH probe circuit
-PH control - relay controls to increment ph up /down
-Temperature probe (DS18B20)
-Pump timer - control relay output, turn item on/off 

Email alerts to inform user of max values, ph, temp

![ScreenShot](/screenshots/main.png)
