# Network AutoSwitch [![Build Status](https://ci.appveyor.com/api/projects/status/github/Tulpep/network-autoswitch)](https://ci.appveyor.com/project/tulpep/network-autoswitch)

Network AutoSwitch will automatically disable or enable wired and wireless network adapters according to network connection. Depending on the priority selected (Wired or Wireless), when you disconnect your computer from the wireless network it will enable wired adapters automatically or it will enable wireless network adapters when wired network is disabled.  

Proxy AutoSwitch will automatically disable or enable IE proxy server according to network connection. When you disconnect your computer from the wireless network or the wired network adapters are disabled, it will enable or disable IE proxy server depending on the chosen configuration.

## [Download now](https://github.com/Tulpep/Network-AutoSwitch/releases/latest)

Network AutoSwitch and Proxy AutoSwitch can run in interactive mode for your testing purposes and also can run as Windows Service. In Windows Service mode it will work in background without user interaction. You can distribute it using Active Directory or your current deployment software solution to use it as part of your networking hardening strategy.

# How to use it
-p or --priority flag are mandatory to choose the right configuration, passing "Wired" or "Wireless" parameters. Administrative rights are neccesary to enable or disable network interfaces. Run using a command prompt with administative rights as:
```bash
NetworkAutoSwitch.exe -p Wired
```
or

```bash
NetworkAutoSwitch.exe -p Wireless
```
When using as a Windows Service it will work for all users in the machine, even it they dont have Windows administrative rights. It will start automatically with your System. You must choose also the priority when
To install it run it as:
```bash
NetworkAutoSwitch.exe --install -p Wired
```

To uninstall it run as:
```bash
NetworkAutoSwitch.exe --uninstall
```

Proxy AutoSwitch can be used in the same way as NetworkAutoSwitch. The priority flag is also mandatory.

```bash
ProxyAutoSwitch.exe -p Wired
```
or

```bash
ProxyAutoSwitch.exe -p Wireless
```

and 

To install it run it as:
```bash
ProxyAutoSwitch.exe --install -p Wireless
```

To uninstall it run as:
```bash
ProxyAutoSwitch.exe --uninstall
```

**This project is supported. Your issues will be handle by our team and we are open to your PRs!**



