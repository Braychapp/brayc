/*
* FILE : MainWindow.xaml.cs
* PROJECT : A04 tasks and threads
* PROGRAMMER : Brayden Chapple
* FIRST VERSION : 2022-11-09
* DESCRIPTION : This program writes guids into a text file up to the specified size by the user and lets them know the size of the file as it runs
* through message boxes
*/
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace tasks_and_threads
{
    /*
    * NAME : MainWindow
    * PURPOSE : The MainWindow class is for displaying the main window to the user and allowing them to interact
    * with the features associated with the main window
    * The class has 4 variables involved with it:
    * tasks: an array of tasks for use in writing the file full of guids
    * Monitor: a lone task used to monitor the size of the file every 0.5 seconds or so
    * file: an object of type FIO which is used for altering the text file
    * taskNumbers: used to tell the program how many tasks to use in the array
    */
    public partial class MainWindow : Window
    {
        //creating tasks
        Task[] tasks;
        Task Monitor;
        FIO file = new FIO();
        int taskNumbers = 25;

        /*
        * MainWindow Default Constructor
        * Description: The default constructor for the MainWindow class, it does not assign any variables
        * Parameters: none
        * Returns: nothing
        */
        public MainWindow()
        {
            InitializeComponent();
        }

        /*
       * Method: FileIo_Click
       * Description: This method is used to open up a text file of the user's choosing
       * Parameters:object sender, RoutedEventArgs e
       * Returns: nothing
       */
        private void FileIO_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            {
                Title = "Open File";
            }
            saveFileDialog.Filter = "Text files (.*txt)|*.txt|All files(*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                //needs to change the filepath and be able to transport that variable elsewhere
                var path = string.Empty;
                path = saveFileDialog.FileName;
                FilePath.Text = path;


            }
        }

        /*
       * Method: TextBox_TextChanged
       * Description: This method does nothing
       * Parameters:object sender, TextChangedEventArgs e
       * Returns: nothing
       */
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        /*
       * Method: GenerateFile_Click
       * Description: This method is used for input validation, as well as turning on all the tasks to start writing to the file
       * Parameters:object sender, RoutedEventArgs e
       * Returns: nothing
       */
        private void GenerateFile_Click(object sender, RoutedEventArgs e)
        {
            //validate that a file was selected before doing anything
            string fPath = FilePath.Text;
            if (fPath == "File Path:")
            {
               MessageBox.Show("Please select a file before attempting to generate file", "File error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //next validation is the file's target size
            int Target = Convert.ToInt32(FileSize.Text);
            if (1000 > Target)
            {
                MessageBox.Show("Please input a size between 1,000 and 20,000,000", "Size error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(Target >= 20000000)
            {
                MessageBox.Show("Please input a size between 1,000 and 20,000,000", "Size error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //make sure the file is empty at this point 
            using (var fs = new FileStream(fPath, FileMode.Truncate))
            {
                fs.Close();
            }

                tasks = new Task[taskNumbers];

            file.sizeReached = false;
            Monitor = new Task(() => file.SizeMonitor(fPath, Target, tasks));
            Monitor.Start();
            for(int i = 0; i < taskNumbers; i++)
            {
                tasks[i] = new Task(() => file.WriteFile(fPath));
                tasks[i].Start();
            }
            
            
            

        }

        
    }
}
