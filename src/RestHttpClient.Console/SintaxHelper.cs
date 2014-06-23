using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestHttpClient.Cmd
{
    public class SintaxHelper
    {
        public static bool ValidSintaxCommand(string[] args)
        {
            if (args.Length == 0)
            {
                WriteHelpMessage();
                
                return false;
            }

            var command = args[0];

            var commandArgs = command.Split(' ');
            if (commandArgs.Length < 2 || commandArgs.Length>3)
            {
                WriteHelpMessage();
                return false;
            }

            return true;
        }



        public static void WriteHelpMessage()
        {
            //foreach (ConsoleColor item in Enum.GetValues(typeof(ConsoleColor)))
            //{
            //    WriteMessage(item.ToString()+"-Cor", item);
            //}
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("sendspace <file=path user=username>");
            Console.WriteLine();
            Console.WriteLine(string.Format("{0,-16}{1,-30}", "file", "full path file for send")); 
            Console.WriteLine(string.Format("{0,-16}{1,-30}", "user", "user name of account send space")); 
            Console.WriteLine(string.Format("{0,-16}{1,-30}", "pwd", "password of user account send space"));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(string.Format("{0,-16}{1,-30}", "exemplo de uso", @"sendspace file=c:\file.txt user=username")); 

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
