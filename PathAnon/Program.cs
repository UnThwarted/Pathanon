using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PathAnon
{
    class Program
    {
        private static int exitCode;
        static int Main(string[] args)
        {
            // Parameters
            // FilePath - path of xml file to be anonymised
            // xpath location of Tag or Attribute to be replaced
            // path of text file containing your replacement text (optional)
            exitCode = 0;
            int returnCode = 0;
            string filePath, tagPath, rhubarbPath = string.Empty;
            string[] rhubarb;
            if (args.Length == 0)
            {
                //Console.WriteLine("please enter the filepath to anonymise, and the Tag in xpath notation");
                filePath = "Test.xml";
                tagPath = "name";
                rhubarbPath = string.Empty;
            }
            else
            {
                filePath = args[0];
                tagPath = args[1];
                rhubarbPath = args[2];
            }
            Console.WriteLine("Before file : " + filePath.ToString() + "\n");
            Console.WriteLine("xpath : " + tagPath.ToString() + "\n");
            if (rhubarbPath.Length > 0)
            {
                Console.WriteLine("xpath : " + rhubarbPath.ToString() + "\n");
                rhubarb = File.ReadAllLines(rhubarbPath.ToString());
            }
            else
            {
                if (tagPath == "name")
                {
                    rhubarb = File.ReadAllLines(@"lipsumNames.txt");
                }
                else
                {
                    rhubarb = File.ReadAllLines(@"lipsumList.txt");
                }
            }

            var xDoc = System.Xml.Linq.XDocument.Load(filePath);
            int anonNameCount = 0;

            anonNameCount = PathAnon(xDoc, filePath, tagPath, rhubarb);

            switch (Program.exitCode)
            {
                case 0:
                    Console.WriteLine("\n\nreturn code : " + exitCode + " : " +
                                        "finished with no errors\n");
                    break;
                case 1:
                    Console.WriteLine("\n\nreturn code : " + exitCode + " : " +
                                        "invalid parameters - please correct parameters and retry\n");
                    break;
                case 99:                    Console.WriteLine("\n\nreturn code : " + exitCode +
                                        "severe error detected - please check format of xml file, xpath (and own csv if using own nonsense file)\n");
                    break;
                default:
                    Console.WriteLine("\n\nreturn code : " + exitCode + " : " + Math.Abs(exitCode) +
                                        "validation {0}", (exitCode < -1 ? "issues " : "issue ") +  "detected\n");
                    break;
            }
            string resultsPath = filePath.Substring(0, filePath.Length - 4) + "_results" + ".xml";
            Console.WriteLine("{0} replacements were made, check the results file: {1}", anonNameCount, resultsPath);
            xDoc.Save(resultsPath);
            Console.WriteLine("\nHit spacebar to continue");
            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey();
            } while ((cki.Key != ConsoleKey.Escape) && (cki.Key != ConsoleKey.Spacebar));

        returnCode = Program.exitCode;
            return returnCode;
        }
        private static int PathAnon(XDocument xDoc, string filePath, string tagPath, string [] rhubarb)
        {
            int anonNameCount = 0;
            // replace
            try
            {
                foreach (System.Xml.Linq.XElement xe in xDoc.Descendants("drop"))
                {
                    Console.WriteLine("To change: {0} : {1} : {2}", anonNameCount,
                        xe.Element(tagPath).Value, rhubarb[anonNameCount]);
                    xe.Element(tagPath).Value = rhubarb[anonNameCount];
                    anonNameCount++;
                }
            }
            catch (Exception)
            {
                exitCode = -1;
                Console.WriteLine("ErrorEventArgs when attempting to replace text ({0})", Program.exitCode);
            }
            return anonNameCount;

        }
    }
}
