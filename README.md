# Network AutoSwitch [![Build Status](https://ci.appveyor.com/api/projects/status/github/Tulpep/network-autoswitch)](https://ci.appveyor.com/project/tulpep/network-autoswitch)

Network AutoSwitch will automatically disable wired network adapters when a wireless connection is detected. When you disconnect your computer from the wireless network it will enable wired adapters automatically. 

## [Download now](https://github.com/Tulpep/Network-AutoSwitch/releases/latest)

Network AutoSwitch can run in interactive mode for your testing purposes and also can run as Windows Service. In Windows Service mode it will work in background without user interaction. You can distribute it using Active Directory or your current deployment software solution to use it as part of your networking hardening strategy.

# How to use it
Run using a command prompt with administative rights as:
```bash
NetworkAutoSwitch.exe
```
Administrative rights are neccesary to enable or disable network interfaces.

When using as a Windows Service it will work for all users in the machine, even it they dont have Windows administrative rights. It will start automatically with your System.
To install it run it as:
```bash
NetworkAutoSwitch.exe --install
```

To uninstall it run as:
```bash
NetworkAutoSwitch.exe --uninstall
```


**This project is supported. Your issues will be handle by our team and we are open to your PRs!**



