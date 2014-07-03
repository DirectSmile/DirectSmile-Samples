using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LBSampleCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            string serviceUrl = "http://yourserver/dsmo/lb.asmx";
            string username = "your username";
            string password = "your password";


             string xml  = @"<?xml version=""1.0"" encoding=""utf-8""?><Command>
                      <Parameters>
                          <FandV FieldName=""Cache"" FieldValue=""OFF""/>
                          <FandV FieldName=""OUTPUT"" FieldValue=""PDFPREVIEW""/>
                          <FandV FieldName=""JPGCOMPRESSION"" FieldValue=""""/>
                          <FandV FieldName=""CONVERTTO"" FieldValue=""""/>
                          <FandV FieldName=""OutputPagePixWidth"" FieldValue=""""/>
                          <FandV FieldName=""PAGERANGE"" FieldValue=""""/>
                          <FandV FieldName=""IMPPREVIEW"" FieldValue=""""/>
                          <FandV FieldName=""SUPPRESSSETIMAGECROPPING"" FieldValue=""""/>
                      </Parameters>
                      <StaticFields>
                          <FandV FieldName=""01NameInPicture"" FieldValue=""'my first Document'""/>
                          <FandV FieldName=""StartMonth"" FieldValue=""'1'""/>
                          <FandV FieldName=""mYear"" FieldValue=""'2009'""/>
                          <FandV FieldName=""zHemisphere"" FieldValue=""'North'""/>
                          <FandV FieldName=""Claim"" FieldValue=""'Printing Emotions'""/>
                          <FandV FieldName=""Logo"" FieldValue=""'DirectSmile_4c.eps'""/>
                      </StaticFields>
                  </Command>";


            // Create SOAP Client
            lb.LBSoapClient lb = new lb.LBSoapClient("LBSoap",serviceUrl);

            // Authenticate
            dynamic sessionId = lb.Authenticate(username, password, string.Empty);

            // Returns Error or Session ID
            if (sessionId.StartsWith("ERR"))
            {
                Console.WriteLine("Please check credentials: " + sessionId);
                Console.ReadLine();
                return;
            }

            // Request document
            string startDocResult = lb.StartDoc(sessionId, "CalendarV3", xml.ToString());

            // Parse result into XElement
            XElement startDocResultXml = XElement.Parse(startDocResult);

            do
            {
                switch (startDocResultXml.Attribute("State").Value)
                {
                    case "2":
                        // Error
                        Console.WriteLine(startDocResultXml.Attribute("LastErrorMsg").Value);
                        Console.ReadLine();
                        return;
                    case "3":
                    case "4":
                        // Ready or Progress
                        string pdfUrl = startDocResultXml.Attribute("PDFUrl").Value;
                        if (!string.IsNullOrEmpty(pdfUrl) && pdfUrl.EndsWith(".pdf"))
                        {
                            // PDF generated
                            Console.WriteLine("PDF Url: " + pdfUrl);
                            Console.ReadLine();
                            return;
                        }
                        break;
                    default:
                        //Progress
                        Console.WriteLine(string.Format("Progress: {0} {1}%", startDocResultXml.Attribute("LastProgressMsg").Value, startDocResultXml.Attribute("LastProgressPercent").Value));
                        break;
                }

                System.Threading.Thread.Sleep(1000);

                // Request status
                startDocResult = lb.GetDocStatus(sessionId, startDocResultXml.Attribute("Key").Value);
                if(!string.IsNullOrEmpty(startDocResult))
                { 
                    startDocResultXml = XElement.Parse(startDocResult);
                }

            } while (true);
        }
    }
}
