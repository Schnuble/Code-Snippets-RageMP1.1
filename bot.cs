using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition_V_Discord_Bot
{
    public class AdminCommands : ModuleBase<SocketCommandContext>
    {
        //Hier wird eure Client Pipe deklariert. 
        //An der Stelle, wo der . drin ist, muss die IP-Adresse des Ziel Servers hinein. Der Punkt steht für local (Beide System auf gleichem Rechner/Server)
        //Als zweites vergebt ihr eurer Pipe einen Namen
        //Der dritte Punkt gibt die Pipe Richtung an. In diesem Fall Out, da der Bot etwas an den Server senden soll.
        public static NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", "ServerRead_ClientWrite", PipeDirection.Out);
        //Pipes arbeiten in ihrem eigenen Thread. Vergesst nicht, diese Funktion aufzurufen, wenn euer Bot Ready ist, damit die Pipe ordnungsgemäß aufgebaut wird.
        public static void Start()
        {
            Thread clientWriteThread = new Thread(ClientThread_Write);
            clientWriteThread.Start();
        }

        private static void ClientThread_Write()
        {
            namedPipeClientStream = new NamedPipeClientStream(".", "ServerRead_ClientWrite", PipeDirection.Out);
            namedPipeClientStream.Connect();
        }
        //Hier ist ein einfacher test kommant, der eine Nachricht (1 Wort) annimmt.
        [Command("test")]
        public async Task KickPlayerFromGameServer(string msg)
        {
            StreamString streamString = new StreamString(namedPipeClientStream);
            streamString.WriteString(msg);
        }
    }
    //Diese Klasse ist netterweise von Microsoft selbst, also Hippidi Hoppidi, your Code is now my Property :)
    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            int len = 0;

            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            byte[] inBuffer = new byte[len];
            ioStream.Read(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}
