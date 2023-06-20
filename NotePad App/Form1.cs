using System.Globalization;
using System.Xml;

namespace NotePad_App
{
    public partial class Form1 : Form
    {
        struct note
        {
            private static int _id = 1;
            public note(string title, string text)
            {
                this.title = title;
                this.text = text;
                noteId = _id++;
            }

            public note(string title, string text, int noteId)
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
        List<note> notes = new List<note>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int[,] labelPoints = new int[,] { { 21, 35 }, { 21, 70 }, { 554, 41 } }; //Stores the points where the labels are located
            string[] labelText = new string[] { "Title:", "Text:", "List of Notes:" };//Stores the text for each label

            int[,] buttonPoints = new int[,] { { 70, 369 }, { 182, 369 }, { 562, 369 }, { 678, 369 } };// Stores the points where the buttons are located
            string[] buttonText = new string[] { "Save", "New", "Open", "Delete" }; //Stores the button text
            for (int i = 0; i < 4; i++) //For loop to create and add each button and label
            {
                Button newbutton = new Button();
                newbutton.Location = new Point(buttonPoints[i, 0], buttonPoints[i, 1]);
                newbutton.Size = new Size(84, 29);
                newbutton.Text = buttonText[i];
                newbutton.Name = buttonText[i];
                newbutton.Click += new EventHandler(buttonPress);
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
            Controls.Add(noteListBox);
            //Each of these three has the Name property "Box" to be used as a search key in Controls.Find()

            LoadNotesFromFile();
            PopulateNoteList();
        }

        private void buttonPress(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                Control[] textBoxes = Controls.Find("Box", false);
                ListBox tempList = textBoxes[2] as ListBox;
                switch (button.Name)
                {
                    case "Save":
                        string title = textBoxes[0].Text;
                        string text = textBoxes[1].Text;

                        tempList = textBoxes[2] as ListBox;
                        tempList.Items.Add(title);
                        textBoxes[2] = tempList;
                        notes.Add(new note(title, text));
                        WriteNotesToFile();
                        break;

                    case "New":
                        textBoxes[0].Text = "";
                        textBoxes[1].Text = "";
                        break;

                    case "Open":
                        tempList = textBoxes[2] as ListBox;
                        if(tempList.SelectedItem != null)
                        {
                            string selectedNote = tempList.SelectedItem.ToString();
                            note note = GetNote(selectedNote);
                            textBoxes[0].Text = note.title;
                            textBoxes[1].Text = note.text;
                        }
                        break;

                    case "Delete":
                        tempList = textBoxes[2] as ListBox;
                        tempList.Items.Remove(tempList.SelectedItem);
                        break;
                }
            }
        }

        private note GetNote(string noteTitle)
        {
            foreach(note note in notes)
            {
                if(note.title == noteTitle)
                {
                    return note;
                }
            }
            return new note();
        }

        private void WriteNotesToFile()
        {
            using (StreamWriter writer = new StreamWriter("noteData.txt"))
            {
                foreach(note noteToWrite in notes)
                {
                    writer.WriteLine($"#{noteToWrite.noteId}: {noteToWrite.title}");
                    writer.WriteLine(noteToWrite.text);
                }
            }
        }

        private void LoadNotesFromFile()
        {
            string[] noteData;
            using (StreamReader reader = new StreamReader("noteData.txt"))
            {
                List<string> data = new List<string>();
                while (!reader.EndOfStream)
                {
                    data.Add(reader.ReadLine());
                }
                noteData = data.ToArray();
            }
            for(int i = 0; i < noteData.Length; i++)
            {
                if (noteData[i].Contains("#") && noteData[i].Contains(":"))
                {
                    int noteId = int.Parse(noteData[i][1].ToString());
                    string noteTitle = noteData[i].Substring(4);
                    string text = "";
                    while (i + 1 < noteData.Length && noteData[i + 1].Length > 0)
                    {
                        if (!noteData[i + 1].Contains("#") && !noteData[i + 1].Contains(":"))
                        {
                            text += $"{noteData[i + 1]}\r\n";
                            i++;
                        }
                        else
                        {
                            i--;
                            break;
                        }
                        
                    }
                    notes.Add(new note(noteTitle, text, noteId));
                }
            }
        }

        private void PopulateNoteList()
        {
            Control[] textBoxes = Controls.Find("Box", false);
            ListBox tempList = textBoxes[2] as ListBox;
            foreach(note note in notes)
            {
                tempList = textBoxes[2] as ListBox;
                tempList.Items.Add(note.title);
            }
            textBoxes[2] = tempList;
        }
    }
}