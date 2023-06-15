namespace NotePad_App
{
    public partial class Form1 : Form
    {
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
            Controls.Add(titleTextBox);

            TextBox textBox = new TextBox();
            textBox.Location = new Point(70, 68);
            textBox.Size = new Size(438, 290);
            textBox.Multiline = true;
            Controls.Add(textBox);

            ListBox noteListBox = new ListBox();
            noteListBox.Location = new Point(554, 70);
            noteListBox.Size = new Size(216, 289);
            Controls.Add(noteListBox);
        }
    }
}