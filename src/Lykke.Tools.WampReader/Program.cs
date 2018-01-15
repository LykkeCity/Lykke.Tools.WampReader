using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Fclp;
using WampSharp.V2;
using WampSharp.V2.Client;

namespace Lykke.Tools.WampReader
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var appArguments = TryGetAppArguments(args);

            if (appArguments == null)
            {
                return;
            }

            var outputWriter = TryGetOutputWriter(appArguments);
            var channel = await ConnectAsync(appArguments);
            var realmProxy = channel.RealmProxy;

            Console.WriteLine($"Connected to the server {appArguments.Uri}");
            Console.WriteLine("Press any key to disconnect");

            var subscription = Subscribe(realmProxy, appArguments, outputWriter);

            Console.ReadKey(true);

            subscription.Dispose();
            channel.Close();

            if (outputWriter != null)
            {
                await outputWriter.FlushAsync();
                outputWriter.Close();
                outputWriter.Dispose();
            }
        }

        private static IDisposable Subscribe(IWampRealmProxy realmProxy, AppArguments appArguments, StreamWriter outputWriter)
        {
            var subscription = realmProxy.Services
                .GetSubject<dynamic>(appArguments.Topic)
                .Subscribe(message =>
                {
                    Console.WriteLine(message);

                    outputWriter?.WriteLine(message);
                });

            return subscription;
        }

        private static async Task<IWampChannel> ConnectAsync(AppArguments appArguments)
        {
            var factory = new DefaultWampChannelFactory();
            var channel = string.IsNullOrEmpty(appArguments.AuthMethod) && string.IsNullOrEmpty(appArguments.AuthId)
                ? factory.CreateJsonChannel(GetUri(appArguments), appArguments.Realm)
                : factory.CreateJsonChannel(GetUri(appArguments), appArguments.Realm, new ClientAuthenticator(appArguments.AuthMethod, appArguments.AuthId));

            while (!channel.RealmProxy.Monitor.IsConnected)
            {
                try
                {
                    Console.WriteLine($"Trying to connect to the server {appArguments.Uri}...");

                    channel.Open().Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to connect: {e.Message}");
                    Console.WriteLine("Retrying in 5 sec...");

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }

            return channel;
        }

        private static string GetUri(AppArguments appArguments)
        {
            var uri = appArguments.Uri.ToString();

            if (uri.EndsWith('/'))
            {
                return uri;
            }

            return $"{uri}/";
        }

        private static StreamWriter TryGetOutputWriter(AppArguments appArguments)
        {
            if (string.IsNullOrWhiteSpace(appArguments.OutputFilePath))
            {
                return null;
            }

            var fileStream = File.Open(
                appArguments.OutputFilePath,
                appArguments.AppendOutput ? FileMode.Append : FileMode.Create,
                FileAccess.Write,
                FileShare.Read);

            return new StreamWriter(fileStream, Encoding.UTF8, bufferSize: 16, leaveOpen: false);
        }

        private static AppArguments TryGetAppArguments(string[] args)
        {
            var parser = new FluentCommandLineParser<AppArguments>();

            parser.SetupHelp("?", "help")
                .Callback(text => Console.WriteLine(text));

            parser.Setup(x => x.Uri)
                .As('u')
                .Required()
                .WithDescription("-u <uri>. Wamp host URI. Required");

            parser.Setup(x => x.Realm)
                .As('r')
                .Required()
                .WithDescription("-r <realm>. Realm name. Required");

            parser.Setup(x => x.Topic)
                .As('t')
                .Required()
                .WithDescription("-t <topic>. Topic name. Required");

            parser.Setup(x => x.OutputFilePath)
                .As('o')
                .SetDefault(null)
                .WithDescription("-o <file>. Output file path. Optional, default is empty");

            parser.Setup(x => x.AuthMethod)
                .As('m')
                .SetDefault(null)
                .WithDescription("-m <auth method>. Auth method name. Optional, default is empty");
            
            parser.Setup(x => x.AuthId)
                .As('i')
                .SetDefault(null)
                .WithDescription("-i <authentication id>. Authentication id. Optional, default is empty");

            parser.Setup(x => x.AppendOutput)
                .As('a')
                .SetDefault(false)
                .WithDescription("-a. Append output file. Optional, default is false");
            
            var parsingResult = parser.Parse(args);

            if (!parsingResult.HasErrors)
            {
                return parser.Object;
            }

            Console.WriteLine("Lykke Wamp Reader (c) 2017");
            Console.WriteLine("Usage:");

            parser.HelpOption.ShowHelp(parser.Options);

            return null;
        }
    }
}
