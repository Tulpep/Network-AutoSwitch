# Network-AutoSwitch  ![Build Status](https://ci.appveyor.com/api/projects/status/github/Tulpep/Network-AutoSwitch)

Network AutoSwitch will automatically disable wired network adapters when a wireless connection is detected. Whe you disconnect your computer from the wireless network it will enable wired adapters automatically. 

## [Download now](https://github.com/Tulpep/Network-AutoSwitch/releases/latest)

Network AutoSwitch run in interactive mode for your testing purposes but also can run as Windows Service. In Windows Service mode it will work in background without user interaction. You can distribute it using Active Directory or your current deployment solution software and use it as part of your networking hardening strategy.

# How to use it
Run using a command prompt with administative rights (require for enable or disable network connections dynamically) as:
```bash
NetworkAutoSwitch.exe
```

When used as Windows Service it will work for all users in the machine, even it they dont have Windows administrative rights. It will start automatically with your System.
To install it run as
```bash
NetworkAutoSwitch.exe --install
```

To uninstall it run as:
```bash
NetworkAutoSwitch.exe --uninstall
```


**This project is supported. Your issues will be handle for the team and we are open to your PRs!**



