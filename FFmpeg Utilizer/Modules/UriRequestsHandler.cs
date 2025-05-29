using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FFmpeg_Utilizer.Modules
{
    public class UriRequestsHandler
    {
        private Main main;
        private int port;
        private TcpListener tcpListener;
        private Thread tcpListenerThread;
        private bool isServerStarted = false;

        // Enhanced media type validation
        private readonly HashSet<string> supportedVideoFormats = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "mp4", "webm", "avi", "mov", "wmv", "flv", "mkv", "ogv", "3gp", "m4v"
        };

        private readonly HashSet<string> supportedAudioFormats = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "mp3", "wav", "aac", "m4a", "flac", "ogg", "wma"
        };

        private readonly HashSet<string> supportedStreamFormats = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "m3u8", "mpd", "ts"
        };

        private readonly HashSet<string> supportedMethods = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "GET", "HEAD", "OPTIONS"
        };

        public UriRequestsHandler(Main _main, int _port)
        {
            main = _main;
            port = _port;
        }

        public bool StartServer()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                tcpListenerThread = new Thread(new ThreadStart(ListenForClientRequests))
                {
                    IsBackground = true
                };
                tcpListener.Start();
                tcpListenerThread.Start();
                isServerStarted = true;

                //main.notice.SetNotice($"URI Server started on port {port}", NoticeModule.TypeNotice.Success);
                main.SetURIServerStatus(true);
                return true;
            }
            catch (SocketException ex)
            {
                main.notice.SetNotice($"Failed to start server on port {port}: {ex.Message}", NoticeModule.TypeNotice.Error);
                return false;
            }
            catch (Exception ex)
            {
                main.notice.SetNotice($"Unexpected error starting server: {ex.Message}", NoticeModule.TypeNotice.Error);
                return false;
            }
        }

        private void ListenForClientRequests()
        {
            try
            {
                while (isServerStarted)
                {
                    using (TcpClient client = tcpListener.AcceptTcpClient())
                    {
                        ProcessClientRequest(client);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // Server has been stopped, this is expected
            }
            catch (SocketException ex)
            {
                if (isServerStarted) // Only log if server should be running
                {
                    main.notice.SetNotice($"Socket error in listener: {ex.Message}", NoticeModule.TypeNotice.Error);
                }
            }
            catch (Exception ex)
            {
                main.notice.SetNotice($"Error in client listener: {ex.Message}", NoticeModule.TypeNotice.Error);
            }
        }

        private void ProcessClientRequest(TcpClient client)
        {
            try
            {
                byte[] bytes = new byte[1024];
                NetworkStream stream = client.GetStream();

                int length = stream.Read(bytes, 0, bytes.Length);
                if (length == 0) return;

                string request = Encoding.UTF8.GetString(bytes, 0, length);
                var httpRequest = ParseHttpRequest(request);

                if (httpRequest == null)
                {
                    SendErrorResponse(stream, "400 Bad Request", "Invalid HTTP request");
                    return;
                }

                // Validate HTTP method against supported methods
                if (!supportedMethods.Contains(httpRequest.Method))
                {
                    SendErrorResponse(stream, "405 Method Not Allowed", $"Method \'{httpRequest.Method}\' not supported. Supported methods: {string.Join(", ", supportedMethods)}");
                    return;
                }

                // Handle different HTTP methods
                switch (httpRequest.Method.ToUpper())
                {
                    case "OPTIONS":
                        HandleOptionsRequest(stream, httpRequest);
                        break;
                    case "HEAD":
                        HandleHeadRequest(stream, httpRequest);
                        break;
                    case "GET":
                        HandleGetRequest(stream, httpRequest);
                        break;
                    default:
                        // This should never happen due to the validation above, but keeping for safety
                        SendErrorResponse(stream, "405 Method Not Allowed", "Method not supported");
                        break;
                }
            }
            catch (Exception ex)
            {
                main.notice.SetNotice($"Error processing client request: {ex.Message}", NoticeModule.TypeNotice.Error);
            }
        }

        private HttpRequestInfo ParseHttpRequest(string request)
        {
            try
            {
                string[] lines = request.Split(new[] { "\r\n\r\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length == 0) return null;

                string[] firstLine = lines[0].Split(' ');
                if (firstLine.Length < 3) return null;

                var httpRequest = new HttpRequestInfo
                {
                    Method = firstLine[0],
                    RawUrl = firstLine[1],
                    HttpVersion = firstLine[2]
                };

                // Parse URL and query parameters
                if (httpRequest.RawUrl.Contains('?'))
                {
                    string[] urlParts = httpRequest.RawUrl.Split('?');
                    httpRequest.Path = urlParts[0];
                    httpRequest.QueryString = urlParts[1];
                    httpRequest.QueryParameters = ParseQueryParameters(httpRequest.QueryString);
                }
                else
                {
                    httpRequest.Path = httpRequest.RawUrl;
                    httpRequest.QueryParameters = new Dictionary<string, string>();
                }

                // Parse headers
                httpRequest.Headers = new Dictionary<string, string>();
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) break;

                    int colonIndex = lines[i].IndexOf(':');
                    if (colonIndex > 0)
                    {
                        string headerName = lines[i].Substring(0, colonIndex).Trim();
                        string headerValue = lines[i].Substring(colonIndex + 1).Trim();
                        httpRequest.Headers[headerName] = headerValue;
                    }
                }

                return httpRequest;
            }
            catch
            {
                return null;
            }
        }

        private Dictionary<string, string> ParseQueryParameters(string queryString)
        {
            var parameters = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(queryString)) return parameters;

            try
            {
                string[] pairs = queryString.Split('&');
                foreach (string pair in pairs)
                {
                    string[] keyValue = pair.Split('=');
                    if (keyValue.Length == 2)
                    {
                        string key = Uri.UnescapeDataString(keyValue[0]);
                        string value = Uri.UnescapeDataString(keyValue[1]);
                        parameters[key] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                main.notice.SetNotice($"Error parsing query parameters: {ex.Message}", NoticeModule.TypeNotice.Warning);
            }

            return parameters;
        }

        private void HandleOptionsRequest(NetworkStream stream, HttpRequestInfo request)
        {
            // Handle CORS preflight request
            string response = BuildHttpResponse("200 OK", "", new Dictionary<string, string>
            {
                ["Access-Control-Allow-Origin"] = "*",
                ["Access-Control-Allow-Methods"] = "GET, HEAD, OPTIONS",
                ["Access-Control-Allow-Headers"] = "Content-Type, Authorization",
                ["Access-Control-Max-Age"] = "86400"
            });

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);
        }

        private void HandleHeadRequest(NetworkStream stream, HttpRequestInfo request)
        {
            // HEAD request should return the same headers as GET but no body
            if (IsValidDownloadRequest(request))
            {
                string response = BuildHttpResponse("202 Accepted", "", GetCorsHeaders());
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }
            else
            {
                SendErrorResponse(stream, "400 Bad Request", "");
            }
        }

        private void HandleGetRequest(NetworkStream stream, HttpRequestInfo request)
        {
            if (!IsValidDownloadRequest(request))
            {
                SendErrorResponse(stream, "400 Bad Request", "Invalid request parameters");
                return;
            }

            try
            {
                string filename = request.QueryParameters["addName"];
                string url = request.QueryParameters["addURL"];

                // Additional validation
                if (!IsValidUrl(url))
                {
                    SendErrorResponse(stream, "400 Bad Request", "Invalid URL format");
                    return;
                }

                if (!IsValidFilename(filename))
                {
                    SendErrorResponse(stream, "400 Bad Request", "Invalid filename");
                    return;
                }

                if (!IsValidMediaFile(url, filename))
                {
                    SendErrorResponse(stream, "400 Bad Request", "Unsupported media format");
                    return;
                }

                // Process the request (add to M3U8 list)
                AddToM3U8List(filename, url);

                // Send success response
                string response = BuildHttpResponse("202 Accepted", "Request processed successfully", GetCorsHeaders());
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);

                //main.notice.SetNotice($"Added: {filename} -> {url}", NoticeModule.TypeNotice.Success);
            }
            catch (Exception ex)
            {
                main.notice.SetNotice($"Error handling GET request: {ex.Message}", NoticeModule.TypeNotice.Error);
                SendErrorResponse(stream, "500 Internal Server Error", "Server error occurred");
            }
        }

        private bool IsValidDownloadRequest(HttpRequestInfo request)
        {
            return request.QueryParameters.ContainsKey("addName") &&
                   request.QueryParameters.ContainsKey("addURL") &&
                   !string.IsNullOrWhiteSpace(request.QueryParameters["addName"]) &&
                   !string.IsNullOrWhiteSpace(request.QueryParameters["addURL"]);
        }

        private bool IsValidUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url)) return false;

                // Decode the URL if it\'s encoded
                string decodedUrl = Uri.UnescapeDataString(url);

                // Check if it\'s a valid URI
                Uri uri = new Uri(decodedUrl);

                // Must be HTTP or HTTPS
                return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidFilename(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename)) return false;

            try
            {
                // Check for invalid filename characters
                char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
                return !filename.Any(c => invalidChars.Contains(c));
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidMediaFile(string url, string filename)
        {
            try
            {
                // Extract extension from filename first
                string extension = System.IO.Path.GetExtension(filename)?.TrimStart('.') ?? "";

                // If no extension in filename, try to extract from URL
                if (string.IsNullOrEmpty(extension))
                {
                    Uri uri = new Uri(Uri.UnescapeDataString(url));
                    extension = System.IO.Path.GetExtension(uri.AbsolutePath)?.TrimStart('.') ?? "";
                }

                // Check URL content for stream formats
                string lowerUrl = url.ToLower();
                if (lowerUrl.Contains("m3u8") || lowerUrl.Contains(".mpd") || lowerUrl.Contains(".ts"))
                {
                    return true;
                }

                // Check if extension is supported
                return !string.IsNullOrEmpty(extension) &&
                       (supportedVideoFormats.Contains(extension) ||
                        supportedAudioFormats.Contains(extension) ||
                        supportedStreamFormats.Contains(extension));
            }
            catch
            {
                return false;
            }
        }

        private void AddToM3U8List(string filename, string url)
        {
            try
            {
                // Invoke on UI thread to safely update the ListView
                main.Invoke((Action)(() =>
                {
                    var item = new ListViewItem(filename)
                    {
                        SubItems = { Uri.UnescapeDataString(url), "• Waiting" }
                    };
                    main.M3U8_listView.Items.Add(item);

                    Core.ChangeTab(Core.Tabs.M3U8);
                    main.BringToFront();
                    Core.BringWindowToFront(main);
                }));
            }
            catch (Exception ex)
            {
                main.notice.SetNotice($"Error adding to M3U8 list: {ex.Message}", NoticeModule.TypeNotice.Error);
            }
        }

        private Dictionary<string, string> GetCorsHeaders()
        {
            return new Dictionary<string, string>
            {
                ["Access-Control-Allow-Origin"] = "*",
                ["Access-Control-Allow-Methods"] = "GET, HEAD, OPTIONS",
                ["Access-Control-Allow-Headers"] = "Content-Type, Authorization"
            };
        }

        private string BuildHttpResponse(string statusCode, string body, Dictionary<string, string> additionalHeaders = null)
        {
            var headers = new StringBuilder();
            headers.AppendLine($"HTTP/1.1 {statusCode}");
            headers.AppendLine($"Server: {Core.softwareName} {Core.GetVersion()}");
            headers.AppendLine("Content-Type: text/plain; charset=utf-8");
            headers.AppendLine($"Content-Length: {Encoding.UTF8.GetByteCount(body)}");
            headers.AppendLine("Accept-Ranges: bytes");

            // Add CORS headers
            var corsHeaders = GetCorsHeaders();
            foreach (var header in corsHeaders)
            {
                headers.AppendLine($"{header.Key}: {header.Value}");
            }

            // Add additional headers
            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    if (!corsHeaders.ContainsKey(header.Key))
                    {
                        headers.AppendLine($"{header.Key}: {header.Value}");
                    }
                }
            }

            headers.AppendLine(); // Empty line before body
            headers.Append(body);

            return headers.ToString();
        }

        private void SendErrorResponse(NetworkStream stream, string statusCode, string message)
        {
            try
            {
                string response = BuildHttpResponse(statusCode, message);
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }
            catch (Exception ex)
            {
                main.notice.SetNotice($"Error sending error response: {ex.Message}", NoticeModule.TypeNotice.Error);
            }
        }

        public void StopServer()
        {
            try
            {
                isServerStarted = false;
                tcpListener?.Stop();
                tcpListenerThread?.Join(1000); // Wait up to 1 second for thread to finish
                //main.notice.SetNotice("URI Server stopped", NoticeModule.TypeNotice.Info);
            }
            catch (Exception ex)
            {
                main.notice.SetNotice($"Error stopping server: {ex.Message}", NoticeModule.TypeNotice.Error);
            }
            finally
            {
                main.SetURIServerStatus(false);
            }
        }

        public void KillServer()
        {
            try
            {
                isServerStarted = false;
                tcpListener?.Stop();

                // More aggressive shutdown - abort thread if it doesn\'t finish quickly
                if (tcpListenerThread != null)
                {
                    if (!tcpListenerThread.Join(500)) // Wait only 500ms
                    {
                        tcpListenerThread.Abort(); // Force terminate if needed
                    }
                }

                //main.notice.SetNotice("URI Server killed", NoticeModule.TypeNotice.Info);
            }
            catch (Exception ex)
            {
                main.notice.SetNotice($"Error killing server: {ex.Message}", NoticeModule.TypeNotice.Error);
            }
            finally
            {
                main.SetURIServerStatus(false);
            }
        }

        // Helper class to hold parsed HTTP request information
        private class HttpRequestInfo
        {
            public string Method { get; set; }
            public string RawUrl { get; set; }
            public string Path { get; set; }
            public string QueryString { get; set; }
            public string HttpVersion { get; set; }
            public Dictionary<string, string> QueryParameters { get; set; }
            public Dictionary<string, string> Headers { get; set; }
        }
    }
}