using System.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net.Cache;

namespace WPFEcommerceApp {
    public static class FileCache {
        public enum CacheMode {
            FromIE,
            Local,
        };

        // Record whether a file is being written.
        private static readonly Dictionary<string, bool> IsWritingFile = new Dictionary<string, bool>();

        // Timeout for performing the file download request.
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(5);

        // HttpClient is intended to be instantiated once per application, rather than per-use.
        private static readonly Lazy<HttpClient> LazyHttpClient = new Lazy<HttpClient>(() => new HttpClient());
        public static string CacheDirectory { get; set; }

        public static RequestCachePolicy CachePolicy = new RequestCachePolicy(RequestCacheLevel.Default);

        static FileCache() {
            CacheDirectory = string.Format("{0}\\{1}\\Cache\\",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Process.GetCurrentProcess().ProcessName);
        }

        public static async Task<MemoryStream> SaveCache(string url, string subfolder = "image") {
            if(!Directory.Exists(CacheDirectory + subfolder)) {
                Directory.CreateDirectory(CacheDirectory + subfolder);
            }
            var uri = new Uri(url);
            var fileNameBuilder = new StringBuilder();
            using(var sha1 = new SHA1Managed()) {
                var canonicalUrl = uri.ToString();
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(canonicalUrl));
                fileNameBuilder.Append(BitConverter.ToString(hash).Replace("-", "").ToLower());
                if(Path.HasExtension(canonicalUrl))
                    fileNameBuilder.Append(Path.GetExtension(canonicalUrl).Split('?')[0]);
            }

            var fileName = fileNameBuilder.ToString();
            string localFile;
            if(!string.IsNullOrEmpty(subfolder))
                localFile = string.Format("{0}{1}\\{2}", CacheDirectory, subfolder, fileName);
            else
                localFile = string.Format("{0}\\{1}", CacheDirectory, fileName);
            var memoryStream = new MemoryStream();

            FileStream fileStream = null;
            if(!IsWritingFile.ContainsKey(fileName) && File.Exists(localFile)) {
                using(fileStream = new FileStream(localFile, FileMode.Open, FileAccess.Read)) {
                    await fileStream.CopyToAsync(memoryStream);
                }
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }

            var client = LazyHttpClient.Value;
            //client.Timeout = RequestTimeout;
            try {
                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

                if(response.IsSuccessStatusCode is false) {
                    return null;
                }

                var responseStream = await response.Content.ReadAsStreamAsync();

                if(!IsWritingFile.ContainsKey(fileName)) {
                    IsWritingFile[fileName] = true;
                    fileStream = new FileStream(localFile, FileMode.Create, FileAccess.Write);
                }

                using(responseStream) {
                    var bytebuffer = new byte[100];
                    int bytesRead;
                    do {
                        bytesRead = await responseStream.ReadAsync(bytebuffer, 0, 100);
                        if(fileStream != null)
                            await fileStream.WriteAsync(bytebuffer, 0, bytesRead);
                        await memoryStream.WriteAsync(bytebuffer, 0, bytesRead);
                    } while(bytesRead > 0);
                    if(fileStream != null) {
                        await fileStream.FlushAsync();
                        fileStream.Dispose();
                        IsWritingFile.Remove(fileName);
                    }
                }
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            } catch(WebException) {
                return null;
            }
        }
    }
}