using Microsoft.VisualBasic;
using Notepad.Forms;
using NotepadCore;
using NotepadCore.Functionality;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Notepad
{
    public partial class MainForm : Form
    {
        FileOperation fileOperation;
        EditOperation editOperation;
        Timer timer;
        FormFind formFind;
        FormReplace formReplace;
        public EditOperation EditOperation
        {
            get { return editOperation; }

            set { editOperation = value; }
        }

        public MainForm()
        {
            InitializeComponent();
            fileOperation = new FileOperation();
            editOperation = new EditOperation();
            formFind = new FormFind(this);
            formFind.Editor = txtArea;
            fileOperation.InitializeNewFile();
            this.Text = fileOperation.Filename;
            timer = new Timer();
            timer.Tick += Mytimer_Tick;
            timer.Interval = 500;
            txtArea.HideSelection = false;
        }

        private void Mytimer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            editOperation.Add_UndoRedo(txtArea.Text);
            UpdateView();
        }

        private void newFileMenu_Click(object sender, EventArgs e)
        {
            //New File Menu
            if (fileOperation.IsFileSaved)
            {
                //New File Status
                txtArea.Text = "";
                fileOperation.InitializeNewFile();
                UpdateView();
            }
            else
            {
                DialogResult result = MessageBox.Show("Do you need to save the Changes to " + fileOperation.Filename, "NoteNest", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question
                    );

                if (result == DialogResult.Yes)
                {
                    if (fileOperation.Filename.Contains("Untitled"))
                    {
                        SaveFileDialog newFileSave = new SaveFileDialog();
                        newFileSave.Filter = "Text(*.txt)|*.txt";
                        if (newFileSave.ShowDialog() == DialogResult.OK)
                        {
                            fileOperation.SaveFile(newFileSave.FileName, txtArea.Lines);
                            UpdateView();
                        }
                    }
                    else
                    {
                        fileOperation.SaveFile(fileOperation.FileLocation, txtArea.Lines);
                        UpdateView();
                    }

                }
                else if (result == DialogResult.No)
                {

                    //User select not to save so Initialize a new file
                    txtArea.Text = "";
                    fileOperation.InitializeNewFile();
                    UpdateView();
                }
            }
        }



        private void UpdateView()
        {
            this.Text = !fileOperation.IsFileSaved ? fileOperation.Filename + "*" : fileOperation.Filename;
            undoEditMenu.Enabled = editOperation.CanUndo() ? true : false;
            redoEditMenu.Enabled = editOperation.CanRedo() ? true : false;
            findnextEditMenu.Enabled = findEditMenu.Enabled;
        }

        private void txtArea_TextChanged(object sender, EventArgs e)
        {
            fileOperation.IsFileSaved = false;
            if (editOperation.TxtAreaTextChangeRequired)
            {
                timer.Start();
            }
            else
            {
                editOperation.TxtAreaTextChangeRequired = false;
            }
            UpdateView();
        }

        private void openFileMenu_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text(*.txt)|*.txt";
            openFile.InitialDirectory = "C:";
            openFile.Title = "Open File";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                txtArea.TextChanged -= txtArea_TextChanged;
                txtArea.Text = fileOperation.OpenFile(openFile.FileName);
                txtArea.TextChanged += txtArea_TextChanged;
                UpdateView();
            }
        }

        private void saveFileMenu_Click(object sender, EventArgs e)
        {
            if (!fileOperation.IsFileSaved)
            {
                if (!this.Text.Contains("Untitled.txt"))
                {
                    fileOperation.SaveFile(fileOperation.FileLocation, txtArea.Lines);
                    UpdateView();
                }
                else
                {
                    SaveFile();
                }
            }
        }

        private void SaveFile()
        {
            SaveFileDialog fileSave = new SaveFileDialog();
            fileSave.Filter = "Text(*.txt)|*.txt";
            if (fileSave.ShowDialog() == DialogResult.OK)
            {
                fileOperation.SaveFile(fileSave.FileName, txtArea.Lines);
                UpdateView();
            }
        }

        private void saveasFileMenu_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void exitFileMenu_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void editMenu_Click(object sender, EventArgs e)
        {
            cutEditMenu.Enabled = txtArea.SelectedText.Length > 0 ? true : false;
            copyEditMenu.Enabled = txtArea.SelectedText.Length > 0 ? true : false;
            pasteEditMenu.Enabled = Clipboard.GetDataObject().GetDataPresent(DataFormats.Text);
        }

        private void cutEditMenu_Click(object sender, EventArgs e)
        {
            txtArea.Cut();
            pasteEditMenu.Enabled = true;
        }

        private void copyEditMenu_Click(object sender, EventArgs e)
        {
            txtArea.Copy();
            pasteEditMenu.Enabled = true;
        }

        private void pasteEditMenu_Click(object sender, EventArgs e)
        {
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
            {
                txtArea.Paste();
            }
        }

        private void editMenu_MouseEnter(object sender, EventArgs e)
        {
            editMenu_Click(sender, e);
        }

        private void selectallEditMenu_Click(object sender, EventArgs e)
        {
            txtArea.SelectAll();
        }

        private void deleteEditMenu_Click(object sender, EventArgs e)
        {
            txtArea.Text = txtArea.Text.Remove(txtArea.SelectionStart, txtArea.SelectionLength);
        }

        private void timedateEditMenu_Click(object sender, EventArgs e)
        {
            txtArea.Text = txtArea.Text.Insert(txtArea.SelectionStart, editOperation.DateTime_Now());
        }

        private void undoEditMenu_Click(object sender, EventArgs e)
        {
            txtArea.Text = editOperation.UndoClicked();
            UpdateView();
        }

        private void redoEditMenu_Click(object sender, EventArgs e)
        {
            txtArea.Text = editOperation.RedoClicked();
            UpdateView();
        }

        private void gotoEditMenu_Click(object sender, EventArgs e)
        {

            string input = Interaction.InputBox("Enter Line Number:", "Go To Line", "1");
            try
            {
                int lineNumber = Convert.ToInt32(input);
                if (lineNumber < 1 || lineNumber > txtArea.Lines.Length)
                {
                    MessageBox.Show($"Invalid Line! Enter a number between 1 and {txtArea.Lines.Length}.",
                                    "Out of Range", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show("Input cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int charIndex = 0;
                for (int i = 0; i < lineNumber - 1; i++)
                {
                    charIndex += txtArea.Lines[i].Length + 1;
                }
                txtArea.Focus();
                txtArea.Select(charIndex, txtArea.Lines[lineNumber - 1].Length);
                txtArea.ScrollToCaret();

                MessageBox.Show($"Moved to Line {lineNumber}", "Go To Line", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Please enter a valid number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void findEditMenu_Click(object sender, EventArgs e)
        {
            if (formFind == null)
            {
                formFind = new FormFind(this);
                formFind.Editor = txtArea;
            }
            formFind.Show();
        }

        private void findnextEditMenu_Click(object sender, EventArgs e)
        {
            formFind.UpdateSearchQuery();
            if (formFind.Qry.SearchString.Length == 0)
            {
                formFind.Show();
            }
            else
            {
                FindNextResult result = editOperation.FindNext(formFind.Qry);
                if (result.SearchStatus)
                {
                    txtArea.Select(result.SelectionStart, formFind.Qry.SearchString.Length);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            txtArea.WordWrap = wordwrapFormatMenu.Checked;
            statusbarViewMenu.Enabled = !wordwrapFormatMenu.Checked;
            if (statusbarViewMenu.Enabled)
            {
                statusbarViewMenu.Checked = true;
            }
            statusContent.Visible = statusbarViewMenu.Checked;
        }

        private void wordwrapFormatMenu_CheckedChanged(object sender, EventArgs e)
        {
            txtArea.WordWrap = wordwrapFormatMenu.Checked;
            statusbarViewMenu.Enabled = !wordwrapFormatMenu.Checked;
            statusbarViewMenu.Checked = true;
            statusContent.Visible = statusbarViewMenu.Enabled;
        }

        private void statusbarViewMenu_CheckedChanged(object sender, EventArgs e)
        {
            statusContent.Visible = statusbarViewMenu.Checked;
        }

        private void txtArea_SelectionChanged(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            int pos = txtArea.SelectionStart;
            int line = txtArea.GetLineFromCharIndex(pos) + 1;
            int col = pos - txtArea.GetFirstCharIndexOfCurrentLine() + 1;
            status.Text = "Ln " + line + ", Col " + col;
        }

        private void replaceEditMenu_Click(object sender, EventArgs e)
        {
            if (formReplace == null)
            {
                formReplace = new FormReplace();
                formReplace.Editor = txtArea;
                formReplace.editOpertion = editOperation;
            }
            formReplace.Show();
        }

        private void fontFormatMenu_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.ShowColor = true;
            fontDialog.Font = txtArea.Font;
            fontDialog.Color = txtArea.ForeColor;
            if (fontDialog.ShowDialog() != DialogResult.Cancel)
            {
                txtArea.Font = fontDialog.Font;
                txtArea.ForeColor = fontDialog.Color;
            }
        }

        private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.webbie.org.uk/thunder/Help/_Microsoft/_Microsoft_Applications/_NotePad/NotePad_MB_Help.htm#:~:text=Hold%20down%20the%20ALT%20key,brief%20description%20of%20its%20function.");
        }

        private void aboutnotepadHelpMenu_Click(object sender, EventArgs e)
        {
            FormAbout aboutForm = new FormAbout();
            aboutForm.ShowDialog();
        }
    }
}
