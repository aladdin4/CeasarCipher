using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace CeasarCipherWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
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

                MessageBox.Show($"{selectedOption}", "1");
            }

            if (selectedOption == "Encrypt from a file" || selectedOption == "Encrypt from manual text")
            {

                EncryptPathway(selectedOption);
                //hide confirm and show reset button
                //TBD
                                    
                //hide the dropdown
                FunctionalitySelector.Visibility = Visibility.Collapsed;
                MessageBox.Show($"{selectedOption}", "2");
            }

            if (selectedOption == "Decrypt from a file" || selectedOption == "Decrypt from manual text")
            {

                DecryptPathway(selectedOption);
                //hide confirm and show reset button
                //TBD

                //hide the dropdown
                FunctionalitySelector.Visibility = Visibility.Collapsed;
                MessageBox.Show($"{selectedOption}", "3");
            }



           

            //// Make the second dropdown visible
            //OptionsDropdown.Visibility = Visibility.Visible;

            //hide the first dropdown
            //
        }
    }
    private void EncryptPathway(string selectedOption)
    {

    }
    private void DecryptPathway(string selectedOption)
    {

    }

        //if (OptionsDropdown.SelectedItem is ComboBoxItem subOption)
        //{
        //    string selectedSubOption = subOption.Content.ToString();
        //    if (selectedSubOption == "Encrypt from a file" || selectedSubOption == "Decrypt from a file")
        //    {
        //        ProcessFile();
        //    }
        //    else
        //    {
        //        MessageBox.Show($"You selected: {selectedSubOption}", "Selection");
        //    }

        //    //hide the confirm button
        //   // ConfirmButton.Visibility = Visibility.Collapsed;
        //}
        
   

  

    private void ProcessFile()
    {
        string filePath = PickFile();

        if (!string.IsNullOrEmpty(filePath))
        {
            // Read the file content
            string fileContent = File.ReadAllText(filePath);

            // Perform encryption or decryption
            MessageBox.Show($"File Content: {fileContent}", "File Loaded");

            // Add your logic here to encrypt or decrypt the file content
        }
        else
        {
            MessageBox.Show("No file was selected.", "Error");
            //show  the confirm button
            ConfirmButton.Visibility = Visibility.Visible;
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


}