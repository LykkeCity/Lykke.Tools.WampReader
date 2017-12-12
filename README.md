# Lykke.Tools.WampReader
Tool to read a Wamp topic for the testing purpose. Using this tool, you can specify a Wamp host uri, realm and topic to read from. Received data will be displayed in the console and optionally can be saved to the file.

## Download

You cand download the latest binaries by this [link](https://github.com/LykkeCity/Lykke.Tools.WampReader/releases/download/v1.0.0/Lykke.Tools.WampReader-v1.0.0.zip).
You can found all versions in the [Releases](https://github.com/LykkeCity/Lykke.Tools.WampReader/releases). 

## Run

To run this tool, you should have [.NetCore runtime](https://www.microsoft.com/net/download/windows) installed on your machine.

To run the tool you need to type ```dotnet Lykke.Tools.WampReader.dll <options>``` in the console.

Awailable options:

```
-a: Append output file. Optional, default is false
-o <file>: Output file path. Optional, default is empty
-r <realm>: Realm name. Required
-t <topic>: Topic name. Required
-u <uri>: Wamp host URI. Required
```

Run example:

```
dotnet Lykke.Tools.WampReader.dll -u wss://wamp-dev.lykkex.net/ws -r prices -t candle.mt.btcusd.bid.sec -o log.txt -a
```
