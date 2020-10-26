using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace FFmpeg_Utilizer.Modules.Submodule
{
    public static class URIRequestExtender
    {
        public static string FixURL(string url)
        {
            List<string> DecodedText = new List<string>() {
            {"\""},
            {"#"},
            {"$"},
            {"%"},
            {"&"},
            {"'"},
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
                if (url.Contains(EncodedText[i])) url = url.Replace(EncodedText[i], DecodedText[i]);

            return url;
        }

        public static bool IsM3u8(string url)
        {
            return !string.IsNullOrEmpty(url) && url.ToLower().Contains("m3u8");
        }

        public static bool IsValidUrl(string url)
        {
            return !string.IsNullOrEmpty(url) && (url.StartsWith("www") || url.StartsWith("http"));
        }

        public static void SendResponse(string sHttpVersion, ref Socket connection, bool success)
        {
            if (success)
                SendHeader(sHttpVersion, 0, " 202 Accepted", ref connection);
            else
                SendHeader(sHttpVersion, 0, " 400 Bad Request", ref connection);

            SendToBrowser("", ref connection);
            connection.Close();
        }

        private static void SendHeader(string HttpVersion, int iTotBytes, string StatusCode, ref Socket mySocket, string MIMEHeader = "text/html")
        {
            string sBuffer = "";
            sBuffer += HttpVersion + StatusCode + Environment.NewLine;
            sBuffer += "Server: " + Core.softwareName + " " + Core.GetVersion() + Environment.NewLine;
            sBuffer += "Content-Type: " + MIMEHeader + Environment.NewLine;
            sBuffer += "Accept-Ranges: bytes" + Environment.NewLine;
            sBuffer += "Content-Length: " + iTotBytes + Environment.NewLine + Environment.NewLine;
            byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer);
            SendToBrowser(bSendData, ref mySocket);
        }

        private static void SendToBrowser(String sData, ref Socket mySocket) => SendToBrowser(Encoding.ASCII.GetBytes(sData), ref mySocket);

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