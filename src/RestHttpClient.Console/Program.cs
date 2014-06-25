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
                    WriteLineMessage("No administrator user", ConsoleColor.Red);

                }
            }
            catch (Exception ex)
            {
                WriteLineMessage(ex.Message, ConsoleColor.Red);
                SendFile(new String[] { Console.ReadLine() });
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

        }

        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {

                        password = password.Substring(0, password.Length - 1);

                        int pos = Console.CursorLeft;

                        Console.SetCursorPosition(pos - 1, Console.CursorTop);

                        Console.Write(" ");

                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password;
        }


        static void SendFile(string[] args)
        {
            Console.Title = "Sendspace upload";
            Console.ForegroundColor = ConsoleColor.White;
            if (!SintaxHelper.ValidSintaxCommand(args))
                SintaxHelper.WriteHelpMessage();
            else if (HasApiKey())
            {
                var commandArgs = GetCommandArgs(args);
                WriteMessage("Password: ", ConsoleColor.White);
                commandArgs.Password = ReadPassword();
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
                WriteLineMessage("Não foi configurada API KEY", ConsoleColor.Red);
                WriteLineMessage("Informe a API Key: ", ConsoleColor.Green);
                Console.ForegroundColor = ConsoleColor.White;
                var apiKey = Console.ReadLine();

                if (string.IsNullOrEmpty(apiKey))
                {
                    WriteLineMessage("API KEY não informada", ConsoleColor.Red);
                    SendFile(args);
                }
                SetApiKey(apiKey);
                SendFile(args);
            }
        }

        static void restSrv_OnSending(object sender, Events.EventProgressArgs args)
        {
            var count = 1;
            if (!args.Done)
            {
                ClearCurrentConsoleLine();
                var msgPercent = string.Format("Sending {0}%", args.ProgressInfo.Meter);
                Console.Write(msgPercent);
            }
            else
            {
                WriteLineMessage("", ConsoleColor.White);
                WriteLineMessage("File uploaded", ConsoleColor.Green);
                
            }
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write(" ");
            Console.SetCursorPosition(0, currentLineCursor);
        }

        static CommandArgs GetCommandArgs(string[] args)
        {
            
            var commandArgs = new CommandArgs();

            
            foreach (var item in args)
            {
                if (item.ToLower().StartsWith("file="))
                    commandArgs.FileName = item.Substring(5, item.Length - 5);
                else if (item.ToLower().StartsWith("user="))
                    commandArgs.UserName = item.Substring(5, item.Length - 5);
                else if (item.ToLower().StartsWith("pwd="))
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

        static void WriteLineMessage(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
        }

        static void WriteMessage(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(msg);
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
