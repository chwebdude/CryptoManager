# CryptoManager
A Cryptocurrency Manager

| Master                                                                                                                                                                        	| Latest                                                                                                                                            	|
|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	|---------------------------------------------------------------------------------------------------------------------------------------------------	|
| [![Build status](https://ci.appveyor.com/api/projects/status/j63d7v4wdi4yi0o9/branch/master?svg=true)](https://ci.appveyor.com/project/chwebdude/cryptomanager/branch/master) 	![Build status](https://webdude.visualstudio.com/_apis/public/build/definitions/4959fc14-70a5-4e9b-b324-aa9246ec93a1/16/badge)| [![Build status](https://ci.appveyor.com/api/projects/status/j63d7v4wdi4yi0o9?svg=true)](https://ci.appveyor.com/project/chwebdude/cryptomanager) 	|


This is a simple tool to import your crypto transactions and trades and show your profit and much more. It runns on your own computer. So you don't have to provide your credentials to any third party. Everything stays on your disk!
The tool itself is based on .Net Core. So it's plattform independent :blush:

# Table of Content
- [CryptoManager](#cryptomanager)
- [Table of Content](#table-of-content)
- [Usage](#usage)
    - [First Start](#first-start)
- [Development](#development)
    - [Dependencies](#dependencies)

# Usage
Download your Windows or Mac OS X Package from the release section.
Just run the Script ```RunWindows.bat``` or ```RunOSX```. It will firstly start the webserver, then wait a few seconds and then start your webbrowser on ```http://localhost:5000```.
## First Start
First you mast define your Importers. Click on **Exchanges**. Then you can add your API informations. it will automaticly try to import trades and fundings. On **Transactions** is a list of all trades, deposits and withdraws. It's not very usefull, but sometime handy. The **Fundings** section provides you with information about how much you have on your wallet/exchange. And last there is a **Invest** section which shows how much your invest is worth now (when you never sold it). More to come! 


# Development
I would be happy to see some help from other developers. Just create a fork and push it back to this repository. Otherwise create Issue with your Bug/Idea.
## Dependencies
- .net Core SDK
- NodeJS + Yarn Package Manager

To start your development execute ```yarn``` in the ```CryptoManager``` directory.