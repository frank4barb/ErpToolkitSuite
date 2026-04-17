using Google.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ErpToolkit.Helpers
{

    //Dal DB -> risposta HTTP
    //var mime = MimeMapping.GetMimeFromExtension(fileExtension);
    //return File(data, mime, enableRangeProcessing: true);
    
    //Da MIME -> nome file
    //var ext = MimeMapping.GetDefaultExtensionFromMime(contentType);
    //var fileName = $"documento.{ext}";

    //Viewer(come il tuo TagHelper)
    //if (MimeMapping.IsRenderableInBrowser(contentType)){    
    //  usa <img>, <object>, <audio>, <video>
    // }else{    
    //  forza download
    //  }
    public static class MimeMapping
    {
        // estensione → mime
        private static readonly Dictionary<string, string> _extToMime =
            new(StringComparer.OrdinalIgnoreCase)
        {
            // ---------- Documenti ----------
            ["txt"]  = "text/plain",
            ["csv"]  = "text/csv",
            ["htm"]  = "text/html",
            ["html"] = "text/html",
            ["xml"]  = "application/xml",
            ["pdf"]  = "application/pdf",
            ["rtf"]  = "application/rtf",
            ["doc"]  = "application/msword",
            ["docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ["xls"]  = "application/vnd.ms-excel",
            ["xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ["ppt"]  = "application/vnd.ms-powerpoint",
            ["pptx"] = "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ["json"] = "application/json",

            // ---------- Immagini ----------
            ["jpg"]  = "image/jpeg",
            ["jpeg"] = "image/jpeg",
            ["png"]  = "image/png",
            ["gif"]  = "image/gif",
            ["bmp"]  = "image/bmp",
            ["svg"]  = "image/svg+xml",
            ["webp"] = "image/webp",
            ["tiff"] = "image/tiff",
            ["ico"]  = "image/x-icon",

            // ---------- Audio ----------
            ["mp3"]  = "audio/mpeg",
            ["wav"]  = "audio/wav",
            ["ogg"]  = "audio/ogg",
            ["aac"]  = "audio/aac",
            ["flac"] = "audio/flac",
            ["m4a"]  = "audio/mp4",

            // ---------- Video ----------
            ["mp4"]  = "video/mp4",
            ["webm"] = "video/webm",
            ["ogv"]  = "video/ogg",
            ["avi"]  = "video/x-msvideo",
            ["mov"]  = "video/quicktime",
            ["mkv"]  = "video/x-matroska",

            // ---------- Archivi ----------
            ["zip"]  = "application/zip",
            ["rar"]  = "application/vnd.rar",
            ["7z"]   = "application/x-7z-compressed",
            ["tar"]  = "application/x-tar",
            ["gz"]   = "application/gzip"
        };

        // mime → estensioni
        private static readonly Dictionary<string, string[]> _mimeToExt =
            _extToMime
                .GroupBy(kvp => kvp.Value, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Key).ToArray(),
                    StringComparer.OrdinalIgnoreCase
                );

        // ----------------------------
        // API PUBBLICA
        // ----------------------------

        public static string GetMimeFromExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
                return "application/octet-stream";

            extension = NormalizeExtension(extension);

            return _extToMime.TryGetValue(extension, out var mime)
                ? mime
                : "application/octet-stream";
        }

        public static string GetDefaultExtensionFromMime(string mime)
        {
            if (string.IsNullOrWhiteSpace(mime))
                return string.Empty;

            return _mimeToExt.TryGetValue(mime, out var exts)
                ? exts[0]
                : string.Empty;
        }

        public static IReadOnlyList<string> GetExtensionsFromMime(string mime)
        {
            if (string.IsNullOrWhiteSpace(mime))
                return Array.Empty<string>();

            return _mimeToExt.TryGetValue(mime, out var exts)
                ? exts
                : Array.Empty<string>();
        }

        public static bool IsRenderableInBrowser(string mime)
        {
            if (string.IsNullOrWhiteSpace(mime))
                return false;

            return mime.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                || mime.StartsWith("audio/", StringComparison.OrdinalIgnoreCase)
                || mime.StartsWith("video/", StringComparison.OrdinalIgnoreCase)
                || mime == "application/pdf"
                || mime.StartsWith("text/");
        }

        private static string NormalizeExtension(string ext)
        {
            ext = ext.Trim().ToLowerInvariant();
            return ext.StartsWith(".") ? ext[1..] : ext;
        }
    }
}
