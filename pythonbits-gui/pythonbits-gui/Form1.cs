using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;

namespace pythonbits_gui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // click Browse -> display Browse dialog
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            // user picked a file -> set it in the "File name" input
            textBox1.Text = openFileDialog1.FileName;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // "File name" input has changed -> try to guess scene name
            // set it to Series / Movie name input
            string fn = openFileDialog1.SafeFileName;
            string res = "";
            string[] matchers = new string[] {
@"(.*)\.S(\d{2})E(\d{2})", 
    // Derp.Dong.S01E03.720p.derp-HERP
@"(.*)\.\d{4}\.((720p|1080p)\.)?(DVDRip|BRRip|BDRip|BluRay|CAM|TS|TC|R5|DVDSCR)",
    // Dinga.Ding.2009.720p.BRRip.XviD-LOLDONGS
@"(.*)\.(\d{4})", 
    // Unga.Bunga.2010
@"(.*)\.(LIMITED|UNRATED|READNFO|INTERNAL|PROPER)", 
    // sometimes the year is missing and one of these might be in its place
            };
            foreach (string onematch in matchers)
            {
                Regex r = new Regex(onematch, RegexOptions.IgnoreCase);
                Match m = r.Match(fn);
                if (m.Success)
                {
                    button2.Enabled = true;
                    res = m.Groups[1].Value;
                    break;
                }
            }
            if (res == "")
            { // we've failed, try the directory name
                string dirname = openFileDialog1.FileName.Replace('\\' + fn, "");
                dirname = dirname.Substring(dirname.LastIndexOf('\\') + 1);
                foreach (string onematch in matchers)
                {
                    Regex r = new Regex(onematch, RegexOptions.IgnoreCase);
                    Match m = r.Match(dirname);
                    if (m.Success)
                    {
                        button2.Enabled = true;
                        res = m.Groups[1].Value;
                        break;
                    }
                }
            }

            if (res == "")
            {   // FOILED, AGAIN!
                // set label text to red
                res = fn;
                label2.ForeColor = System.Drawing.Color.OrangeRed;
            }

            textBox2.Text = res.Replace('.', ' ');
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            // user is changing Movie / Series name
            // if label was red, turn it back
            // start with select all, but only if user tabs to control
            label2.ForeColor = System.Drawing.SystemColors.ControlText;
            textBox2.SelectAll();
        }

        private void label2_ForeColorChanged(object sender, EventArgs e)
        {
            if (label2.ForeColor == System.Drawing.SystemColors.ControlText)
                button2.Enabled = true;
            else
                button2.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // let's gen the desc!
            
            // don't let the user double click the button
            button2.Enabled = false;

            // hourglass cursor while working
            Cursor.Current = Cursors.WaitCursor;
            
            // open a pipe to bin\pythonbits.exe and grab its stdout
            // quotes around the params in case of spaces
            
            ProcessStartInfo pythonbits_bin_descriptor = new ProcessStartInfo(
                "bin\\pythonbits.exe", "\"" + textBox2.Text + "\" \"" + textBox1.Text + "\""
                );
            pythonbits_bin_descriptor.RedirectStandardOutput = true;
            pythonbits_bin_descriptor.UseShellExecute = false;
            Process pythonbits_bin = Process.Start(pythonbits_bin_descriptor);
            StreamReader pythonbits_out = pythonbits_bin.StandardOutput;
            pythonbits_bin.WaitForExit(60000);

            // we're done, change the cursor back and post the desc to the multiline input
            Cursor.Current = Cursors.Default;
            textBox3.Text = pythonbits_out.ReadToEnd();

            // close the stream & process
            pythonbits_out.Close();
            pythonbits_bin.Close();

        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            // grab the first file that user drops and run the file dialog events on it
            string[] filesDropped = (string[])e.Data.GetData(DataFormats.FileDrop);
            openFileDialog1.FileName = filesDropped[0];
            openFileDialog1_FileOk(this, null);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            // drag & drop handling
            // if user is dropping a file, allow it
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.All;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // see if user is trying to "open with"
            string[] args = Environment.GetCommandLineArgs();
            try
            {
                string fn = args[1];
                openFileDialog1.FileName = fn;
                openFileDialog1_FileOk(this, null);
            }
            catch (IndexOutOfRangeException)
            {
                // no command line param, do display form as normally
            }
        }
    }
}
