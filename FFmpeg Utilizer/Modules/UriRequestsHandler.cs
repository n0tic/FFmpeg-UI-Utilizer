#pragma warning disable IDE0059 // Nagging about requestString

using FFmpeg_Utilizer.Data;
using FFmpeg_Utilizer.Forms;
using FFmpeg_Utilizer.Modules.Submodule;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Web;

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
                    {
                        URIRequestExtender.SendResponse(sHttpVersion, ref connection, false);
                        connection.Close();
                    }

                    //At present we will only deal with GET type
                    if (buffer.Substring(0, 3) == "GET" && fullRequest[4] == '/' && fullRequest[5] == '?' && fullRequest.Contains("="))
                    {
                        // Split the URL by the ampersand separator to get the two query parameters
                        string[] queryParameters = fullRequest.Split('&');

                        // Get the value of the "addName" parameter
                        string addName = HttpUtility.UrlDecode(queryParameters[0].Substring(queryParameters[0].IndexOf('=') + 1));
                        addName = addName.Replace(".m3u8", "");
                        addName = addName.Replace("%20", " ");
                        addName = RemoveIllegalPathCharacters(addName);

                        // Get the value of the "addURL" parameter
                        string addURL = HttpUtility.UrlDecode(queryParameters[1].Substring(queryParameters[1].IndexOf('=') + 1));

                        // For testing purposes
                        if (addName == "TEST" && addURL == "TEST")
                        {
                            URIRequestExtender.SendResponse(sHttpVersion, ref connection, true);
                            connection.Close();
                        }

                        if (URIRequestExtender.IsValidUrl(addURL) && URIRequestExtender.IsValidFile(addURL) && !main.m3u8Processor.inProcess)
                        {
                            main.Invoke(new Action(() => {
                                ListViewItem item = new ListViewItem(new[] { MakeUniqueName(addName), addURL, "• Waiting" });
                                main.M3U8_listView.Items.Add(item);
                                Core.ChangeTab(Core.Tabs.M3U8);
                                //main.BringToFront();
                                Core.BringWindowToFront(main);
                            }));
                        }

                        URIRequestExtender.SendResponse(sHttpVersion, ref connection, true);
                    }
                    else URIRequestExtender.SendResponse(sHttpVersion, ref connection, false);

                    connection.Close();
                }
            }
        }

        private string MakeUniqueName(string name)
        {
            int i = 0;
            string newName = name;



            // Check if the name is already in the list view
            while (main.M3U8_listView.Items.Cast<ListViewItem>().Any(item => item.SubItems[0].Text == newName))
            {
                i++;
                newName = $"{name} {i}";
            }

            return newName;
        }

        public static string RemoveIllegalPathCharacters(string path)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).Distinct().ToArray();
            return new string(path.Where(c => !invalidChars.Contains(c)).ToArray());
        }

        public void KillServer()
        {
            tcpServer?.Stop();
            UriListener?.Abort();
            main.SetURIServerStatus(false);
        }
    }
}