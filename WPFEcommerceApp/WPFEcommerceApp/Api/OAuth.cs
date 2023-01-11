using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net.Mail;
using System.Windows.Threading;

namespace WPFEcommerceApp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class OAuth {
        
        #region RetrieveData
        public async Task<Tuple<string, object>> Authentication() {
            // Generates state and PKCE values.
            string state = randomDataBase64url(32);
            string code_verifier = randomDataBase64url(32);
            string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
            const string code_challenge_method = "S256";

            // Creates a redirect URI using an available port on the loopback address.
            string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());
            //output("redirect URI: " + redirectURI);

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectURI);
            //output("Listening..");
            http.Start();

            // Creates the OAuth 2.0 authorization request.
            // Scope is declare in here
            var scope = CreateScopes(scopes);
            string authorizationRequest = string.Format("{0}?response_type=code&scope={1}&redirect_uri={2}&client_id={3}&state={4}&code_challenge={5}&code_challenge_method={6}",
                authorizationEndpoint,
                scope,
                System.Uri.EscapeDataString(redirectURI),
                clientID,
                state,
                code_challenge,
                code_challenge_method);

            // Opens request in the browser.
            System.Diagnostics.Process.Start(authorizationRequest);

            // Waits for the OAuth authorization response.
            HttpListenerContext context = null;

            var token = new CancellationTokenSource();
            token.CancelAfter(8228);
            try {
                context = await http.GetContextAsync().AsCancellable(token.Token);
            } catch {
                return null;
            }

            // Sends an HTTP response to the browser.
            var response = context.Response;
            string responseString = string.Format(
                "<html><head><meta http-equiv='refresh' content='2;url=https://google.com'></head><body><h2>Return to the WANO....</h2></body></html>"
                );
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) => {
                responseOutput.Close();
                http.Stop();
                Console.WriteLine("HTTP server stopped.");
            }, TaskScheduler.FromCurrentSynchronizationContext());
            LoginViewModel.IsLoading = true;

            // Checks for errors.
            if(context.Request.QueryString.Get("error") != null) {
                output(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
                return new Tuple<string, object>(null, null);
            }
            if(context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null) {
                output("Malformed authorization response. " + context.Request.QueryString);
                return new Tuple<string, object>(null, null);
            }

            // extracts the code
            var code = context.Request.QueryString.Get("code");
            var incoming_state = context.Request.QueryString.Get("state");

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if(incoming_state != state) {
                output(String.Format("Received request with invalid state ({0})", incoming_state));
                return new Tuple<string, object>(null, null);
            }
            //output("Authorization code: " + code);

            // Starts the code exchange at the Token Endpoint.
            return await performCodeExchange(code, code_verifier, redirectURI);
        }
        async Task<Tuple<string, object>> performCodeExchange(string code, string code_verifier, string redirectURI) {
            //output("Exchanging code for tokens...");

            // builds the  request
            string tokenRequestURI = tokenEndpoint;
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
                code,
                Uri.EscapeDataString(redirectURI),
                clientID,
                code_verifier,
                clientSecret
                );

            // sends the request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            Tuple<string, object> res = new Tuple<string, object>(null, null);
            object Item3 = null;
            try {
                // gets the response
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using(StreamReader reader = new StreamReader(tokenResponse.GetResponseStream())) {
                    // reads response body
                    string responseText = await reader.ReadToEndAsync();
                    //output(responseText);

                    // converts to dictionary
                    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    string access_token = tokenEndpointDecoded["access_token"];
                    string refresh_token = tokenEndpointDecoded["refresh_token"];
                    res = new Tuple<string, object>(refresh_token, null);
                    Item3 = await userinfoCall(access_token);
                }
            } catch(WebException ex) {
                if(ex.Status == WebExceptionStatus.ProtocolError) {
                    var response = ex.Response as HttpWebResponse;
                    if(response != null) {
                        output("HTTP: " + response.StatusCode);
                        using(StreamReader reader = new StreamReader(response.GetResponseStream())) {
                            // reads response body
                            string responseText = await reader.ReadToEndAsync();
                            output(responseText);
                        }
                        return new Tuple<string, object>(null, null);
                    }

                }
            }
            return new Tuple<string, object>(res.Item1, Item3);
        }

        async Task<object> userinfoCall(string access_token) {
            //output("Making API Call to Userinfo...");

            // builds the  request
            string userinfoRequestURI = userInfoEndpoint;

            try {
                HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
                userinfoRequest.Method = "GET";
                userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
                userinfoRequest.ContentType = "application/x-www-form-urlencoded";
                userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

                // gets the response
                WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
                using(StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream())) {
                    // reads response body
                    string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
                    return await getUserDetail(userinfoResponseText, access_token);
                }
            } catch { Debug.WriteLine("UserInfo"); return null; }
        }

        async Task<object> getUserDetail(string json, string access_token) {
            var jsonParse = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            string userinfoRequestURI = CreatePeopleEndpoint(jsonParse["sub"], personFields);

            // sends the request
            try {
                HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
                userinfoRequest.Method = "GET";
                userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
                userinfoRequest.ContentType = "application/x-www-form-urlencoded";
                userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

                // gets the response
                WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
                using(StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream())) {
                    // reads response body
                    string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();
                    var parse = JsonConvert.DeserializeObject<Dictionary<string, object>>(userinfoResponseText);
                    return new Tuple<object, object>(jsonParse, parse);
                }
            } catch { Debug.WriteLine("UserDetail"); return new Tuple<object, object>(jsonParse, null); }
        }
        #endregion
        #region MethodsConfig
        public async Task<bool> SendEmail(string receiver, string Subject, string Body) {
            try {
                using(MailMessage mail = new MailMessage()) {
                    mail.From = new MailAddress(email);
                    mail.To.Add(receiver);
                    mail.Subject = Subject;
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    mail.Priority = MailPriority.High;

                    using(SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)) {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(email, apwsrt);
                        smtp.EnableSsl = true;
                        await Task.Run(() => smtp.Send(mail));
                    }
                }
                return true;
            } catch { return false; }
        }
        public static int GetRandomUnusedPort() {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
        void output(string output) {
            Console.WriteLine(output);
        }

        string randomDataBase64url(uint length) {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return base64urlencodeNoPadding(bytes);
        }

        byte[] sha256(string inputStirng) {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        string base64urlencodeNoPadding(byte[] buffer) {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }
        #endregion
        #region ClientConfig
        string clientID = "MTI5MTA1NDQxMjY4LXNta2Z1dDUyZDAzaXRkbnJnZjJmcWUxc2FpcHVkOGMyLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29t";
        string clientSecret = "R09DU1BYLWNBT1RaSGxzTXJQc2xzRk5pWHNJRnVTOVp1SWw=";
        string email = "d2Fub3NlcnZpY2Uubm9yZXBseUBnbWFpbC5jb20=";
        string apwsrt = "Y3R0YWdyY3RieXZiamRkdA==";
        const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        const string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";
        const string peopleEndpoint = "https://people.googleapis.com/v1/people/";
        public OAuth() {
            clientID = Hashing.Base64Decode(clientID);
            clientSecret = Hashing.Base64Decode(clientSecret);
            email = Hashing.Base64Decode(email);
            apwsrt = Hashing.Base64Decode(apwsrt);
        }
        string CreatePeopleEndpoint(string id, string[] personFields) {
            string res = peopleEndpoint;
            res += id + "?personFields=";
            for(int i = 0; i < personFields.Length - 1; i++) {
                res += personFields[i] + ",";
            }
            res += personFields[personFields.Length-1];
            return res;
        }
        string CreateScopes(string[] scopes) {
            string x = "";
            for(int i = 0; i < scopes.Length - 1; i++) {
                x += scopes[i] + "%20";
            }
            x += scopes[scopes.Length-1];
            return x;
        }
        string[] personFields = { "genders", "birthdays", "coverPhotos", "phoneNumbers" };
        string[] scopes = {
                //"https://www.googleapis.com/auth/user.birthday.read",
                //"https://www.googleapis.com/auth/user.gender.read",
                //"https://www.googleapis.com/auth/user.phonenumbers.read",
                "https://www.googleapis.com/auth/userinfo.email",
                "https://www.googleapis.com/auth/userinfo.profile"
        };
        #endregion
    }
}
