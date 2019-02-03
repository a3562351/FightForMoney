color 0A && echo off

start "RouteServer1" Server.exe 1
ping -n 2 127.0.0.1>nul

start "RouteServer2" Server.exe 2
ping -n 2 127.0.0.1>nul

start "LoginServer" Server.exe 3
ping -n 2 127.0.0.1>nul

start "CommonServer" Server.exe 4
ping -n 2 127.0.0.1>nul

start "MainServer" Server.exe 5
ping -n 2 127.0.0.1>nul

start "InstanceServer" Server.exe 6
ping -n 2 127.0.0.1>nul

start "DataServer" Server.exe 7
ping -n 2 127.0.0.1>nul

