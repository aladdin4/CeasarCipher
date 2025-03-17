using System.IO;
using System.Windows;
using System.Windows.Controls;
using MathNet.Numerics.Statistics;
using Microsoft.Win32;

namespace CeasarCipherWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    string state;
    public MainWindow()
    {
        InitializeComponent();
    }
    private async void Confirm_Click(object sender, RoutedEventArgs e)
    {
        

        if (FunctionalitySelector.SelectedItem is ComboBoxItem selectedItem)
        {
            string selectedOption = selectedItem.Content.ToString();

            if (selectedOption == "Encrypt Text" || selectedOption == "Decrypt Text")
            {

                LoadingBar.Visibility = Visibility.Visible;        // Show the ProgressBar
                await Task.Delay(300);               // Simulate a loading delay of 0.3 seconds (500 milliseconds)
                LoadingBar.Visibility = Visibility.Collapsed;      // Hide the ProgressBar after the delay

                FunctionalitySelector.Items.Clear();

                if (selectedOption == "Encrypt Text")
                {
                    FunctionalitySelector.Items.Add(new ComboBoxItem { Content = "Encrypt from a file", IsSelected = true });
                    FunctionalitySelector.Items.Add(new ComboBoxItem { Content = "Encrypt from manual text" });
                }

                if (selectedOption == "Decrypt Text")
                {
                    FunctionalitySelector.Items.Add(new ComboBoxItem { Content = "Decrypt from a file", IsSelected = true });
                    FunctionalitySelector.Items.Add(new ComboBoxItem { Content = "Decrypt from manual text" });
                }

            }

            if (selectedOption == "Encrypt from a file" || selectedOption == "Encrypt from manual text")
            {
                UIProcessing(false);

                if (selectedOption == "Encrypt from a file")
                {
                    ProcessFile(false);
                }
            }

            if (selectedOption == "Decrypt from a file" || selectedOption == "Decrypt from manual text")
            {
                UIProcessing(true);
                

                if (selectedOption == "Decrypt from a file")
                {
                    ProcessFile(true);
                }

                //hide the dropdown
                FunctionalitySelector.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void UIProcessing(bool isDecryptPathway)
    {
        state = isDecryptPathway ? "Decrypt" : "Encrypt";

        //1) Change please select the required functionality and hide the confirm button
        FunctionalityText.Text = $"Please enter the text to {state}";
        ConfirmButton.Visibility = Visibility.Collapsed;

        //2) Show the reset button
        ResetButton.Visibility = Visibility.Visible;

        //3) Hide the dropdown
        FunctionalitySelector.Visibility = Visibility.Collapsed;

        //4) Show the text box and the action button and edit it's content
        ManualTextInput.Visibility = Visibility.Visible;
        ActionButton.Visibility = Visibility.Visible;
        ActionButton.Content = state + " Text";

        if (!isDecryptPathway)
        {
            KeyGrid.Visibility = Visibility.Visible;
        }
    }


    private void ProcessFile(bool isDecryptPathway)
    {
        state = isDecryptPathway ? "Decrypt" : "Encrypt";
        ConfirmButton.Content = "Pick File";
        string filePath = PickFile();

        if (!string.IsNullOrEmpty(filePath))
        {
            // Read the file content
            string fileContent = File.ReadAllText(filePath);
            ManualTextInput.Text = fileContent;
        }
    }

    private string PickFile()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Select a File",
            Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        };

        // Show the dialog and check if the user selected a file
        if (openFileDialog.ShowDialog() == true)
        {
            return openFileDialog.FileName; // Return the full file path
        }

        return null; // Return null if the user cancels
    }

    private void ActionButton_Click(object sender, RoutedEventArgs e)
    {
        string inputValue = ManualTextInput.Text;
        string key = KeyTextBox.Text;

        if (state == "Encrypt")
        {
            ResultTextBox.Text = "Result: " +  Encrypt(inputValue, Convert.ToInt32(key));
            ResultTextBox.Visibility = Visibility.Visible;
        }
        else
        {
            DecryptText(inputValue);
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

    private void DecryptText(string encryptedText)
    {
        // Convert frequencies to arrays
        var englishFreqArray = englishFrequencies.Values.ToArray();

        //To make a list of each shift, from 1-26.
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

        ResultTextBox.Text = "Result: " + Decrypt(encryptedText, maxCorrelationIndex);
        ResultTextBox.Visibility = Visibility.Visible;
        ResultKeyTextBox.Text = "Key (Shift): " + maxCorrelationIndex;
        ResultKeyTextBox.Visibility = Visibility.Visible;
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
        var sortedEncryptedTextFrequencies = encryptedTextFrequencies
            .OrderBy(kvp => kvp.Key)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        return sortedEncryptedTextFrequencies;
    }
   

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        FunctionalitySelector.SelectedIndex = 0;
        FunctionalityText.Text = "Please Select The Required Functionality:";
        ResetButton.Visibility = Visibility.Collapsed;
        ManualTextInput.Visibility = Visibility.Collapsed;
        ManualTextInput.Text = "";
        ActionButton.Visibility = Visibility.Collapsed;
        ConfirmButton.Visibility = Visibility.Visible;
        ConfirmButton.Content = "Confirm";
        FunctionalitySelector.Visibility = Visibility.Visible;
        FunctionalitySelector.Items.Clear();
        FunctionalitySelector.Items.Add(new ComboBoxItem { Content = "Encrypt Text", IsSelected = true });
        FunctionalitySelector.Items.Add(new ComboBoxItem { Content = "Decrypt Text" });
        KeyTextBox.Text = "3";
        KeyGrid.Visibility = Visibility.Collapsed;
        ResultTextBox.Visibility = Visibility.Collapsed;
        ResultTextBox.Text = "";
        ResultKeyTextBox.Visibility = Visibility.Collapsed;
        ResultKeyTextBox.Text = "";
    }
}