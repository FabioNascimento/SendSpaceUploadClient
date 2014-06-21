using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestHttpClient.Cmd
{
    public class Program
    {
        const string _apiKey = "apikey";
        public static void Main(string[] args)
        {
            SendFile(args);
        }

        static void SendFile(string[] args)
        {
            args = new string[] { @"file:e:\BugNET-1.5.265-Install.zip user:moisesmiranda pwd:Mjm007#!" };
            ValidSintaxCommand(args);
            if (HasApiKey())
            {
                var commandArgs = GetCommandArgs(args);
                var restSrv = new RestHttpClient.HttpService();
                restSrv.OnSending += restSrv_OnSending;

                var token = restSrv.GetToken(GetApiKeyValue());
                var login = restSrv.Login(token, commandArgs.UserName, commandArgs.Password);
                var uploadInfo = restSrv.GetUploadInf(login);
                restSrv.UploadFile(commandArgs.FileName, uploadInfo);
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

        static void ValidSintaxCommand(string[] args)
        {
            if (args.Length == 0)
                WriteHelpMessage();

            var command = args[0];

            var commandArgs = command.Split(' ');
            if (commandArgs.Length != 3)
                WriteHelpMessage();

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

        private static void WriteHelpMessage()
        {
            //foreach (ConsoleColor item in Enum.GetValues(typeof(ConsoleColor)))
            //{
            //    WriteMessage(item.ToString()+"-Cor", item);
            //}
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("sendspace <file:path user:username pwd:password");
            Console.WriteLine();
            Console.WriteLine(string.Format("{0,-12}{1,-30}", "file", "full path file for send")); ;
            Console.WriteLine(string.Format("{0,-12}{1,-30}", "user", "user name of account send space")); ;
            Console.WriteLine(string.Format("{0,-12}{1,-30}", "pwd", "password of user account send space")); ;

            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
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
