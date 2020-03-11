using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CrossWordPuzle
{
    public partial class Form1 : Form
    {
        Clues clue_window = new Clues();
        List<id_cells> idc = new List<id_cells>();
        public string puzzle_file = Application.StartupPath + "\\Puzzles\\puzzle_1.pzl";
        
        public Form1()
        {
            buildWorldList();
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void buildWorldList()
        {
            string line = "";
            using (StreamReader s = new StreamReader(puzzle_file))
            {
                line = s.ReadLine();//ignores the first line
                while ((line=s.ReadLine())!=null)
                {
                    string[] l = line.Split('|');
                    idc.Add(new id_cells(Int32.Parse(l[0]), Int32.Parse(l[1]), l[2], l[3], l[4], l[5]));
                    clue_window.dgvClues.Rows.Add(new string[] { l[3], l[2], l[5] });
                }
            }

        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Help About");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeBoard();
            clue_window.SetDesktopLocation(this.Location.X + this.Width + 1, this.Location.Y);
            clue_window.StartPosition = FormStartPosition.Manual;

            clue_window.Show();
            clue_window.dgvClues.AutoResizeColumns();
        }
        private void InitializeBoard()
        {
            dgvPuzzle.BackgroundColor = Color.Black;
            dgvPuzzle.DefaultCellStyle.BackColor = Color.Black;


            for (int i = 0;  i< 21;i++)
            
                dgvPuzzle.Rows.Add();
            //Set width of column
            foreach ( DataGridViewColumn c in dgvPuzzle.Columns)
            {
                c.Width = dgvPuzzle.Width / dgvPuzzle.Columns.Count;

            }
            foreach (DataGridViewRow c in dgvPuzzle.Rows)
            {
                c.Height = dgvPuzzle.Height / dgvPuzzle.Columns.Count;

            }
            for (int row = 0; row < dgvPuzzle.Rows.Count; row++)
            {
                for (int col = 0; col < dgvPuzzle.Columns.Count; col++)
                {
                    dgvPuzzle[col, row].ReadOnly = true;
                }
            }

            foreach ( id_cells i in idc)
            {
                int start_col = i.X;
                int start_row = i.Y;
                char[] word = i.word.ToCharArray();

                for (int j = 0; j < word.Length; j++)
                {
                    if (i.direction.ToUpper() == "ACROSS")
                        formatCell(start_row, start_col + j, word[j].ToString());
                    if (i.direction.ToUpper() == "DOWN")
                        formatCell(start_row+j, start_col, word[j].ToString());
                }

            }

        }
        private void formatCell(int row,int col,string letter)
        {
            DataGridViewCell c = dgvPuzzle[col, row];
            c.Style.BackColor = Color.White;
            c.ReadOnly = false;
            c.Style.SelectionBackColor = Color.Cyan;
            c.Tag = letter;
        }

        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            clue_window.SetDesktopLocation(this.Location.X + this.Width + 1, this.Location.Y);
        }

        private void dgvPuzzle_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //make letter uppercase
            try
            {
                dgvPuzzle[e.ColumnIndex, e.RowIndex].Value = dgvPuzzle[e.ColumnIndex, e.RowIndex].Value.ToString().ToUpper();
            }
            catch 
            {

               
            }
            //truncate to one letter
            try
            {
                if (dgvPuzzle[e.ColumnIndex, e.RowIndex].Value.ToString().Length > 1)
                    dgvPuzzle[e.ColumnIndex, e.RowIndex].Value = dgvPuzzle[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, 1);
            }
            catch
            {

               
            }
            //format color if correct
            try
            {
                if (dgvPuzzle[e.ColumnIndex, e.RowIndex].Value.ToString().ToUpper().Equals(dgvPuzzle[e.ColumnIndex, e.RowIndex].Tag.ToString().ToUpper()))
                    dgvPuzzle[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.DarkSeaGreen;
                else
                    dgvPuzzle[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.IndianRed;
            }
            catch
            {


            }
        }

        private void openPuzzleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Puzzle Files|*.pzl";
            if (ofd.ShowDialog()==DialogResult.OK)
            {
                puzzle_file = ofd.FileName;
                dgvPuzzle.Rows.Clear();
                clue_window.dgvClues.Rows.Clear();
                idc.Clear();

                buildWorldList();
                InitializeBoard();
            }
        }

        private void dgvPuzzle_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            string number = "";
            //foreach(item c in list of items)
            if(idc.Any(c=>(number=c.number)!=""&& c.X == e.ColumnIndex &&c.Y==e.RowIndex))
            {
                Rectangle r = new Rectangle(e.CellBounds.X,e.CellBounds.Y,e.CellBounds.Width,e.CellBounds.Height);
                e.Graphics.FillRectangle(Brushes.White, r);
                Font f = new Font(e.CellStyle.Font.FontFamily, 7);
                e.Graphics.DrawString(number, f, Brushes.Black, r);
                e.PaintContent(e.ClipBounds);
                e.Handled = true;

            }
        }
    }
    public class id_cells
    {
        public int X;
        public int Y;
        public string direction;
        public string number;
        public string word;
        public string clue;

        public id_cells(int x,int y,string d,string n,string w,string c)
        {
            this.X = x;
            this.Y = y;
            this.direction = d;
            this.number = n;
            this.word = w;
            this.clue = c;
        }
    }
}
