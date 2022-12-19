using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Storage;

namespace WPFEcommerceApp {
    public class FireStorageAPI {
        readonly static string BUCKET = "wano-wpf.appspot.com";
        public static async Task<string> Push(string Path, string Root, string Name, params string[] child) {
            var stream = File.Open(Path, FileMode.Open);
            var task = CreateRef(Root, Name, child);
            int clone = 0;
            string nclone = Name;
            while(await Exist(task)) {
                Name = nclone + $"_{clone++}";
                task = CreateRef(Root, Name, child);
            }
            var downloadUrl = await task.PutAsync(stream);
            return downloadUrl;
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
            var ins = new FirebaseStorage(BUCKET).Child(Root);
            for(int i = 0; i < child.Length; i++) {
                ins = ins.Child(child[i]);
            }
            ins = ins.Child(Name);
            return ins;
        }
    }
}
