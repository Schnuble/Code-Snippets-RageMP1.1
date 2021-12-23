using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Linq;
using System.Text;

//Ebenfalls wieder einen Thread starten
public static void StartPipeline()
{
    Thread serverReadThread = new Thread(ServerThread_Read);
    serverReadThread.Start();
}

private static void ServerThread_Read()
{
    //Hier den gleichen Pipenamen und Direction auf In setzen
    NamedPipeServerStream namedPipeServerStream = new NamedPipeServerStream("ServerRead_ClientWrite", PipeDirection.In);
    //Bissl Stuff, damit die Kiste nicht abraucht :)
    while (true)
    {
        if (!namedPipeServerStream.IsConnected)
        {
            try
            {
                namedPipeServerStream.WaitForConnection();
                Console.WriteLine("Connection Established");
            } catch (IOException) { namedPipeServerStream.Disconnect(); }
        }
        else
        {
            //Hier den gesendeten String abfangen
            StreamString streamString = new StreamString(namedPipeServerStream);
            try
            {
                string receiver = streamString.ReadString();
                Console.WriteLine(receiver);
            }
            catch (Exception) { }
        }
    }            
}
//Microsoft kurz beklauen
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
