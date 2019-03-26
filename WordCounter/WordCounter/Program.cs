using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WordCounter
{
    class Program
    {
        public static string userInput = "";

        static void Main(string[] args)
        {
            while (userInput != "q")
            {
                Console.WriteLine("\r\nText file top 10 words\r\n");

                //Output the word count for a given file.
                OutputWordCount(10);

                // See if the user wants to try again
                if (userInput != "q")
                {
                    Console.WriteLine("\r\nPress q to exit or any other key run the application again.");
                    userInput = Console.ReadLine();
                }
            }
        }

        /// <summary>
        /// Output the word count to the console for a provided file
        /// </summary>
        /// <param name="resultLimit"></param>
        public static void OutputWordCount(int resultLimit)
        {
            StreamReader sr = null;

            Console.WriteLine("   Enter the file location and name - e.g c:/test/filename.txt");
            Console.WriteLine("   or enter the URL of the file - e.g http://www.test.com/filename.txt");
            Console.WriteLine("\r\n   Press q to quit the program");

            // Loop until we have a valid file in memory or the user exits the application
            while (sr == null && userInput != "q")
            {
                Console.Write("\r\nLocation: ");
                userInput = Console.ReadLine();

                if (userInput != "q")
                {
                    // try and load the file into memory
                    sr = ReadFileToStream(userInput);

                    // if we don't have a valid file to process, ask the user to try again
                    if (sr == null)
                        Console.WriteLine("Please try again");
                }
            }

            if (sr != null)
            {
                // Build a dictionary of words and their occurence in the document
                var dict = CountWordsInDocument(sr);

                // Limit the results to the top x results
                var ordered = dict.OrderByDescending(a => a.Value).Take(resultLimit);

                // Output the results to the user
                Console.WriteLine("\r\nTop ten words\r\n");

                foreach (var keyValPair in ordered)
                    Console.WriteLine(String.Format("{0} - {1}", keyValPair.Key, keyValPair.Value));
            }
        }

        /// <summary>
        /// Read a file to stream from a given folder location or url
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public static StreamReader ReadFileToStream(string fileLocation)
        {
            try
            {
                // Test whether this is a url and return stream
                Uri uriResult;
                if(Uri.TryCreate(fileLocation, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                { 
                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead(fileLocation);
                    StreamReader reader = new StreamReader(stream);

                    if (reader != null)
                        return reader;
                }
                // otherwise test if this is a valid url
                else if (fileLocation.IndexOfAny(Path.GetInvalidPathChars()) == -1)
                {
                    StreamReader sr = new StreamReader(fileLocation);

                    if (sr != null)
                        return sr;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("URL or File location was not found or is in an incorrect format");
            }

            return null;
        }

        /// <summary>
        /// Returns a dictionary of words and their count from a document stream
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static Dictionary<string, int> CountWordsInDocument(StreamReader sr)
        {
            var output = new Dictionary<string, int>();

            if (sr != null)
            {
                // declare delimiters we can use to determine what separates one word from another
                string delim = " ,.\"'�";

                while (!sr.EndOfStream)
                { 
                    // Read the document line by line and trim any whitespace
                    string line = sr.ReadLine();
                    line.Trim();

                    // break the line down to a list of words using the delimeters
                    string[] fields = line.Split(delim.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    // Loop the words and add them to the dictionary, checking whether they already exist.
                    foreach (string word in fields)
                    {
                        if (output.ContainsKey(word))
                            output[word]++;
                        else
                            output[word] = 1;
                    }
                }

                return output;                
            }

            return null;
        }
    }
}
