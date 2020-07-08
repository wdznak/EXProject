# EXProject
.Net Core Solution with MVC project and Identity Server.
MVC project is an app that connects to crypto exchange websockets and saves data to files. On start, it connects to websockets specified in a JSON file. Websockets are running in a Background Task that can be accessed by singleton class "ConnectionsManager". New connections can be created by a form available under /home/addconnection. Every websocket data is saved to file in a folder following schema "Crypto/[Coin Symbol]/[FileName].txt".

Identity Server at the moment has only basic protection restricting access to the whole page.

Project is in a very early stage.
![Connections](../assets/connections.png?raw=true)
![Add Connection](../assets/addconnection.png?raw=true)
