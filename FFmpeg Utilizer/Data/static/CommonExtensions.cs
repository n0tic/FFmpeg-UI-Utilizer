using System.Collections.Generic;

/*
 * 01:28 2020-09-21
 * https://fileinfo.com/filetypes/common
 *
 * We don't need all of these but... I made the effort so it will stay =)
 */

namespace FFmpeg_Utilizer
{
    public static class CommonExtensions
    {
        public static List<string> TextFiles = new List<string> { ".doc", ".docx", ".log", ".msg", ".odt", ".pages", ".rtf", ".tex", ".tif", ".tiff", ".txt", ".wpd", ".wps" };
        public static List<string> TextFilesDesc = new List<string> { "Microsoft Word Document", "Microsoft Word Open XML Document", "Log File", "Outlook Mail Message", "OpenDocument Text Document", "Pages Document", "Rich Text Format File", "LaTeX Source Document", "Tagged Image File", "Tagged Image File Format", "Plain Text File", "WordPerfect Document", "Microsoft Works Word Processor Document" };

        public static List<string> DataFiles = new List<string> { ".csv", ".dat", ".ged", ".key", ".keychain", ".pps", ".ppt", ".pptx", ".sdf", ".tar", ".tax2016", ".tax2019", ".vcf", ".xml" };
        public static List<string> DataFilesDesc = new List<string> { "Comma Separated Values File", "Data File", "GEDCOM Genealogy Data File", "Keynote Presentation", "Mac OS X Keychain File", "PowerPoint Slide Show", "PowerPoint Presentation", "PowerPoint Open XML Presentation", "Standard Data File", "Consolidated Unix File Archive", "TurboTax 2016 Tax Return", "TurboTax 2019 Tax Return", "vCard File", "XML File" };

        public static List<string> AudioFiles = new List<string> { ".aif", ".iff", ".m3u", ".m4a", ".mid", ".mp3", ".mpa", ".wav", ".wma" };
        public static List<string> AudioFilesDesc = new List<string> { "Audio Interchange File Format", "Interchange File Format", "Media Playlist File", "MPEG-4 Audio File", "MIDI File", "MP3 Audio File", "MPEG-2 Audio File", "WAVE Audio File", "Windows Media Audio File" };

        public static List<string> VideoFiles = new List<string> { ".3g2", ".3gp", ".asf", ".avi", ".flv", ".m4v", ".mov", ".mp4", ".mpg", ".rm", ".srt", ".swf", ".vob", ".wmv", ".ts" };
        public static List<string> VideoFilesDesc = new List<string> { "3GPP2 Multimedia File", "3GPP Multimedia File", "Advanced Systems Format File", "Audio Video Interleave File", "Flash Video File", "iTunes Video File", "Apple QuickTime Movie", "MPEG-4 Video File", "MPEG Video File", "RealMedia File", "SubRip Subtitle File", "Shockwave Flash Movie", "DVD Video Object File", "Windows Media Video File", "Video Stream File" };

        public static List<string> ImageFiles3D = new List<string> { ".3dm", ".3ds", ".max" };
        public static List<string> ImageFiles3DDesc = new List<string> { "Rhino 3D Model", "3D Studio Scene", "3ds Max Scene File" };

        public static List<string> RasterImageFiles = new List<string> { ".mbp", ".dds", ".gif", ".heic", ".jpg", ".png", ".psd", ".pspimage", ".tga", ".thm", ".yuv" };
        public static List<string> RasterImageFilesDesc = new List<string> { "Bitmap Image File", "DirectDraw Surface", "Graphical Interchange Format File", "High Efficiency Image Format", ".JPEG Image", "Portable Network Graphic", "Adobe Photoshop Document", "PaintShop Pro Image", "Targa Graphic", "Thumbnail Image File", "YUV Encoded Image File" };

        public static List<string> VectorImageFiles = new List<string> { ".ai", ".eps", ".svg" };
        public static List<string> VectorImageFilesDesc = new List<string> { "Adobe Illustrator File", "Encapsulated PostScript File", "Scalable Vector Graphics File" };

        public static List<string> PageLayoutFiles = new List<string> { ".indd", ".pct", ".pdf" };
        public static List<string> PageLayoutFilesDesc = new List<string> { "Adobe InDesign Document", "Picture File", "Portable Document Format File" };

        public static List<string> SpreadsheetFiles = new List<string> { ".xlr", ".xls", ".xlsx" };
        public static List<string> SpreadsheetFilesDesc = new List<string> { "Works Spreadsheet", "Excel Spreadsheet", "Microsoft Excel Open XML Spreadsheet" };

        public static List<string> DatabaseFiles = new List<string> { ".accdb", ".db", ".dbf", ".mdb", ".pdb", ".sql" };
        public static List<string> DatabaseFilesDesc = new List<string> { "Access 2007 Database File", "Database File", "Database File", "Microsoft Access Database", "Program Database", "Structured Query Language Data File" };

        public static List<string> ExecutableFiles = new List<string> { ".apk", ".app", ".bat", ".cgi", ".com", ".exe", ".gadget", ".jar", ".wsf" };
        public static List<string> ExecutableFilesDesc = new List<string> { "Android Package File", "macOS Application", "DOS Batch File", "Common Gateway Interface Script", "DOS Command File", "Windows Executable File", "Windows Gadget", "Java Archive File", "Windows Script File" };

        public static List<string> GameFiles = new List<string> { ".b", ".dem", ".gam", ".nes", ".rom", ".sav" };
        public static List<string> GameFilesDesc = new List<string> { "Grand Theft Auto 3 Saved Game File", "Video Game Demo File", "Saved Game File", "Nintendo (NES) ROM File", "N64 Game ROM File", "Saved Game" };

        public static List<string> CADFiles = new List<string> { ".dwg", ".dxf" };
        public static List<string> CADFilesDesc = new List<string> { "AutoCAD Drawing Database File", "Drawing Exchange Format File" };

        public static List<string> GISFiles = new List<string> { ".gpx", ".kml", ".kmz" };
        public static List<string> GISFilesDesc = new List<string> { "GPS Exchange File", "Keyhole Markup Language File", "Google Earth Placemark File" };

        public static List<string> WebFiles = new List<string> { ".asp", ".aspx", ".cer", ".cfm", ".csr", ".css", ".dcr", ".htm", ".html", ".js", ".jsp", ".php", ".rss", ".xhtml" };
        public static List<string> WebFilesDesc = new List<string> { "Active Server Page", "Active Server Page Extended File", "Internet Security Certificate", "ColdFusion Markup File", "Certificate Signing Request File", "Cascading Style Sheet", "Shockwave Media File", "Hypertext Markup Language File", "Hypertext Markup Language File", "JavaScript File", "Java Server Page", "PHP Source Code File", "Rich Site Summary", "Extensible Hypertext Markup Language File" };

        public static List<string> PluginFiles = new List<string> { ".crx", ".plugin" };
        public static List<string> PluginFilesDesc = new List<string> { "Chrome Extension", "Mac OS X Plugin" };

        public static List<string> FontFiles = new List<string> { ".fnt", ".fon", ".otf", ".ttf" };
        public static List<string> FontFilesDesc = new List<string> { "Windows Font File", "Generic Font File", "OpenType Font", "TrueType Font" };

        public static List<string> SystemFiles = new List<string> { ".cab", ".cpl", ".cur", ".deskthemepack", ".dll", ".dmp", ".drv", ".icns", ".ico", ".lnk", ".sys" };
        public static List<string> SystemFilesDesc = new List<string> { "Windows Cabinet File", "Windows Control Panel Item", "Windows Cursor", "Windows 8 Desktop Theme Pack File", "Dynamic Link Library", "Windows Memory Dump", "Device Driver", "macOS Icon Resource File", "Icon File", "Windows Shortcut", "Windows System File" };

        public static List<string> SettingsFiles = new List<string> { ".cfg", ".ini", ".prf" };
        public static List<string> SettingsFilesDesc = new List<string> { "Configuration File", "Windows Initialization File", "Outlook Profile File" };

        public static List<string> EncodedFiles = new List<string> { ".hqx", ".mim", ".uue" };
        public static List<string> EncodedFilesDesc = new List<string> { "BinHex 4.0 Encoded File", "Multi-Purpose Internet Mail Message File", "Uuencoded File" };

        public static List<string> CompressedFiles = new List<string> { ".7z", ".cbr", ".deb", ".gz", ".pkg", ".rar", ".rpm", ".sitx", ".tar.gz", ".zip", ".zipx" };
        public static List<string> CompressedFilesDesc = new List<string> { "7-Zip Compressed File", "Comic Book RAR Archive", "Debian Software Package", "Gnu Zipped Archive", "Mac OS X Installer Package", "WinRAR Compressed Archive", "Red Hat Package Manager File", "StuffIt X Archive", "Compressed Tarball File", "Zipped File", "Extended Zip File" };

        public static List<string> DiskImageFiles = new List<string> { ".bin", ".cue", ".dmg", ".iso", ".mdf", ".toast", ".vcd" };
        public static List<string> DiskImageFilesDesc = new List<string> { "Binary Disc Image", "Cue Sheet File", "Apple Disk Image", "Disc Image File", "Media Disc Image File", "Toast Disc Image", "Virtual CD" };

        public static List<string> DeveloperFiles = new List<string> { ".c", ".class", ".cpp", ".cs", ".dtd", ".fla", ".java", ".lua", ".m", ".pl", ".py", ".sh", ".sln", ".swift", ".vb", ".vcxproj", ".xcodeproj" };
        public static List<string> DeveloperFilesDesc = new List<string> { "C/C++ Source Code File", "Java Class File", "C++ Source Code File", "C# Source Code File", "Document Type Definition File", "Adobe Animate Animation", "Java Source Code File", "Lua Source File", "Objective-C Implementation File", "Perl Script", "Python Script", "Bash Shell Script", "Visual Studio Solution File", "Swift Source Code File", "Visual Basic Project Item File", "Visual C++ Project", "Xcode Project" };

        public static List<string> BackupFiles = new List<string> { ".bak", ".tmp" };
        public static List<string> BackupFilesDesc = new List<string> { "Backup File", "Temporary File" };

        public static List<string> MiscFiles = new List<string> { ".ics", ".msi", ".part", ".torrent" };
        public static List<string> MiscFilesDesc = new List<string> { "Calendar File", "Windows Installer Package", "Partially Downloaded File", "BitTorrent File" };

        public static List<string> GetAllExtensionsList()
        {
            List<string> extensions = new List<string>();
            foreach (string ext in TextFiles) extensions.Add(ext);
            foreach (string ext in DataFiles) extensions.Add(ext);
            foreach (string ext in AudioFiles) extensions.Add(ext);
            foreach (string ext in VideoFiles) extensions.Add(ext);
            foreach (string ext in ImageFiles3D) extensions.Add(ext);
            foreach (string ext in RasterImageFiles) extensions.Add(ext);
            foreach (string ext in VectorImageFiles) extensions.Add(ext);
            foreach (string ext in PageLayoutFiles) extensions.Add(ext);
            foreach (string ext in SpreadsheetFiles) extensions.Add(ext);
            foreach (string ext in DatabaseFiles) extensions.Add(ext);
            foreach (string ext in ExecutableFiles) extensions.Add(ext);
            foreach (string ext in GameFiles) extensions.Add(ext);
            foreach (string ext in CADFiles) extensions.Add(ext);
            foreach (string ext in GISFiles) extensions.Add(ext);
            foreach (string ext in WebFiles) extensions.Add(ext);
            foreach (string ext in PluginFiles) extensions.Add(ext);
            foreach (string ext in FontFiles) extensions.Add(ext);
            foreach (string ext in SystemFiles) extensions.Add(ext);
            foreach (string ext in SettingsFiles) extensions.Add(ext);
            foreach (string ext in EncodedFiles) extensions.Add(ext);
            foreach (string ext in CompressedFiles) extensions.Add(ext);
            foreach (string ext in DiskImageFiles) extensions.Add(ext);
            foreach (string ext in DeveloperFiles) extensions.Add(ext);
            foreach (string ext in BackupFiles) extensions.Add(ext);
            foreach (string ext in MiscFiles) extensions.Add(ext);
            return extensions;
        }

        public static List<string> GetAllExtensionsDescriptionsList()
        {
            List<string> descriptions = new List<string>();
            foreach (string ext in TextFilesDesc) descriptions.Add(ext);
            foreach (string ext in DataFilesDesc) descriptions.Add(ext);
            foreach (string ext in AudioFilesDesc) descriptions.Add(ext);
            foreach (string ext in VideoFilesDesc) descriptions.Add(ext);
            foreach (string ext in ImageFiles3DDesc) descriptions.Add(ext);
            foreach (string ext in RasterImageFilesDesc) descriptions.Add(ext);
            foreach (string ext in VectorImageFilesDesc) descriptions.Add(ext);
            foreach (string ext in PageLayoutFilesDesc) descriptions.Add(ext);
            foreach (string ext in SpreadsheetFilesDesc) descriptions.Add(ext);
            foreach (string ext in DatabaseFilesDesc) descriptions.Add(ext);
            foreach (string ext in ExecutableFilesDesc) descriptions.Add(ext);
            foreach (string ext in GameFilesDesc) descriptions.Add(ext);
            foreach (string ext in CADFilesDesc) descriptions.Add(ext);
            foreach (string ext in GISFilesDesc) descriptions.Add(ext);
            foreach (string ext in WebFilesDesc) descriptions.Add(ext);
            foreach (string ext in PluginFilesDesc) descriptions.Add(ext);
            foreach (string ext in FontFilesDesc) descriptions.Add(ext);
            foreach (string ext in SystemFilesDesc) descriptions.Add(ext);
            foreach (string ext in SettingsFilesDesc) descriptions.Add(ext);
            foreach (string ext in EncodedFilesDesc) descriptions.Add(ext);
            foreach (string ext in CompressedFilesDesc) descriptions.Add(ext);
            foreach (string ext in DiskImageFilesDesc) descriptions.Add(ext);
            foreach (string ext in DeveloperFilesDesc) descriptions.Add(ext);
            foreach (string ext in BackupFilesDesc) descriptions.Add(ext);
            foreach (string ext in MiscFilesDesc) descriptions.Add(ext);
            return descriptions;
        }

        public static string[] GetAllExtensionsArray()
        {
            List<string> ext = GetAllExtensionsList();
            string[] extensions = new string[ext.Count];

            for (int i = 0; i < ext.Count; i++) extensions[i] = ext[i];

            return extensions;
        }

        public static string[] GetAllExtensionsDescriptionArray()
        {
            List<string> desc = GetAllExtensionsDescriptionsList();
            string[] descriptions = new string[desc.Count];

            for (int i = 0; i < desc.Count; i++) descriptions[i] = desc[i];

            return descriptions;
        }
    }
}