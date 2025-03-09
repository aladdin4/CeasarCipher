using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using static System.Net.Mime.MediaTypeNames;
namespace CeasarCipher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                WriteLineColored("igsr", ConsoleColor.Yellow);
                Console.Write("Name: ");
                WriteLineColored("Ahmed Aladdin", ConsoleColor.Green);
                Console.Write("ID: ");
                WriteLineColored("1404-5-034", ConsoleColor.Green);
                Console.WriteLine();

                WriteLineColored("Welcome To Caesars Cipher Program", ConsoleColor.Cyan);
                WriteLineColored("*******************************", ConsoleColor.Cyan);
                Console.WriteLine("To Decrypt A Text, Please Press 1");
                Console.WriteLine("To Encrypt A Text, Please Press 2");
                int decryptEncrypt = Convert.ToInt32(Console.ReadLine());
                if (decryptEncrypt == 1)  //decrypt path
                {
                    DecryptPath();
                }
                else if (decryptEncrypt == 2)  //encrypt path
                {
                    EncryptPath();
                }
                else
                {
                    WriteLineColored("Invalid Input", ConsoleColor.Red);
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
            }
        }




        private static void WriteLineColored(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
        //1) Decryption
        private static void DecryptPath()
        {
            Console.WriteLine("To Enter The Text Manually, Please Press 1");
            Console.WriteLine("To Enter The Text From a File, Please Press 2");
            int manualFromFile = Convert.ToInt32(Console.ReadLine());
            if (manualFromFile == 1)  //manual path
            {
                Console.WriteLine("Enter the text to decrypt: ");
                string text = Console.ReadLine();
                if (string.IsNullOrEmpty(text))
                {
                    WriteLineColored("Invalid input. Please enter a valid text.", ConsoleColor.Red);
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
                else
                {
                    DecryptText(text);
                }
            }
            else if (manualFromFile == 2)  //from a file
            {
                string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.txt");
                if (files.Length == 0)
                {
                    WriteLineColored("No text files found in the current directory.", ConsoleColor.Red);
                    return;
                }

                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {Path.GetFileName(files[i])}");
                }

                Console.WriteLine("Enter the number of the file you want to select:");
                int fileChoice = Convert.ToInt32(Console.ReadLine()) - 1;
                if (fileChoice >= 0 && fileChoice < files.Length)
                {
                    string filePath = files[fileChoice];
                    string text = File.ReadAllText(filePath);
                    if (string.IsNullOrEmpty(text))
                    {
                        WriteLineColored("Invalid input. Please enter a valid text.", ConsoleColor.Red);
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                    }
                    else
                    {
                        DecryptText(text);
                    }
                }
                else
                {
                    WriteLineColored("Invalid choice. Please try again.", ConsoleColor.Red);
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
            }
            else
            {
                WriteLineColored("Invalid choice", ConsoleColor.Red);
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }





        private static void DecryptText(string encryptedText)
        {
            // Convert frequencies to arrays
            var englishFreqArray = englishFrequencies.Values.ToArray();

            //we need to make a list of each shift, where 0 is the original one, and so on.
            var shifts = Enumerable.Range(1, 26).ToList();
            var correlationValues = new double[26];
            foreach (int shift in shifts)
            {
                var text = Decrypt(encryptedText, shift);
                var frequencies = CalculateLetterFrequencies(text);
                correlationValues[shift - 1] = Correlation.Pearson(englishFreqArray, frequencies.Values.ToArray());
            }
            //get the largest correlation value
            var maxCorrelation = correlationValues.Max();

            //get the index of the largest correlation value
            var maxCorrelationIndex = correlationValues.ToList().IndexOf(maxCorrelation) + 1;

            //display the results
            Console.Write("The text was encrypted using a Caesar Cipher with a shift of ");
            WriteLineColored($"(key): {maxCorrelationIndex}", ConsoleColor.Yellow);
            Console.Write("The decrypted text is: ");
            WriteLineColored($"{Decrypt(encryptedText, maxCorrelationIndex)}", ConsoleColor.Green);
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
        private static string Decrypt(string text, int key)
        {
            string decryptedText = string.Empty;
            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    //to start from A and avoid special characters
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    char decryptedChar = (char)((c - key - offset + 26) % 26 + offset);
                    decryptedText += decryptedChar;
                }
                else
                {
                    decryptedText += c;
                }
            }
            return decryptedText;
        }
        private static Dictionary<char, double> englishFrequencies = new Dictionary<char, double>
        {
                {'A', 7.487792},
                {'B', 1.295442},
                {'C', 3.544945},
                {'D', 3.621812},
                {'E', 13.99891},
                {'F', 2.183939},
                {'G', 1.73856},
                {'H', 4.225448},
                {'I', 6.653554},
                {'J', 0.269036},
                {'K', 0.465726},
                {'L', 3.569814},
                {'M', 3.39121},
                {'N', 6.741725},
                {'O', 7.372491},
                {'P', 2.428106},
                {'Q', 0.262254},
                {'R', 6.140351},
                {'S', 6.945198},
                {'T', 9.852595},
                {'U', 3.004612},
                {'V', 1.157533},
                {'W', 1.691083},
                {'X', 0.278079},
                {'Y',1.643606},
                {'Z',0.036173}
        };
        static Dictionary<char, double> CalculateLetterFrequencies(string text)
        {
            var encryptedTextFrequencies = new Dictionary<char, double>();

            int totalLetters = 0;
            foreach (char c in text.ToUpper())
            {
                if (char.IsLetter(c))
                {
                    if (encryptedTextFrequencies.ContainsKey(c))
                    {
                        encryptedTextFrequencies[c]++;
                    }
                    else
                    {
                        encryptedTextFrequencies[c] = 1;
                    }
                    totalLetters++;
                }
            }

            // Convert counts to frequencies (percentages)
            foreach (var key in encryptedTextFrequencies.Keys.ToList())
            {
                encryptedTextFrequencies[key] = (encryptedTextFrequencies[key] / totalLetters) * 100;
            }

            // Ensure all letters are present in the frequency dictionary
            for (char c = 'A'; c <= 'Z'; c++)
            {
                if (!encryptedTextFrequencies.ContainsKey(c))
                {
                    encryptedTextFrequencies[c] = 0;
                }
            }
            var sortedEncryptedTextFrequencies = encryptedTextFrequencies.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return sortedEncryptedTextFrequencies;
        }

        //2) Encryption
        private static void EncryptPath()
        {
            Console.WriteLine("To Enter The Text Manually, Please Press 1");
            Console.WriteLine("To Enter The Text From a File, Please Press 2");
            int manualFromFile = Convert.ToInt32(Console.ReadLine());
            if (manualFromFile == 1)  //manual path
            {
                Console.WriteLine("Enter the text to encrypt: ");
                string text = Console.ReadLine();
                Console.WriteLine("Enter the key: ");
                int key = Convert.ToInt32(Console.ReadLine());
                string encryptedText = Encrypt(text, key);
                Console.WriteLine("Encrypted text: " + encryptedText);
            }
            else if (manualFromFile == 2)  //from a file
            {
                string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.txt");
                if (files.Length == 0)
                {
                    Console.WriteLine("No text files found in the current directory.");
                    return;
                }

                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {Path.GetFileName(files[i])}");
                }

                Console.WriteLine("Enter the number of the file you want to select:");
                int fileChoice = Convert.ToInt32(Console.ReadLine()) - 1;
                if (fileChoice >= 0 && fileChoice < files.Length)
                {
                    string filePath = files[fileChoice];
                    string text = File.ReadAllText(filePath);
                    Console.WriteLine("Enter the key: ");
                    int key = Convert.ToInt32(Console.ReadLine());
                    string encryptedText = Encrypt(text, key);
                    Console.WriteLine("Encrypted text: " + encryptedText);
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid choice");
            }
        }
        private static string Encrypt(string text, int key)
        {
            string encryptedText = string.Empty;
            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    //to start from A and avoid special characters
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    char encryptedChar = (char)((c + key - offset) % 26 + offset);
                    encryptedText += encryptedChar;
                }
                else
                {
                    encryptedText += c;
                }
            }
            return encryptedText;
        }
    }
}
