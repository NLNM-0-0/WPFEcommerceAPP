using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Firebase.Storage;

namespace WPFEcommerceApp {
    public class FireStorageAPI {
        readonly static string BUCKET = "wano-wpf.appspot.com";
        readonly static FirebaseStorage storage = new FirebaseStorage(BUCKET);
        public static async Task<string> Push(string Path, string Root, string Name, params string[] child) {
            var stream = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var task = CreateRef(Root, Name, child);
            int clone = 0;
            string nclone = Name;
            while(await Exist(task)) {
                clone = (clone + new Random().Next(0, 99)) * 2;
                Name = nclone + $"_{clone}";
                task = CreateRef(Root, Name, child);
            }
            var downloadUrl = await task.PutAsync(stream);
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

        static async Task<bool> Exist(FirebaseStorageReference ins) {
            try {
                await ins.GetMetaDataAsync();
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
