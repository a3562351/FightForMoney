color 0A && echo off

rem server_id server_type config_idx
rem 路由服
start "RouteServer" FightForMoney-Server.exe 101 1 "RouteServer101"

ping -n 1 127.0.0.1>nul

rem 登陆服
start "LoginServer" FightForMoney-Server.exe 201 2 "LoginServer201"

rem 聊天服
start "ChatServer" FightForMoney-Server.exe 301 3 "ChatServer301"

rem 场景服
start "MainServer" FightForMoney-Server.exe 401 4 "MainServer401"

rem 副本服
start "InstanceServer" FightForMoney-Server.exe 501 5 "InstanceServer501"

rem 公共服
start "CommonServer" FightForMoney-Server.exe 601 6 "CommonServer601"