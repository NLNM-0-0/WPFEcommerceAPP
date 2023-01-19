using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Firebase.Storage;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace WPFEcommerceApp {
    public class FireStorageAPI {
        readonly static string BUCKET = "wano-wpf.appspot.com";
        readonly static string FireStorageEndpoint = "https://firebasestorage.googleapis.com/v0/b/";
        readonly static FirebaseStorage storage = new FirebaseStorage(BUCKET);
        const string tempJPG = "CreateTempJpg.jpg";
        const string tempIMG = "TempIMG.jpg";
        public static async Task<string> Push(string Path, string Root, string Name, params string[] child) {
            var stream = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read);

            bool CreatedFile = false;
            System.Drawing.Image img = new Bitmap(stream);
            if(!img.RawFormat.Equals(ImageFormat.Jpeg)) {
                using(var bitmap = new Bitmap(img.Width, img.Height)) {
                    bitmap.SetResolution(img.HorizontalResolution, img.VerticalResolution);

                    using(var graphic = Graphics.FromImage(bitmap)) {
                        graphic.Clear(Color.White);
                        graphic.DrawImageUnscaled(img, 0, 0);
                    }
                    bitmap.Save(tempJPG, ImageFormat.Jpeg);
                }

                stream.Close();
                stream = File.Open(tempJPG, FileMode.Open, FileAccess.Read, FileShare.Read);
                CreatedFile = true;
            }
            else {
                stream.Close();
                stream = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            var task = CreateRef(Root, Name, child);
            int clone = 0;
            string nclone = Name;
            while(await Exist(Root, Name, child)) {
                clone = (clone + new Random().Next(0, 99)) * 2;
                Name = nclone + $"_{clone}";
                task = CreateRef(Root, Name, child);
            }

            var downloadUrl = await task.PutAsync(stream);
            stream.Close();
            if(CreatedFile) File.Delete(tempJPG);
            return downloadUrl;
        }


        public static async Task<Tuple<bool, string>> PushFromFile(
            string Path, 
            string Root, 
            string Name, 
            string OldPath = null, 
            params string[] child) {

            Uri uriResult;
            bool result = Uri.TryCreate(Path, UriKind.Absolute, out uriResult)
                 && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if(result)
                return new Tuple<bool, string>(false, "");

            if(OldPath != null && OldPath.Length > 0) {
                await Delete(OldPath);
            }

            var res = await Push(Path, Root, Name, child);
            return new Tuple<bool, string>(true, res);
        }

        public static async Task<string> PushFromImage(
            BitmapSource bm, 
            string Root,
            string Name,
            string OldPath = null,
            params string[] child) {

            FileStream stream = new FileStream(tempIMG, FileMode.Create);
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bm));
            encoder.Save(stream);
            stream.Close();

            if(OldPath != null && OldPath.Length > 0) {
                await Delete(OldPath);
            }

            var res = await Push(tempIMG, Root, Name, child);
            File.Delete(tempIMG);
            return res;
        }
        public static async Task<bool> Delete(string Path) {
            var delContent = "N/A";
            try {
                using(var http = await storage.Options.CreateHttpClientAsync().ConfigureAwait(false)) {
                    var del = await http.DeleteAsync(new Uri(Path)).ConfigureAwait(false);
                    delContent = await del.Content.ReadAsStringAsync().ConfigureAwait(false);
                    del.EnsureSuccessStatusCode();
                }
            }
            catch(Exception ex) {
                throw new FirebaseStorageException(Path, delContent, ex);
            }
            return true;
        }

        static async Task<bool> Exist(string Root, string Name, params string[] child) {
            List<string> childs = new List<string>();
            childs.Add(Root);
            childs = childs.Concat(child).ToList();
            childs.Add(Name);
            try {
                var downloadUrl = $"{FireStorageEndpoint}{BUCKET}/o/{Uri.EscapeDataString(string.Join("/", childs))}?time={DateTime.Now}";
                using(var http = new HttpClient()) {
                    http.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue {
                        NoCache = true,
                        NoStore = true,
                    };
                    //respone will keep return cache if cacheLevel is CacheIfAvailable
                    //=> Should not use this policy
                    //=> But in this app, just store image, so we can use this
                    var result = await http.GetAsync(downloadUrl);
                    var resultContent = await result.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<FirebaseMetaData>(resultContent);
                    if(data.ContentType == null) {
                        return false;
                    }
                }
            }
            catch {
                return false;
            }
            return true;
        }
        static FirebaseStorageReference CreateRef(string Root, string Name, params string[] child) {
            var ins = storage.Child(Root);
            for(int i = 0; i < child.Length; i++) {
                ins = ins.Child(child[i]);
            }
            ins = ins.Child(Name);
            return ins;
        }
    }
}
