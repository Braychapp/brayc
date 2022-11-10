using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace tasks_and_threads
{
    /*
   * NAME : FIO
   * PURPOSE : The FIO class is used for writing to the text file as well as the size monitor task
   * The class has 3 variables involved with it:
   * sizeReached: a bool used to tell if the maximum file size has been reached yet or not
   * writer: writer is used for actually writing to the file within the WriteFile method
   * mutex: used to queue all the tasks to take turns while working
   */
    internal class FIO
    {
        //this clas is going to be for writing to the file that is chosen by the user


        public volatile bool sizeReached = false;
        private StreamWriter writer = null;
        Mutex mutex = null;

        /*
        * FIO Default Constructor
        * Description: The default constructor for the FIO class, it gets the mutex ready to be used
        * Parameters: none
        * Returns: nothing
        */
        public FIO()
        {
            //if the mutex is somehow being used already it will release it
            if (!Mutex.TryOpenExisting("MyMutex", out mutex))
            {
                mutex = new Mutex(true, "MyMutex");
                mutex.ReleaseMutex();
            }
        }
        /*
       * Method: WriteFile
       * Description: This method uses all 25 tasks in the array and uses them to write guids to a file
       * Parameters:string FilePath
       * Returns: nothing
       */
        internal void WriteFile(string FilePath)
        {
            while (!sizeReached)
            {
                mutex.WaitOne();
                try
                {
                    using (writer = new StreamWriter(FilePath, true))
                    {
                        writer.WriteLine(Guid.NewGuid());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception: " + ex.Message);
                }
                finally
                {
                    if (writer != null)
                    {
                         writer.Close();
                    }
                }
                mutex.ReleaseMutex();
            }
        }

        /*
       * Method: SizeMonitor
       * Description: This method is used to keep track of how big the file is through the monitor task
       * Parameters:string FPath, int MaxSize, Task[] tasks
       * Returns: nothing
       */
        internal void SizeMonitor(string FPath, int MaxSize, Task[] tasks)
        {
            FileInfo fi = null;
            //creating a timer to go off every 0.5 seconds
            fi = new FileInfo(FPath);
            Stopwatch timer = new Stopwatch();

            while (!sizeReached)
            {
                //starting the 500 ms timer
                timer.Start();
                if (timer.ElapsedMilliseconds >= 500)
                {
                    sizeReached = CheckSize(FPath, MaxSize);
                    //i wanted to have a text box in my window that would automatically update to meet the file size but I spent 
                    //literally hours trying to do that and I couldn't so i am sorry but this is the only way I could get it to go :(
                    //sorry for message box spam
                    
                    timer.Restart();
                }

                if (sizeReached)
                {
                    foreach (Task task in tasks)
                    {
                        task.Wait();
                    }
                }
                Thread.Sleep(1);
                
            }
            fi = new FileInfo(FPath);
            MessageBox.Show("DONE!\nFinal File Size: " + fi.Length.ToString() + " bytes");
        }

        /*
       * Method: CheckSize
       * Description: This method is used for directly checking if the file's size is too big, and if it's not then the program will continue
       * Parameters:string FPath, int MaxSize
       * Returns: nothing
       */
        internal bool CheckSize(string FPath, int MaxSize)
        {
            
            FileInfo fi = null;
            fi = new FileInfo(FPath);

            if (fi.Length >= MaxSize)
            {
                return true;
            }
            else
            {
                MessageBox.Show("Current File Size: " + fi.Length.ToString() + " bytes");
                return false;
            }
            
        }

    }







    
}
