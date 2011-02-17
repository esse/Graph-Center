using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using IronPython.Hosting;
using IronPython;
using System.Reflection;
using Microsoft.Scripting.Hosting;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


//
//




namespace grafy
{
    public partial class Form1 : Form
    {

        string imp_dir;
        int points_count;
        int[] a = new int[2];
        int[] b = new int[2];
        int counter = 0;
        List<int[]> edges = new List<int[]>();
        Dictionary<string, int> points = new Dictionary<string, int>();
        Dictionary<int, string> reverse_points = new Dictionary<int, string>();
        int startpointx;
        int finishpointx;
        int startpointy;
        int finishpointy;
        System.Drawing.Pen myPen;
        System.Drawing.Graphics formGraphics;
        System.Drawing.SolidBrush myBrush;
        ScriptEngine m_engine;
        ScriptScope m_scope;
        ScriptSource source;
        UndoClass undo;

        public Form1()
        {
            InitializeComponent();
            imp_dir = Directory.GetCurrentDirectory();
            myPen = new System.Drawing.Pen(System.Drawing.Color.Red);
            formGraphics = pictureBox1.CreateGraphics();
            pictureBox1.BackColor = Color.White;
            myBrush = new System.Drawing.SolidBrush(Color.Red);
            m_engine = Python.CreateEngine();
            m_scope = m_engine.CreateScope();
            source = m_engine.CreateScriptSourceFromFile("center_iron.py");
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Cursor.Current = Cursors.WaitCursor;
            string parameters = "";
            foreach (int[] x in edges)
            {
                parameters = parameters + x[0] + "," + x[1] + " ";
            }
            string s = ""; 
            parameters = parameters.Remove(parameters.Length - 1, 1);
            m_scope.SetVariable("ret", parameters);
            Directory.SetCurrentDirectory(imp_dir);
            source.Execute(m_scope);
            s = m_scope.GetVariable<String>("res");
            List<String> skl = new List<String>();
            string[] sk = s.Split(' ');
            skl.AddRange(sk);
            skl.RemoveAt(skl.Count - 1);
            foreach (string ctr in skl)
            {
                int q = Int32.Parse(ctr);
                string wx = reverse_points[q];
                string[] wq = wx.Split(',');
                int myx = Int32.Parse(wq[0]);
                int myy = Int32.Parse(wq[1]);
                myPen.Color = Color.Blue;
                myBrush.Color = Color.Blue;
                formGraphics.DrawEllipse(myPen, myx-8, myy-8, 16, 16);
                formGraphics.FillEllipse(myBrush, myx - 8, myy - 8, 16, 16);
            }
            myPen.Color = Color.Red;
            myBrush.Color = Color.Red;
            Cursor.Current = Cursors.Default;            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clean();
        }

        private void panel1_Click(object sender, MouseEventArgs e)
        {
            undo = new UndoClass(new List<int[]>(edges), new Dictionary<string, int>(points), new Dictionary<int,string>(reverse_points));
            startpointx = e.X;
            startpointy = e.Y;
            a[0] = startpointx / 20;
            a[1] = startpointy/20;
            counter++;

       
            points_count++;
            string key = String.Format("{0}, {1}", a[0], a[1]);
            string key2 = String.Format("{0}, {1}", startpointx, startpointy);
            if (!points.ContainsKey(key))
            {
                if (!(counter > 1))
                {
                    formGraphics.DrawEllipse(myPen, startpointx - 8, startpointy - 8, 16, 16);
                    formGraphics.FillEllipse(myBrush, startpointx - 8, startpointy - 8, 16, 16);
                    points.Add(key, points_count);
                    reverse_points.Add(points_count, key2);
                }
                else
                {
                    MessageBox.Show("Graph must be coherent!");
                }
            }
           
         //   }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            undo = new UndoClass(new List<int[]>(edges), new Dictionary<string, int>(points), new Dictionary<int, string>(reverse_points));
            finishpointx = e.X;
            finishpointy = e.Y;
            b[0] = finishpointx / 20;
            b[1] = finishpointy/20;
            counter++;
            string key = String.Format("{0}, {1}", b[0], b[1]);
            if (!points.ContainsKey(key) && checkBox1.Checked)
            {
                MessageBox.Show("Setting is 'don't create new vertices' - can't draw new vertex");
                return;
            }
            
          
                formGraphics.DrawLine(myPen, startpointx, startpointy, finishpointx, finishpointy);
                
                

                points_count++;
                
                string beginkey = String.Format("{0}, {1}", a[0], a[1]);
                string key2 = String.Format("{0}, {1}", finishpointx, finishpointy);
                if (!points.ContainsKey(key))
                {
                    points.Add(key, points_count);
                    reverse_points.Add(points_count, key2);
                    formGraphics.DrawEllipse(myPen, finishpointx - 8, finishpointy - 8, 16, 16);
                    formGraphics.FillEllipse(myBrush, finishpointx - 8, finishpointy - 8, 16, 16);
                }
            
                int[] edg = { points[beginkey], points[key] };
                
                edges.Add(edg);
                button7.Enabled = true;      
        }

        private void Clean()
        {
            button7.Enabled = false;
            a = new int[2];
            b = new int[2];
            edges = new List<int[]>();
            points = new Dictionary<string, int>();
            reverse_points = new Dictionary<int, string>();
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
            counter = 0;
            points_count=0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Zapis grafu";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                Cursor.Current = Cursors.WaitCursor;
                saveFileDialog1.FileName = saveFileDialog1.FileName + ".grp";
                Stream stream = (Stream)saveFileDialog1.OpenFile();
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, edges);
                stream.Close();
                saveFileDialog1.FileName = saveFileDialog1.FileName.Substring(0, saveFileDialog1.FileName.Length - 3) + "grh";
                stream = (Stream)saveFileDialog1.OpenFile();
                bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, points);
                stream.Close();
                saveFileDialog1.FileName = saveFileDialog1.FileName.Substring(0,saveFileDialog1.FileName.Length - 3) + "gri";
                stream = (Stream)saveFileDialog1.OpenFile();
                bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, reverse_points);
                stream.Close();
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Graf zapisany");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.Title = "Wybierz dowolny z trzech zapisanych plików grafu";
            fDialog.Filter = "Pliki grafów|*.grp;*.gri;*.grh";
            fDialog.CheckFileExists = true;
            fDialog.CheckPathExists = true;
            fDialog.ShowDialog();
            if (fDialog.FileName != "")
            {
                Clean();
                Cursor.Current = Cursors.WaitCursor;
                fDialog.FileName = fDialog.FileName.Substring(0, fDialog.FileName.Length - 3) + "grp";
                Stream stream = (Stream)fDialog.OpenFile();
                BinaryFormatter bformatter = new BinaryFormatter();
                edges = (List<int[]>)bformatter.Deserialize(stream);
                stream.Close();
                fDialog.FileName = fDialog.FileName.Substring(0, fDialog.FileName.Length - 3) + "grh";
                stream = (Stream)fDialog.OpenFile();
                bformatter = new BinaryFormatter();
                points = (Dictionary<string, int>)bformatter.Deserialize(stream);
                stream.Close();
                fDialog.FileName = fDialog.FileName.Substring(0, fDialog.FileName.Length - 3) + "gri";
                stream = (Stream)fDialog.OpenFile();
                bformatter = new BinaryFormatter();
                reverse_points = (Dictionary<int, string>)bformatter.Deserialize(stream);
                stream.Close();
                Redraw();
                Cursor.Current = Cursors.Default;
                button7.Enabled = false;
                MessageBox.Show("Graf odczytany");
            }
        }

        private void Redraw()
        {
            pictureBox1.Image = null;
            pictureBox1.Refresh();
            foreach (KeyValuePair<int, string> kvp in reverse_points)
            {
                string[] wq = kvp.Value.Split(',');
                int myx = Int32.Parse(wq[0]);
                int myy = Int32.Parse(wq[1]);
                myPen.Color = Color.Red;
                myBrush.Color = Color.Red;
                formGraphics.DrawEllipse(myPen, myx - 8, myy - 8, 16, 16);
                formGraphics.FillEllipse(myBrush, myx - 8, myy - 8, 16, 16);

            }
            foreach (int[] arr in edges)
            {
                string mys = reverse_points[arr[0]];
                string myf = reverse_points[arr[1]];
                string[] mysa = mys.Split(',');
                string[] myfa = myf.Split(',');
                int myxs = Int32.Parse(mysa[0]);
                int myys = Int32.Parse(mysa[1]);
                int myxf = Int32.Parse(myfa[0]);
                int myyf = Int32.Parse(myfa[1]);
                myPen.Color = Color.Red;
                formGraphics.DrawLine(myPen, myxs, myys, myxf, myyf);

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Redraw();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Redraw();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Application calculate center of drawed graph\nAfter drawing graph and clicking calculate, vertices that are center of graph will be marked with blue color.\nKliknięcie zapis/odczyt grafu powoduje zapisanie grafu do trzech plików lub odczytanie go (wszystkie pliki są wymagane)\nKliknięcie odśwież spowoduje przerysowanie grafu bez zaznaczonych wyliczonych punktów.\nZaznaczenie pola 'Nie twórz nowych wierzchołków' spowoduje zablokowanie grafu do istniejących wierzchołków i umożliwi rysowanie tylko krawędzi\nRysując każdą kolejną poza pierwszą krawędź należy zaczynać w istniejącym wierzchołku\n\nObecnie narysowana liczba wierzchołków: " + points.Count + "\nObecnie narysowana liczba krawędzi: " + edges.Count, "Pomoc i statystyki grafu");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void Undo()
        {
            UndoClass undo2 = new UndoClass(new List<int[]>(edges), new Dictionary<string, int>(points), new Dictionary<int, string>(reverse_points));
            points = undo.points;
            edges = undo.edges;
            reverse_points = undo.reverse_points;
            Redraw();
            undo = undo2;
        }
       
    }

    class UndoClass
    {
        public List<int[]> edges = new List<int[]>();
        public Dictionary<string, int> points = new Dictionary<string, int>();
        public Dictionary<int, string> reverse_points = new Dictionary<int, string>();

        public UndoClass(List<int[]> _edges, Dictionary<string, int> _points, Dictionary<int, string> _reverse_points)
        {
            edges = _edges;
            points = _points;
            reverse_points = _reverse_points;
        }
    }
}
