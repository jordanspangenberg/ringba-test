using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ringba_test
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new WebClient();

            var filename = "output.txt";

            // Get the XML file if we don't have it. 
            getXML(client);
            // Get the test file at 'TestQuestions/output.txt'
            getTestFile(client, filename);

            int characterCount = 0;
            int uppercaseCount = 0; // Each uppercase letter indicates a new word
            var lowercaseLetters = new Dictionary<char, int>();
            var uppercaseLetters = new Dictionary<char, int>();
            var words = new Dictionary<string, int>();
            var prefixes = new Dictionary<string, int>();
            char ch;

            StringBuilder sb = new StringBuilder();

            if (File.Exists(Directory.GetCurrentDirectory() + $"\\{filename}"))
            {
                using (var streamReader = new StreamReader(Directory.GetCurrentDirectory() + $"\\{filename}"))
                {

                    while (streamReader.Peek() >= 0)
                    {

                        ch = (char)streamReader.Read();


                        if (Char.IsUpper(ch))
                        {
                            if (!uppercaseLetters.ContainsKey(ch))
                            {
                                uppercaseLetters.Add(ch, 1);
                            }
                            else
                            {
                                uppercaseLetters[ch] += 1;
                            }
                            uppercaseCount++; // increment our word count / uppercase count
                            // If we have encountered an uppercase character that means
                            // we need to deal with our words and prefixes. 
                            if (!sb.Equals(""))
                            {
                                string word = sb.ToString();
                                if (!words.ContainsKey(word))
                                {
                                    words.Add(word, 1);
                                }
                                else
                                {
                                    words[word] += 1;
                                }
                                if (word.Length > 2)
                                {
                                    string prefix = word.Substring(0, 2);
                                    if (!prefixes.ContainsKey(prefix))
                                    {
                                        prefixes.Add(prefix, 1);
                                    }
                                    else
                                    {
                                        prefixes[prefix] += 1;
                                    }
                                }
                            }
                            sb.Clear();
                        }
                        else
                        {
                            if (!lowercaseLetters.ContainsKey(ch))
                            {
                                lowercaseLetters.Add(ch, 1);
                            }
                            else
                            {
                                lowercaseLetters[ch] += 1;
                            }
                        }
                        characterCount++;
                        sb.Append(ch);
                    }
                }
            }

            foreach (var key in uppercaseLetters.Keys)
            {
                System.Console.WriteLine($"{key}: " + 
                    (uppercaseLetters[key] + 
                    lowercaseLetters[Char.ToLower(key)]));
            }

            System.Console.WriteLine("Word count: " + uppercaseCount);
            var keyOfMaxValueWords = words.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            System.Console.WriteLine(keyOfMaxValueWords + $": {words[keyOfMaxValueWords]}");
            var keyOfMaxValuePrefixes = prefixes.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            System.Console.WriteLine(keyOfMaxValuePrefixes + $": {prefixes[keyOfMaxValuePrefixes]}");
        }

        static void getTestFile(WebClient client, string filename)
        {

            if (!File.Exists(Directory.GetCurrentDirectory() + $"{filename}"))
            {
                using (client)
                {
                    client.DownloadFile($"https://ringba-test-html.s3-us-west-1.amazonaws.com/TestQuestions/{filename}", $"{filename}");
                }
            }
        }

        static void getXML(WebClient client)
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\test.xml"))
            {
                using (client)
                {
                    client.DownloadFile("https://ringba-test-html.s3-us-west-1.amazonaws.com/", "test.xml");
                }
            }
        }
    }

}
