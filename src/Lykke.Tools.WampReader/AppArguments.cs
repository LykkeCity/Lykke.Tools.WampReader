using System;

namespace Lykke.Tools.WampReader
{
    internal class AppArguments
    {
        public Uri Uri { get; set; }
        public string Realm { get; set; }
        public string Topic { get; set; }
        public string OutputFilePath { get; set; }

        public bool AppendOutput { get; set; }
        public string AuthMethod { get; set; }
        public string AuthId { get; set; }
    }
}
