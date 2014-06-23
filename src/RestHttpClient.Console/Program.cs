using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RestHttpClient.Cmd
{
    public class Program
    {
        const string _apiKey = "apikey";
        public static void Main(string[] args)
        {
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                    SendFile(args);
                else
                {
                    WriteMessage("No administrator user", ConsoleColor.Red);
                    
                }
            }
            catch (Exception ex)
            {
                WriteMessage(ex.Message, ConsoleColor.Red);
                SendFile(new String[] { Console.ReadLine() });
            }

        }

        static void SendFile(string[] args)
        {

            if (!SintaxHelper.ValidSintaxCommand(args))
                SendFile(new String[] { Console.ReadLine() });
            else if (HasApiKey())
            {
                var commandArgs = GetCommandArgs(args);
                var restSrv = new RestHttpClient.HttpService();
                restSrv.OnSending += restSrv_OnSending;

                var token = restSrv.GetToken(GetApiKeyValue());
                var login = restSrv.Login(token, commandArgs.UserName, commandArgs.Password);
                var uploadInfo = restSrv.GetUploadInf(login);
                Console.WriteLine("Aguarde envio!");
                restSrv.UploadFileAsync(commandArgs.FileName, uploadInfo);

                Console.ReadKey();
            }
            else
            {
                WriteMessage("Não foi configurada API KEY", ConsoleColor.Red);
                WriteMessage("Informe a API Key: ", ConsoleColor.Green);
                Console.ForegroundColor = ConsoleColor.White;
                var apiKey = Console.ReadLine();

                if (string.IsNullOrEmpty(apiKey))
                {
                    WriteMessage("API KEY não informada", ConsoleColor.Red);
                    SendFile(args);
                }
                SetApiKey(apiKey);
                SendFile(args);
            }
        }

        static void restSrv_OnSending(object sender, Events.EventProgressArgs args)
        {
            if (!args.Done)
            {
                Console.Clear();
                Console.Write(string.Format("Sending {0}%", args.ProgressInfo.Meter));
            }
            else
            {
                WriteMessage("File uploaded", ConsoleColor.Green);
                Console.ReadKey();
            }

        }



        static CommandArgs GetCommandArgs(string[] args)
        {
            var command = args[0];
            var commandArgs = new CommandArgs();

            var allArgs = command.Split(' ');
            foreach (var item in allArgs)
            {
                if (item.ToLower().StartsWith("file:"))
                    commandArgs.FileName = item.Substring(5, item.Length - 5);
                else if (item.ToLower().StartsWith("user:"))
                    commandArgs.UserName = item.Substring(5, item.Length - 5);
                else if (item.ToLower().StartsWith("pwd:"))
                    commandArgs.Password = item.Substring(4, item.Length - 4);
            }

            return commandArgs;
        }



        static bool HasApiKey()
        {

            return !string.IsNullOrEmpty(GetApiKeyValue());
        }

        private static string GetApiKeyValue()
        {
            var config = ConfigurationSettings.AppSettings;
            var apiKeyValue = config[_apiKey];

            return apiKeyValue;
        }

        static void WriteMessage(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
        }

        static void SetApiKey(string apiKey)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[_apiKey].Value = apiKey;
            config.Save();

            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
