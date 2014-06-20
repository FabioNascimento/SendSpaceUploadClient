using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RestHttpClient.Tests
{
    [TestFixture]
    public class SendSpaceTest
    {
        
        public void Test()
        {
            var httpSrv = new HttpService();
            var token = httpSrv.GetToken(null);
            var login = httpSrv.Login(token, "youruser", "your password");
            var uploadInfo = httpSrv.GetUploadInf(login);


            var fileName = @"..\..\..\xml\upload.xsd";

            httpSrv.UploadFile(fileName, uploadInfo);

            httpSrv.SendFile(fileName, uploadInfo);

            
        }
        
        public void DeveExisteArquivoParaEnvio()
        {
            var fileName = @"..\..\..\xml\upload.xsd";
            Assert.AreEqual(true, File.Exists(fileName));
        }
    }
}
