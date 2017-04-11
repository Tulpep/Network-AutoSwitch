# Network-AutoSwitch  [![Build Status](https://ci.appveyor.com/api/projects/status/github/Tulpep/Network-AutoSwitch)]

Network AutoSwitch will automatically disable wired network adapters when a wireless connection is detected. Whe you disconnect from the wireless network it will enable wired adapters automatically.

## [Download now](https://github.com/Tulpep/Network-AutoSwitch/releases/latest)

Network AutoSwitch can run interative for your testing purposes but also can run as Windows Service. In Windows Service mode will work in background with not intervation to the final user. You can distribute it using Active Directory or your current deployment solution software.

# How to use it
Run using a command prompt with administative rights (require for enable or disable network connections dynamically) as:
```bash
NetworkAutoSwitch.exe
```

When used as Windows Service it will work for all users in the machine, even it they dont have Windows administrative rights.
To install it run as
```bash
NetworkAutoSwitch.exe --install
```

To uninstall it run as:
```bash
NetworkAutoSwitch.exe --uninstall
```

This project is supported. Your issues will be handle for the team and we are open to your PRs!



