using RestHttpClient.Results;
using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RestHttpClient
{
    public class HttpService
    {
        string _apiKeyDefault = "PSKWEA71BT";
        string _baseUri = "http://api.sendspace.com/rest/";

        public TokenResult GetToken(string apiKey)
        {
            var uriCreateToken = string.Format(@"?method=auth.createtoken&api_key={0}&api_version=1.0&response_format=xml&app_version=0.1", string.IsNullOrEmpty(apiKey) ? _apiKeyDefault : apiKey);

            var restClient = new RestClient(_baseUri);
            var request = new RestRequest(uriCreateToken, Method.GET);

            var tokenResult = restClient.Get<TokenResult>(request);

            return tokenResult.Data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userName"></param>
        /// <param name="password">lowercase(md5(token+lowercase(md5(password)))) - md5 values should always be lowercase.</param>
        public LoginResult Login(TokenResult token, string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                throw new Exception("Por favor, informe um nome de usuário e senha");

            var md5Pwd = CalculateMD5Hash(password).ToLower();
            var passwordLowerMD5 = CalculateMD5Hash(token.Token + md5Pwd).ToLower();
            var uriLogin = string.Format(@"?method=auth.login&token={0}&user_name={1}&tokened_password={2}", token.Token, userName, passwordLowerMD5);

            var restClient = new RestClient(_baseUri);
            var request = new RestRequest(uriLogin, Method.GET);

            var result = restClient.Get<LoginResult>(request);
            return result.Data;
        }

        public ResultUpload GetUploadInf(LoginResult login)
        {
            if (login == null)
                throw new Exception("Dados de login não informados");

            var uriUploadInfo = string.Format("?method=upload.getinfo&session_key={0}&speed_limit={1}", login.SessionKey, 0);

            var restClient = new RestClient(_baseUri);
            var request = new RestRequest(uriUploadInfo, Method.GET);

            var result = restClient.Get<ResultUpload>(request);
            var xml = new RestSharp.Deserializers.XmlDeserializer();


            var resultUpload = GetResultUploadBy(result.Content);

            return resultUpload;
        }

        private ResultUpload GetResultUploadBy(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                throw new Exception("Informações de upload não informada");

            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(ResultUpload));
            var fileName = Path.GetTempFileName();

            File.WriteAllText(fileName, xml, Encoding.UTF8);

            var xmlDoc = new XmlTextReader(fileName);

            var resultUpload = new ResultUpload();

            while (xmlDoc.Read())
            {
                switch (xmlDoc.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.

                        if (xmlDoc.Name.Equals("upload"))
                        {
                            if (xmlDoc.HasAttributes)
                            {
                                resultUpload.url = xmlDoc.GetAttribute("url");
                                resultUpload.progress_url = xmlDoc.GetAttribute("progress_url");
                                resultUpload.max_file_size = xmlDoc.GetAttribute("max_file_size");
                                resultUpload.upload_identifier = xmlDoc.GetAttribute("upload_identifier");
                                resultUpload.extra_info = xmlDoc.GetAttribute("extra_info");

                            }
                            return resultUpload;
                        }
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        Console.WriteLine(xmlDoc.Value);
                        break;
                    case XmlNodeType.EndElement: //Display the end of the element.
                        Console.Write("</" + xmlDoc.Name);
                        Console.WriteLine(">");
                        break;
                }
            }



            return resultUpload;
        }

        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public void SendFile(string fileName, ResultUpload resultUpload)
        {

            var progress = new ProgressMessageHandler();

            var client = HttpClientFactory.Create(progress);

            var method = new MultipartFormDataContent();

            var streamContent = new StreamContent(File.Open(fileName, FileMode.Open));
            method.Add(streamContent, "filename");

            var result = client.PostAsync(resultUpload.url, method);

        }

        public async Task<string> Upload(string fileName, ResultUpload resultUpload)
        {
            using (var client = new HttpClient())
            {
                using (var content =
                    new MultipartFormDataContent())
                {

                    content.Headers.Add("extra_info", resultUpload.extra_info);
                    content.Headers.Add("upload_identifier", resultUpload.upload_identifier);
                    content.Headers.Add("max_file_size", resultUpload.max_file_size);
                    content.Add(new StreamContent(new MemoryStream(File.ReadAllBytes(fileName))), "bilddatei", "upload.jpg");
                    content.Add(new StringContent("extra_info=" + resultUpload.extra_info));
                    content.Add(new StringContent("upload_identifier=" + resultUpload.upload_identifier));
                    content.Add(new StringContent("max_file_size=" + resultUpload.max_file_size));


                    var list = new List<KeyValuePair<string, string>>(){
                        new KeyValuePair<string, string>("extra_info", resultUpload.extra_info),
                            new KeyValuePair<string, string>("upload_identifier", resultUpload.upload_identifier),
                            new KeyValuePair<string, string>("max_file_size", resultUpload.max_file_size)
                    };
                    content.Add(new FormUrlEncodedContent(list));





                    using (
                       var message =
                           await client.PostAsync(resultUpload.url, content))
                    {
                        var input = await message.Content.ReadAsStringAsync();

                        return !string.IsNullOrWhiteSpace(input) ? System.Text.RegularExpressions.Regex.Match(input, @"http://\w*\.directupload\.net/images/\d*/\w*\.[a-z]{3}").Value : null;
                    }
                }
            }
        }

        public void UploadFile(string fileName, ResultUpload resultUpload)
        {
            if (!File.Exists(fileName))
                throw new Exception("Arquivo não encontrado");

            RestRequest request = new RestRequest(resultUpload.url, Method.POST);

            var file = request.AddFile("userfile", File.ReadAllBytes(fileName), Path.GetFileName(fileName), "image/pjpeg");

            request.AddParameter("MAX_FILE_SIZE", resultUpload.max_file_size);
            request.AddParameter("UPLOAD_IDENTIFIER", resultUpload.upload_identifier);
            request.AddParameter("extra_info", resultUpload.extra_info);
            //calling server with restClient
            RestClient restClient = new RestClient();
            restClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //upload successfull

                }
                else
                {
                    //error ocured during upload

                }
            });
        }
    }
}
