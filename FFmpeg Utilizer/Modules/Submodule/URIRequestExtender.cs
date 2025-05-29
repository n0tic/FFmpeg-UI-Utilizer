using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace FFmpeg_Utilizer.Modules.Submodule
{
    public static class URIRequestExtender
    {
        /// <summary>
        /// Decodes URL-encoded characters in the given URL string
        /// </summary>
        /// <param name="url">The URL to decode</param>
        /// <returns>Decoded URL string</returns>
        public static string FixURL(string url)
        {
            try
            {
                return Uri.UnescapeDataString(url);
            }
            catch
            {
                // Fallback to manual decoding if Uri.UnescapeDataString fails
                List<string> DecodedText = new List<string>() {
                    {"\""},
                    {"#"},
                    {"$"},
                    {"%"},
                    {"&"},
                    {"\'"},
                    {"+"},
                    {","},
                    {"/"},
                    {":"},
                    {";"},
                    {"<"},
                    {"="},
                    {">"},
                    {"?"},
                    {"@"},
                    {"["},
                    {@"\"},
                    {"]"},
                    {"^"},
                    {"`"},
                    {"{"},
                    {"|"},
                    {"}"},
                    {"~"},
                    {"Ā"},
                    {"ā"},
                    {"Ē"},
                    {"ē"},
                    {"Ī"},
                    {"ī"},
                    {"Ō"},
                    {"ō"},
                    {"Ū"},
                    {"ū"}};
                List<string> EncodedText = new List<string>() {
                    {"%22"},
                    {"%23"},
                    {"%24"},
                    {"%25"},
                    {"%26"},
                    {"%27"},
                    {"%2B"},
                    {"%2C"},
                    {"%2F"},
                    {"%3A"},
                    {"%3B"},
                    {"%3C"},
                    {"%3D"},
                    {"%3E"},
                    {"%3F"},
                    {"%40"},
                    {"%5B"},
                    {"%5C"},
                    {"%5D"},
                    {"%5E"},
                    {"%60"},
                    {"%7B"},
                    {"%7C"},
                    {"%7D"},
                    {"%7E"},
                    {"%C4%80"},
                    {"%C4%81"},
                    {"%C4%92"},
                    {"%C4%93"},
                    {"%C4%AA"},
                    {"%C4%AB"},
                    {"%C5%8C"},
                    {"%C5%8D"},
                    {"%C5%AA"},
                    {"%C5%AB"}};

                for (int i = 0; i < EncodedText.Count; i++)
                    if (url.Contains(EncodedText[i]))
                        url = url.Replace(EncodedText[i], DecodedText[i]);

                return url;
            }
        }

        /// <summary>
        /// Validates if the URL contains supported media file extensions or streaming formats
        /// Note: This is a legacy method. New validation logic is in UriRequestsHandler
        /// </summary>
        /// <param name="url">URL to validate</param>
        /// <returns>True if the file type is supported</returns>
        [Obsolete("Use UriRequestsHandler.IsValidMediaFile instead")]
        public static bool IsValidFile(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            string lowerUrl = url.ToLower();
            return lowerUrl.Contains("m3u8") ||
                   lowerUrl.Contains("mp4") ||
                   lowerUrl.Contains("webm") ||
                   lowerUrl.Contains("avi") ||
                   lowerUrl.Contains("mov") ||
                   lowerUrl.Contains("mp3") ||
                   lowerUrl.Contains("wav") ||
                   lowerUrl.Contains("aac") ||
                   lowerUrl.Contains("flac");
        }

        /// <summary>
        /// Validates if the URL has a valid format
        /// Note: This is a legacy method. New validation logic is in UriRequestsHandler
        /// </summary>
        /// <param name="url">URL to validate</param>
        /// <returns>True if URL format is valid</returns>
        [Obsolete("Use UriRequestsHandler.IsValidUrl instead")]
        public static bool IsValidUrl(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) return false;

                // Basic validation - starts with www or http
                if (url.StartsWith("www") || url.StartsWith("http"))
                {
                    return true;
                }

                // More thorough validation using Uri class
                Uri uri = new Uri(url);
                return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sends HTTP response to the client socket
        /// Note: This method is kept for backward compatibility but should be avoided
        /// </summary>
        /// <param name="sHttpVersion">HTTP version string</param>
        /// <param name="connection">Client socket connection</param>
        /// <param name="success">Whether the request was successful</param>
        [Obsolete("Use UriRequestsHandler response methods instead")]
        public static void SendResponse(string sHttpVersion, ref Socket connection, bool success)
        {
            try
            {
                if (success)
                    SendHeader(sHttpVersion, 0, " 202 Accepted", ref connection);
                else
                    SendHeader(sHttpVersion, 0, " 400 Bad Request", ref connection);

                SendToBrowser("", ref connection);
                connection.Close();
            }
            catch
            {
                // Ignore errors in legacy method
            }
        }

        /// <summary>
        /// Legacy method for sending HTTP headers
        /// </summary>
        [Obsolete("Use UriRequestsHandler response methods instead")]
        private static void SendHeader(string HttpVersion, int iTotBytes, string StatusCode, ref Socket mySocket, string MIMEHeader = "text/html")
        {
            try
            {
                string sBuffer = "";
                sBuffer += HttpVersion + StatusCode + Environment.NewLine;
                sBuffer += "Server: " + Core.softwareName + " " + Core.GetVersion() + Environment.NewLine;
                sBuffer += "Content-Type: " + MIMEHeader + Environment.NewLine;
                sBuffer += "Accept-Ranges: bytes" + Environment.NewLine;
                sBuffer += "Content-Length: " + iTotBytes + Environment.NewLine;

                // CORS headers - moved to proper location before the final newline
                sBuffer += "Access-Control-Allow-Origin: *" + Environment.NewLine;
                sBuffer += "Access-Control-Allow-Methods: GET, HEAD, OPTIONS" + Environment.NewLine;
                sBuffer += "Access-Control-Allow-Headers: Content-Type, Authorization" + Environment.NewLine;
                sBuffer += Environment.NewLine; // Final newline to separate headers from body

                byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);
                SendToBrowser(bSendData, ref mySocket);
            }
            catch
            {
                // Ignore errors in legacy method
            }
        }

        /// <summary>
        /// Legacy method for sending data to browser
        /// </summary>
        [Obsolete("Use UriRequestsHandler response methods instead")]
        private static void SendToBrowser(String sData, ref Socket mySocket) => SendToBrowser(Encoding.ASCII.GetBytes(sData), ref mySocket);

        /// <summary>
        /// Legacy method for sending byte data to browser
        /// </summary>
        [Obsolete("Use UriRequestsHandler response methods instead")]
        private static void SendToBrowser(Byte[] bSendData, ref Socket mySocket)
        {
            try
            {
                if (mySocket.Connected) mySocket.Send(bSendData, bSendData.Length, 0);
            }
            catch { }
        }
    }
}