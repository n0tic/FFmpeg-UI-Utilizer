#pragma warning disable IDE0059 // Nagging about requestString

using FFmpeg_Utilizer.Data;
using FFmpeg_Utilizer.Modules.Submodule;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FFmpeg_Utilizer.Modules
{
    public class UriRequestsHandler
    {
        public Main main;

        public TcpListener tcpServer;
        private static Thread UriListener;

        public UriRequestsHandler(Main main, int port = 288)
        {
            this.main = main;

            // Start Server
            try
            {
                tcpServer = new TcpListener(new IPEndPoint(IPAddress.Loopback, port));
                tcpServer.Start();

                UriListener = new Thread(new ThreadStart(Server));
                UriListener.Start();
            }
            catch (OutOfMemoryException) { }
            catch (ThreadStateException) { }
            catch (ArgumentNullException) { }
            catch (ArgumentOutOfRangeException) { }
            catch (FormatException) { }
            catch { }
        }

        public void Server()
        {
            main.Invoke(new Action(() =>
            {
                main.SetURIServerStatus(true);
            }));

            while (true)
            {
                //Accept new connection
                Socket connection = null;
                try
                {
                    connection = tcpServer.AcceptSocket();
                }
                catch { }

                if (connection.Connected)
                {
                    //Receive data from the client
                    Byte[] bReceive = new Byte[connection.ReceiveBufferSize];
                    int i = connection.Receive(bReceive, bReceive.Length, SocketFlags.None);

                    //Convert Byte to String
                    string buffer = Encoding.ASCII.GetString(bReceive);

                    //Create headers
                    Headers headers = new Headers();
                    headers.AddHeaders(buffer.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

                    // Look for HTTP request
                    int iStartPos = headers.headers[0].IndexOf("HTTP", 1); // i = 7

                    if (iStartPos == -1) // If there was no HTTP request found DENY connection
                        connection.Close();

                    //Data Holders
                    string sHttpVersion = "";
                    string fullRequest = "";
                    string requestString = "";

                    // Get the HTTP text and version e.g. it will return "HTTP/1.1"
                    try
                    {
                        sHttpVersion = headers.headers[0].Substring(iStartPos, 8); // Get string from index 7 and 8 (15) chars forward.
                        fullRequest = headers.headers[0].Substring(0, iStartPos - 1); // "GET /"+query (Query QuestionMark at index 6 and forward.)}
                        requestString = fullRequest.Substring(6);
                    }
                    catch { }

                    if (fullRequest == "GET /favicon.ico") // We don't want to send data if we don't need to.
                        connection.Close();

                    //At present we will only deal with GET type
                    if (buffer.Substring(0, 3) == "GET" && fullRequest[4] == '/' && fullRequest[5] == '?' && fullRequest.Contains("="))
                    {
                        string[] tmpRequests = fullRequest.Split(new char[] { '=' }, 2);
                        if (tmpRequests[0] == "GET /?add")
                        {
                            main.Invoke(new Action(() =>
                            {
                                if (URIRequestExtender.IsValidUrl(tmpRequests[1]) && URIRequestExtender.IsM3u8(tmpRequests[1]) && !main.m3u8Processor.inProcess)
                                {
                                    try
                                    {
                                        //Is the GET request http? "%2F" == "/"
                                        if (tmpRequests[1].Contains("%2F"))
                                            tmpRequests[1] = URIRequestExtender.FixURL(tmpRequests[1]);

                                        Uri hlsFile = new Uri(tmpRequests[1]);

                                        ListViewItem item = new ListViewItem(new[] { Path.GetFileNameWithoutExtension(hlsFile.AbsoluteUri), tmpRequests[1], "• Waiting" });
                                        main.M3U8_listView.Items.Add(item);
                                        Core.ChangeTab(Core.Tabs.M3U8);

                                        URIRequestExtender.SendResponse(sHttpVersion, ref connection, true);
                                    }
                                    catch (Exception)
                                    {
                                        URIRequestExtender.SendResponse(sHttpVersion, ref connection, false);
                                    }
                                }
                                else URIRequestExtender.SendResponse(sHttpVersion, ref connection, false);
                            }));
                        }
                        else URIRequestExtender.SendResponse(sHttpVersion, ref connection, false);
                    }
                    else URIRequestExtender.SendResponse(sHttpVersion, ref connection, false);

                    connection.Close();
                }
            }
        }

        public void KillServer()
        {
            tcpServer?.Stop();
            UriListener?.Abort();
            main.SetURIServerStatus(false);
        }
    }
}