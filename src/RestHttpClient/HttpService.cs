using RestHttpClient.Events;
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
        public event OnSendingHandler OnSending;

        string _apiKeyDefault = "PSKWEA71BT";
        string _baseUri = "http://api.sendspace.com/rest/";
        string _statusProgress;
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

        public ProgressUpload GetProgressUpload(ResultUpload resultUpload)
        {
            if (resultUpload == null)
                throw new Exception("Dados upload não informado");


            var restClient = new RestClient();
            var request = new RestRequest(resultUpload.progress_url, Method.GET);
            var progressUpload = restClient.Get<ProgressUpload>(request);

            return progressUpload.Data;
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


        public async void UploadFile(string fileName, ResultUpload resultUpload)
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

            var _statusProgress = GetProgressUpload(resultUpload).Status;
            
            while (_statusProgress.ToLower().Equals("ok"))
            {

                var progressInternal = GetProgressUpload(resultUpload);
                SetProgressEventArgs(progressInternal, false);
                _statusProgress = progressInternal.Status;
            }

            SetProgressEventArgs(GetProgressUpload(resultUpload), true);

        }

        void SetProgressEventArgs(ProgressUpload progressUpload, bool done)
        {
            if (OnSending != null)
            {
                var args = new EventProgressArgs() { Done = done, ProgressInfo = progressUpload };
                OnSending(this, args);
            }
        }
    }
}
