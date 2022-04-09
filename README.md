# RTS FRAMEWORK
 This Unity5 project is a Real-time Strategy game framework which supports multi-player mode.
 
 Contributors: Zhengrui Xia, Haochen Li
 
## Prerequisite
 Unity 2020.3.12f1c1 + Skynet
 
## Setup Server
 Install and setup Skynet from here: https://github.com/cloudwu/skynet
 
 Move MyServer folder to the Skynet directory.
 
 To start the server, run:
 ```
 cd skynet
 ./skynet MyServer/config
 ```
 
## Run
 Make sure the unity version is correct (2020.3.12f1c1). If you want to use other versions, you will have to change SLua either.
 
 Open with Unity editor, start with the MainUIScene.
 
 For single player mode, just click Start when you are ready.
 
 For multi-player, start the server first. Then set the correct ip address and click connect. When your name showed on the left, you can click start to enter the game.
