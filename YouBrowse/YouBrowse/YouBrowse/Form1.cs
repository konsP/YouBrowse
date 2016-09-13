using System;
using System.Collections.Generic;
using System.ComponentModel;//
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Drawing.Printing;
using System.Threading;

namespace YouBrowse
{
    public partial class Form1 : Form
    {
        public string homeurl, newName, newName2, url;
        private Font printFont;
        public StreamReader draft ;

        public Form1()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
        }

        //Loads Form1
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists("home.txt"))
            {
            File.WriteAllText("home.txt", String.Empty);
            }
            if (!File.Exists("history.txt"))
            {
                File.WriteAllText("history.txt", String.Empty);
            }
            if (!File.Exists("favnames.txt"))
            {
                File.WriteAllText("favnames.txt", String.Empty);
            }
            if (!File.Exists("favlinks.txt"))
            {
                File.WriteAllText("favlinks.txt", String.Empty);
            }
            {
                //Loads the homepage on start up. If there is no homepage set, then notifies the user with a popup message.
                homeurl = loadHome("home.txt");
                if (homeurl == "")
                {
                    noHomepageLabel.Visible = true;
                    // Adds the event and the event handler for the TimerEventProcessor method.
                    timer1.Tick += new EventHandler(TimerEventProcessor);
                    // Sets the timer interval to 4 seconds and starts the timer.
                    timer1.Interval = 4000;
                    timer1.Start();

                }
                else
                {
                    //Sets urlBox text to homeurl.
                    urlBox.Text = homeurl;
                    //Calls the mayhod to make the request and enable/disable the buttons needed.
                    doRequest(homeurl);

                }
                //loads the history and favorites lists from the specified files.
                loadHistory("history.txt");
                loadFavorites("favlinks.txt", "favnames.txt");

            }

        }

        //Timer//
        //Controls the noHomepageLabel visibility when interval time elapses.
        private void timer1_Tick(object sender, EventArgs e)
        {
            noHomepageLabel.Visible = false;
        }

        //Starts a new  instance of timer1 and passes the control to the timer1_Tick method when interval time elapses.
        private static void TimerEventProcessor(Object Object, EventArgs EventArgs)
        {
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            timer1.Enabled = true;
            timer1.Stop();

        }

        //Controls the cancelLabel visibility when interval time elapses.
        private void timer2_Tick(object sender, EventArgs e)
        {
            cancelLabel.Visible = false;
        }

        //Starts a new  instance of timer2 and passes the control to the timer2_Tick method when interval time elapses.
        private static void TimerEventProcessor1(Object Object, EventArgs EventArgs)
        {
            System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
            timer2.Enabled = true;
            timer2.Stop();

        }


        //Background Worker//
        //Initializes the Background Worker.
        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
        }

        //Implements the HTTP request.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // The result of the request (string) is assigned to the Result property of the DoWorkEventArgs  object.
            Request r = new Request((string)e.Argument);
            e.Result = r.getPage((string)e.Argument, worker, e);// url)

        }

        //Handles the event where the Background Worker completes.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            {
                //  Case where an exception is thrown. 
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message);
                }
                else if (e.Cancelled)
                {
                    // Case where the user cancels the operation.
                    cancelLabel.Visible = true;
                    //Empties the main textbox.
                    richtextBox1.Text = String.Empty;

                }
                else
                {
                    // Case where the operation succeeds to give back a result.

                    richtextBox1.Text = e.Result.ToString();
                }

                // Enables the go button.
                goButton.Enabled = true;

                // Enables the home button.
                homeButton.Enabled = true;

                //Enables the urlBox.
                urlBox.Enabled = true;

                // Disable the cancel button.
                cancelButton.Enabled = false;
            }
        }

      


        //Menu bar//

        //File menu //

        //New Window//
        private void newWindowToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Calls the method newWindowToolStripMenuItem_Click in the Tools menu to start a new thread.
            newWindowToolStripMenuItem_Click(sender, e);
        }
        //Save page//
        private void savePageToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Starts a dialog box to save the current page.
            saveFileDialog1.ShowDialog();
        }
        //Save dialog box//
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            // Gets the file name.
            string name = saveFileDialog1.FileName;
            //Writes all the text from richtextBox1 to the file
            File.WriteAllText(name, richtextBox1.Text);
        }
        //Print//
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Calls the printButton_Click method to print the page.
            printButton_Click(sender, e);
        }
        //Exit//
        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Calls the exitToolStripMenuItem_Click to exit the application
            exitToolStripMenuItem_Click(sender, e);
        }


        //History menu//

        //Show History//
        private void showHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Calls the historyButton_Click method
            historyButton_Click(sender, e);
        }


        //Favorites menu//

        //Show favorites/
        private void showFavoritesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Controls the visibility of the Favorites groupbox and all its contents.
            //Also accessible through "Ctrl+B" shortcut.
            if (groupBox1.Visible == false)
            {
                groupBox1.Visible = true;
                listBox1.Visible = true;
            }
            else
            {
                groupBox1.Visible = false;
                listBox1.Visible = false;
            }

        }

        //GroupBox1 Controls//

        //Checks the number of elements in the favorites list and enables/disables the remove button.
        private void groupBox1_Enter(object sender, EventArgs e)
        {
            //Checks if the favorites list is empty else enables the Remove button.
            if (listBox1.Items.Count == 0)
            {
                removeButton.Enabled = false;
            }
        }

        //Controls the double click on an item in the list and make an HTTP request. 
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //Saves the index and the value of the selected item to two variables.
            int index = listBox1.SelectedIndex;
            string selected = (string)listBox1.SelectedItem;
            //Checks if the selected item is a url or a name.
            if (selected.StartsWith("http://www."))
            {
                //Calls the method to make the request and enable/disable the buttons needed.
                doRequest(selected);

                //Writes the  history list in the history file.
                History h = new History(selected);
                h.appendHistory(h.getUrl(), "history.txt");
            }
            else //Case where the selected item is a name.
            {
                string link = null;
                //Opens the favlinks file and reads it till the selected index is reached.
                using (StreamReader reader = new StreamReader("favlinks.txt"))
                {
                    for (int i = 0; i <= index; ++i)
                    {
                        link = reader.ReadLine();
                    }
                    //Closes the file.
                    reader.Close();

                }
                //Calls the method to make the request and enable/disable the buttons needed.
                doRequest(link);
                //Writes the history list in the history file.
                History h = new History(link);
                h.appendHistory(link, "history.txt");
            }
        }//////////////////////////////////

        //Exits the favorites list.
        private void OKButton_Click(object sender, EventArgs e)
        {
            //Sets groupbox1 to invisible.
            if (groupBox1.Visible)
            {
                groupBox1.Visible = false;
                modifytextBox.Visible = false;
                modifyfavoriteLabel.Visible = false;
            }
        }

        //Removes a selected item from the favorites list.
        private void removeButton_Click(object sender, EventArgs e)
        {
            try
            {   //Saves the index and the value of the selected item to two variables.
                int selectedIndex = listBox1.SelectedIndex;
                listBox1.Items.RemoveAt(selectedIndex);


                //Copies the contents of favlinks.txt and favnames.txt
                List<string> linkslist = File.ReadAllLines("favlinks.txt").ToList();
                List<string> nameslist = File.ReadAllLines("favnames.txt").ToList();
                //Removes the selected item from both lists.
                linkslist.RemoveAt(selectedIndex);
                nameslist.RemoveAt(selectedIndex);
                //Rewrites the contents of the lists in the files.
                File.WriteAllLines("favlinks.txt", linkslist.ToArray());
                File.WriteAllLines("favnames.txt", nameslist.ToArray());



            }
            catch (ArgumentOutOfRangeException ae)
            {
                groupBox1.Visible = false;
                modifytextBox.Visible = false;
                modifyfavoriteLabel.Visible = false;
            }
        }

        //Modifies a selected item from the favorites list.
        private void modifyButton_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                modifyfavoriteLabel.Visible = true;
                modifytextBox.Visible = true;
            }
            else
            {
                groupBox1.Visible = false;
                modifytextBox.Visible = false;
                modifyfavoriteLabel.Visible = false;
            }
        }

        //Secondary textbox for the user to give a name to modify a favorite.
        private void modifytextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //Checks if "Enter" is pressed from the keyboard.
                if (e.KeyData == Keys.Enter)
                {
                    //Assignes the current url in a variable.
                    newName2 = modifytextBox.Text;
                    modifytextBox.Clear();
                    int selectedIndex = listBox1.SelectedIndex;
                    
                    //Copies the contents of favnames.txt in a lstring array.
                    string[] names = System.IO.File.ReadAllLines("favnames.txt");
                    //Adds the name in the modifytextBox in the selected position in the array.
                    names[selectedIndex] = newName2;
                    //Clears the favorites listbox.
                    listBox1.Items.Clear();
                    //Rewrites the contents of the array to the file.
                    System.IO.File.WriteAllLines("favnames.txt", names);
                    //Loads the favorites list again.
                    loadFavorites("favlinks.txt", "favnames.txt");
                    modifytextBox.Visible = false;
                    modifyfavoriteLabel.Visible = false;
                }

            }
            catch (Exception exc)
            {
                groupBox1.Visible = false;
                modifytextBox.Visible = false;
                modifyfavoriteLabel.Visible = false;
            }

        }

        //Home menu//

        //Go to homepage//
        private void goToHomepageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Calls the homeButton_Click method.
            homeButton_Click(sender, e);
        }



        //--Toolstrip Items--//

        //Urlbox//
        //Controls the KeyDown event in the Urlbox.
        private void urlBox_KeyDown(object sender, KeyEventArgs e)
        {
            //Case where "Enter" is pressed from the keyboard.
            if (e.KeyData == Keys.Enter)
            {
                //Calls the mayhod to make the request and enable/disable the buttons needed.
                doRequest(urlBox.Text);

                //Writes the history list in the history file.
                History h = new History(urlBox.Text);
                h.appendHistory(h.getUrl(), "history.txt");

            }
        }
        

        //Buttons//

        // History button//
        private void historyButton_Click(object sender, EventArgs e)
        {
            //Checks whether the history groupbox is visible to the user.
            if (groupBox2.Visible == false)
            {
                //Sets the history groupbox and listbox to visible.
                groupBox2.Visible = true;
                listBox2.Visible = true;
            }
            else
            {
                //Sets the history groupbox and listbox to invisible.
                groupBox2.Visible = false;
                listBox2.Visible = false;
                
            }
        }

        //Controls the Mouse Double Click on a history list item.
        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //Saves the index and the value of the selected item to two variables.
            int index = listBox2.SelectedIndex;
            string selected = (string)listBox2.SelectedItem;
           
            //Calls the method to make the request and enable/disable the buttons needed.
             doRequest(selected);
           
           //Writes the history list in the history file.
           History h = new History(selected);
           h.appendHistory(h.getUrl(), "history.txt");

        }

        

        //Print button//
   
        private void printButton_Click(object sender, EventArgs e)
        {
           savePage();
            try
            {
                draft = new StreamReader("text.txt");
                try
                {
                    printFont = new Font("Arial", 10);
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += new PrintPageEventHandler
                       (this.printDocument1_PrintPage);
                    pd.Print();
                }
                finally
                {
                    draft.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e) 
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            string line = null;

            // Calculate the number of lines per page.
            linesPerPage = e.MarginBounds.Height /
               printFont.GetHeight(e.Graphics);

            // Print each line of the file.
            while (count < linesPerPage &&
               ((line = draft.ReadLine()) != null))
            {
                yPos = topMargin + (count *
                   printFont.GetHeight(e.Graphics));
                e.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
                count++;
            }

            // If more lines exist, print another page.
            if (line != null)
                e.HasMorePages = true;
            else
                e.HasMorePages = false;
        }

        private void savePage()
        {
            File.WriteAllText("text.txt", richtextBox1.Text);
        }


        //Go button//
        private void goButton_Click(object sender, EventArgs e)
        {
            //Calls the mayhod to make the request and enable/disable the buttons needed.
            doRequest(urlBox.Text);
            //Writes the history list in the history file.
            History h = new History(urlBox.Text);
            h.appendHistory(h.getUrl(), "history.txt");
        }


        //Home button//
        private void homeButton_Click(object sender, EventArgs e)
        {
            homeurl = loadHome("home.txt");
            if (homeurl == "")
            {
                noHomepageLabel.Visible = true;
                // Adds the event and the event handler for the TimerEventProcessor method.

                timer1.Tick += new EventHandler(TimerEventProcessor);
                // Sets the timer interval to 4 seconds and starts the timer.
                timer1.Interval = 4000;
                timer1.Start();


            }
            else
            {
                //Sets the homeurl in the urlbox.
                urlBox.Text = homeurl;

                //Calls the mayhod to make the request and enable/disable the buttons needed.
                doRequest(urlBox.Text);

                //Writes the history list in the history file.
                History h = new History(urlBox.Text);
                h.appendHistory(h.getUrl(), "history.txt");
            }
        }

        //Tools DropDown Menu//
        
        //Set as Homepage Menu Item//
        private void setAsHomepageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Sets the current page to hamepage.( Writes the current url from the urlbox in the home file.)
            //Also accessible through "Ctrl+ H" shortcut.
            Home home = new Home(urlBox.Text);
            home.saveHome("home.txt", home.getUrl());
        }
        //Clear history Menu Item//
        private void clearHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Clears the history list and the history file.
            //Also accessible through "Ctrl+Shift+H" shortcut.
            listBox2.Items.Clear();
            File.WriteAllText("history.txt", String.Empty);
        }
        //New Window menu Item//
        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Starts a new thread which initializes a new window form.
            //Also accessible through "Ctrl+N" shortcut.
            new Thread(() => new Form1().ShowDialog()).Start();
        }
        //Exit Menu Item//
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Exits the application
            //Also accessible through "Alt+F4" shortcut.

            // Display a message box asking users if they
            // want to exit the application.
            if (MessageBox.Show("Do you want to exit?", "YouBrowse",
                  MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                  == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        
        //Favorites button//
        private void favoritesButton_Click(object sender, EventArgs e)
        {
            //Controls the visibility of the favorites groupbox and all its contents.
            if (textBox2.Visible == false)
            {
                textBox2.Visible = true;
                label1.Visible = true;
                groupBox3.Visible = true;
            }
            else
            {
                textBox2.Visible = false;
                label1.Visible = false;
                groupBox3.Visible = false;
            }
        }

        //Secondary textBox used to rename a new bookmark//

        //Controls the the KeyDown event in the editing textbox//
        private void edittextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //Assignes the current url in a variable.
            newName = textBox2.Text;
            //Checks if the variable is empty.
            if (!newName.Equals(""))
            {
                //Checks if "Enter" is pressed from the keyboard.
                if (e.KeyData == Keys.Enter)
                {
                    //Adds the alternative name(given by the user) to the favorites list, writes the name in the favnames file and the link (from the urlbox) to the favlinks file.
                    listBox1.Items.Add(newName);
                    Favorite f = new Favorite(urlBox.Text, newName);
                    f.appendFavorite(f.getUrl(), f.getName(), "favlinks.txt", "favnames.txt");
                    //Clears the edit textbox and sets groupbox3 (which contains the textbox and its explanatory label) to invisible.
                    textBox2.Clear();
                    groupBox3.Visible = false;
                    //textBox2.Visible = false;
                    //label1.Visible = false;
                    removeButton.Enabled = true;
                }
            }
            else //Case where the newName variable is empty.(the user did not provide an alternative name)
            {
                //Checks if "Enter" is pressed from the keyboard.
                if (e.KeyData == Keys.Enter)
                {
                    //Adds the current url(from the urlbox) to the favorites list, writes an empty line in the favnames file and the url in the favlinks file.
                    listBox1.Items.Add(urlBox.Text);
                    Favorite f = new Favorite(urlBox.Text, "");
                    f.appendFavorite(f.getUrl(), f.getName(), "favlinks.txt", "favnames.txt");
                    //Clears the edit textbox and sets groupbox3 (which contains the textbox and its explanatory label) to invisible.
                    textBox2.Clear();
                    groupBox3.Visible = false;
                    //textBox2.Visible = false;
                    //label1.Visible = false;
                    removeButton.Enabled = true;
                }
            }
        }

        
        //Cancel button//
        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Cancels the asynchronous operation. 
            this.backgroundWorker1.CancelAsync();


            // Disables the cancel button.
            cancelButton.Enabled = false;
            //Shows the cancel label.
            cancelLabel.Visible = true;
            // Adds the event and the event handler for the TimerEventProcessor method.
            timer2.Tick += new EventHandler(TimerEventProcessor1);
            // Sets the timer interval to 3 seconds and starts the timer.
            timer2.Interval = 3000;
            timer2.Start();
        }
                

        //Load methods//

        // Loads the contents of a file to the listbox//
        public void loadHistory(string path)
        {
            //Copies all entries of a file to a string array.
            string[] historylinks = System.IO.File.ReadAllLines(path);
            //Adds all entries of the array to a listbox.
            foreach (string hl in historylinks)
            {
                listBox2.Items.Add(hl);
            }
        }

        //Loads the contents of a file and return a string// 
        public string loadHome(string filename)
        {
            //Copies the contents of a file to a variable. 
            string homeurl = System.IO.File.ReadAllText(filename);
            return homeurl;
        }

        //Loads the contents of two files and adds them to the listbox//
        public void loadFavorites(string filel, string file2)
        {
            //Sets the counter to zero. 
            //This counter keeps track of the current element to be written in the listbox.
            int counter = 0;
            //Copies the contents of a file to a string array.
            string[] names = System.IO.File.ReadAllLines(file2);
            //Iterates to each entry in the string array
            foreach (string n in names)
            {
                //Checks if the string is not empty.
                if (!n.Equals(""))
                {
                    //Adds the item to a listbox.
                    listBox1.Items.Add(n);
                }
                else //Case where the string is empty.
                {
                    string link = null;
                    //Opens a file on another path (favlinks.txt).
                    using (StreamReader reader = new StreamReader(filel))
                    {
                        //Iterates to each line of the file till the given index.
                        for (int i = 0; i <= counter; ++i)
                        {
                            //Reads each line.
                            link = reader.ReadLine();
                        }
                        //Adds the last link to the listbox.
                        listBox1.Items.Add(link);
                        //Closes the file.
                        reader.Close();
                    }

                }
                //Increases the counter.
                counter++;
            }
        }

        //DoRequest//

        //Calls the background worker to make the request and enables/disables the buttons needed.
        public void doRequest(string url)
        {
            // Resets the text in the result textbox.
            richtextBox1.Text = String.Empty;

            // Disables the go and home buttons. 
            this.goButton.Enabled = false;
            this.homeButton.Enabled = false;

            //Disables the urlbox until the Background worker completes.
            this.urlBox.Enabled = false;

            // Enables the cancel button. 
            this.cancelButton.Enabled = true;


            // Starts the asynchronous operation.
            backgroundWorker1.RunWorkerAsync(url);

            //Adds the selected url in the urlbox.  
            urlBox.Text = url;
            //Adds the url to  the history list.
            listBox2.Items.Add(url);
            
        }

       
    }
}
