using NotepadCore;
using System.Windows.Forms;
using System;

namespace Notepad
{
    public partial class MainForm : Form
    {
        FileOperation fileOperation;
        public MainForm()
        {
            InitializeComponent();
            fileOperation=new FileOperation();
            fileOperation.InitializeNewFile();
            this.Text = fileOperation.Filename;
        }

        private void newFileMenu_Click(object sender, EventArgs e)
        {
            //New File Menu
            if (fileOperation.IsFileSaved)
            {
                //New File Status
                fileOperation.InitializeNewFile();
                txtArea.Text = "";
                UpdateView();
            }
            else
            {
                DialogResult result = MessageBox.Show("Do you need to save the Changes to " + fileOperation.Filename, "NoteNest", MessageBoxButtons.YesNoCancel, MessageBoxIcon.
                    );

                if (result == DialogResult.Yes)
                {
                    if (fileOperation.Filename.Contains("Untitled")) {
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
                        fileOperation.SaveFile(fileOperation.Filename, txtArea.Lines);
                        UpdateView();
                    }

                }
                else if (result == DialogResult.No) {

                    //User select not to sav so Initialize a new file
                    txtArea.Text = "";
                    fileOperation.InitializeNewFile();
                    UpdateView();
                }
            }
        }

    

        private void UpdateView()
        {
            this.Text = !fileOperation.IsFileSaved ? fileOperation.Filename + "*" : fileOperation.Filename;
        }

        private void txtArea_TextChanged(object sender, EventArgs e)
        {
            fileOperation.IsFileSaved = false;
            UpdateView();
        }

        private void openFileMenu_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text(*.txt)|*.txt";
            openFile.InitialDirectory = "D:";
            openFile.Title = "Open File";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                txtArea.TextChanged -= txtArea_TextChanged;
                txtArea.Text = fileOperation.OpenFile(openFile.FileName);
                txtArea.TextChanged += txtArea_TextChanged;
                UpdateView();
            }
        }

      
    }
}
