using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace NotePad_App
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Stores data about a note
        /// </summary>
        struct Note
        {
            private static int _id = 1;
            /// <summary>
            /// Constructor that automatically assigns an ID
            /// </summary>
            /// <param name="title">Title of the note</param>
            /// <param name="text">Text within the note</param>
            public Note(string title, string text)
            {
                this.title = title;
                this.text = text;
                noteId = _id++;
            }
            /// <summary>
            /// Manual constructor
            /// </summary>
            /// <param name="title">Title of the note</param>
            /// <param name="text">Text within the note</param>
            /// <param name="noteId">ID of the note</param>
            public Note(string title, string text, int noteId)
            {
                this.title = title;
                this.text = text;
                this.noteId = noteId;
                _id = noteId + 1;
            }
            public int noteId { get; private set; }
            public string title { get; set; }
            public string text { get; set; }
        }
        List<Note> notes = new List<Note>();
        string noteSavePath = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Called upon form load, creates form structure and handles stored notes
        /// </summary>
        /// <param name="sender">The form loades</param>
        /// <param name="e">The event argument</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "NotePad";
            int[,] labelPoints = new int[,] { { 21, 35 }, { 21, 70 }, { 554, 41 } }; //Stores the points where the labels are located
            string[] labelText = new string[] { "Title:", "Text:", "List of Notes:" };//Stores the text for each label

            int[,] buttonPoints = new int[,] { { 70, 369 }, { 182, 369 }, { 294, 369 }, { 562, 369 }, { 678, 369 } };// Stores the points where the buttons are located
            string[] buttonText = new string[] { "Save", "Save As", "New", "Open Folder", "Delete" }; //Stores the button text
            for (int i = 0; i < 5; i++) //For loop to create and add each button and label
            {
                Button newbutton = new Button();
                newbutton.Location = new Point(buttonPoints[i, 0], buttonPoints[i, 1]);
                newbutton.Size = new Size(84, 29);
                newbutton.Text = buttonText[i];
                newbutton.Name = buttonText[i];
                newbutton.Click += new EventHandler(ButtonPress);
                Controls.Add(newbutton);

                if (i < 3)
                {
                    Label newLabel = new Label();
                    newLabel.Text = labelText[i];
                    newLabel.Location = new Point(labelPoints[i, 0], labelPoints[i, 1]);
                    newLabel.Size = i == 2 ? new Size(100, 15) : new Size(38, 15); //Uses the i index to determine which label size to use
                    Controls.Add(newLabel);
                }
            }

            //Creates and adds remaining UI elements
            TextBox titleTextBox = new TextBox();
            titleTextBox.Location = new Point(70, 33);
            titleTextBox.Size = new Size(438, 23);
            titleTextBox.Name = "Box";
            Controls.Add(titleTextBox);

            TextBox textBox = new TextBox();
            textBox.Location = new Point(70, 68);
            textBox.Size = new Size(438, 290);
            textBox.Multiline = true;
            textBox.Name = "Box";
            Controls.Add(textBox);

            ListBox noteListBox = new ListBox();
            noteListBox.Location = new Point(554, 70);
            noteListBox.Size = new Size(216, 289);
            noteListBox.Name = "Box";
            noteListBox.DoubleClick += new EventHandler(ListBoxDoubleClick);
            Controls.Add(noteListBox);
            //Each of these three has the Name property "Box" to be used as a search key in Controls.Find()
        }
        /// <summary>
        /// Detects when an assigned button has been pressed
        /// </summary>
        /// <param name="sender">The button pressed</param>
        /// <param name="e">The event argument</param>
        private void ButtonPress(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                Control[] textBoxes = Controls.Find("Box", false);
                ListBox tempList = textBoxes[2] as ListBox;
                switch (button.Name)
                {
                    case "Save":
                        SaveNote(textBoxes);
                        break;

                    case "Save As":
                        SaveNoteAs(textBoxes);
                        break;

                    case "New":
                        textBoxes[0].Text = string.Empty;
                        textBoxes[1].Text = string.Empty;
                        break;

                    case "Open Folder":
                        OpenNoteFolder(textBoxes);
                        break;

                    case "Delete":
                        DeleteNote(textBoxes);
                        break;
                }
            }
        }

        private void ListBoxDoubleClick(object sender, EventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if(listBox != null)
            {
                Control[] textBoxes = Controls.Find("Box", false);
                if (listBox.SelectedItem == null)
                {
                    return;
                }
                string title = listBox.SelectedItem.ToString();
                string[] noteData;
                if (File.Exists($"{noteSavePath}\\{title}.txt"))
                {
                    textBoxes[0].Text = title;
                    textBoxes[1].Text = File.ReadAllText($"{noteSavePath}\\{title}.txt");
                }
            }
        }
        private void SaveNote(Control[] textBoxes)
        {
            string title = textBoxes[0].Text;
            string text = textBoxes[1].Text;
            bool saved = false;
            if(noteSavePath == string.Empty && OpenNoteFolder(textBoxes))
            {
                foreach (string s in Directory.GetFiles(noteSavePath))
                {
                    FileInfo fileInfo = new FileInfo(s);
                    Console.WriteLine(fileInfo.Name);
                    if (fileInfo.Name.Substring(0, fileInfo.Name.Length - 4) == title)
                    {
                        File.WriteAllText(s, text);
                        saved = true;
                        break;
                    }
                }
                if (!saved)
                {
                    SaveNoteAs(textBoxes);
                }
            }
        }
        private void SaveNoteAs(Control[] textBoxes)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.FileName = textBoxes[0].Text;
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName))
                {
                    streamWriter.WriteLine(textBoxes[1].Text);
                }
            }
        }

        private bool OpenNoteFolder(Control[] textBoxes)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = true;
            folderBrowserDialog.ShowDialog();
            noteSavePath = folderBrowserDialog.SelectedPath;

            ListBox tempList = textBoxes[2] as ListBox;
            tempList.Items.Clear();
            if(folderBrowserDialog.SelectedPath == string.Empty)
            {
                return false;
            }
            foreach (string s in Directory.GetFiles(folderBrowserDialog.SelectedPath))
            {
                FileInfo fileInfo = new FileInfo(s);
                Console.WriteLine(fileInfo.Name);
                if (fileInfo.Name.Contains(".txt"))
                {
                    tempList.Items.Add(fileInfo.Name.Substring(0, fileInfo.Name.Length - 4));
                }
            }
            textBoxes[2] = tempList;
            return true;
        }

        private void DeleteNote(Control[] textBoxes)
        {
            ListBox tempList = textBoxes[2] as ListBox;
            if(tempList.SelectedItem == null)
            {
                return;
            }
            string title = tempList.SelectedItem.ToString();
            tempList.Items.Remove(tempList.SelectedItem);
            File.Delete($"{noteSavePath}\\{title}.txt");
        }
    }
}