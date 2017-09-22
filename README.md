# Quizzy
This is a demo app I made to practice async/await methods and also socket programing. 

It's a trivia game consisting of a client and a server which are described below. 

## Server
The server hosts the games and sends the questions to each connected client.
It has the following commands which can be run from the it's shell.

* Start - Starts the server
* Stop - stops the server
* Quit - closes the app
* Clients - displays a list of clients currently connected
* Game - starts a new game
* Help - displays this list of commands

## Client
The client handles the io for an individual user. It displays the questions
sent by the server and tracks it's individual user's score. 
It has the following commands which can be run through it's shell.

* Connect - conencts to the server
* Disconnect - disconnects from the server
* Reset - resets the score to 0
* Score - displays the score
* Quit - closes the app
* Help - displays this list of commands
* a - answer a for a question
* b - answer b for a question
* c - answer c for a question
* d - answer d for a question