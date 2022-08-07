using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphs
{
    public partial class Form1 : Form
    {
        List<Vertex> vertices = new List<Vertex>();
        List<Edge> edges = new List<Edge>();
        int[,] adjMatrix = new int[8, 8];
        int origin;
        int termin;
        public bool drag = false;
        Pen penPrevState;
        Point prevMousePos;
        Point pos;

        public Form1()
        {
            InitializeComponent();
        }

        public void CreateVertex(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pos = e.Location;

                if (drag)
                {
                    termin = vertices.Count + 1;
                    edges.Add(new Edge(origin, termin, edges.Count + 1));
                    AdjustMatrix(origin, termin, edges.Count);
                    origin = termin;
                }
                else
                {
                    origin = vertices.Count + 1;
                    drag = true;
                }

                vertices.Add(new Vertex(pos, vertices.Count + 1, splitContainer.Panel1));
                PrintMatrix(vertices.Count);
                TakeAScreenshot();

                fixedUpdate.Start();
            }
            else
            {
                fixedUpdate.Stop();
                Graphics g = splitContainer.Panel1.CreateGraphics();
                g.DrawLine(penPrevState, pos, prevMousePos);
                drag = false;
            } 
        }

        public void AdjustMatrix(int v_1, int v_2, int edge_count)
        {
            int size = adjMatrix.GetLength(0);
            if (vertices.Count >= size)
            {
                adjMatrix = new int[size * 2, size * 2];
                for (int i = 0; i < edge_count; i++) { adjMatrix[edges[i].v_1 - 1, edges[i].v_2 - 1] = 1; adjMatrix[edges[i].v_2 - 1, edges[i].v_1 - 1] = 1; }
            }
            
            adjMatrix[v_1 - 1, v_2 - 1] = 1; adjMatrix[v_2 - 1, v_1 - 1] = 1;
        }

        public void PrintMatrix(int vertex_count)
        {
            string m = "";
            for (int i = 0; i < vertex_count; i++)
                for (int j = 0; j < vertex_count; j++)
                    if (j < vertex_count - 1) m += $"{adjMatrix[i, j]}  ";
                    else m += $"{adjMatrix[i, j]}\n";
            matrix.Text = m;
        }

        public void TakeAScreenshot()
        {
            Bitmap bmpPrevState = new Bitmap(splitContainer.Panel1.Width, splitContainer.Panel1.Height);
            Graphics.FromImage(bmpPrevState).CopyFromScreen(PointToScreen(splitContainer.Panel1.Location), new Point(0, 0), splitContainer.Panel1.Size);
            penPrevState = new Pen(new TextureBrush(bmpPrevState), 15);
            penPrevState.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        }

        public static void Clicked(Vertex sender)
        {
            if (Program.f1.drag == true)
            {
                Program.f1.edges.Add(new Edge(Program.f1.origin, sender.num, Program.f1.edges.Count + 1));                
                Program.f1.AdjustMatrix(Program.f1.origin, sender.num, Program.f1.edges.Count);                
            }
            else {Program.f1.drag = true;}

            Program.f1.origin = sender.num;
            Program.f1.pos = sender.pos;
            Program.f1.PrintMatrix(Program.f1.vertices.Count());
            Program.f1.TakeAScreenshot();
            Program.f1.fixedUpdate.Start();
        }

        public void CheckIfHovered(ref Vertex hovered, ref bool connect)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].hover == true)
                {
                    hovered = vertices[i];
                    connect = true;
                }
            }
        }

        private void fixedUpdate_Tick(object sender, EventArgs e)
        {
            Vertex hovered = null;
            bool connect = false;
            CheckIfHovered(ref hovered, ref connect);

            if (prevMousePos != splitContainer.Panel1.PointToClient(MousePosition)) //&& prevMousePos != hovered.pos)
            {
                Graphics g = splitContainer.Panel1.CreateGraphics();
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawLine(penPrevState, pos, new Point(prevMousePos.X, prevMousePos.Y));
                Pen whtPen = new Pen(Color.White, 10);
                whtPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                if (connect == true) { prevMousePos = hovered.pos; g.DrawLine(whtPen, pos, hovered.pos); }
                else { prevMousePos = splitContainer.Panel1.PointToClient(MousePosition); g.DrawLine(whtPen, pos, prevMousePos); }
            }
        }
    }

    public class Vertex
    {
        public int num;
        public Point pos;
        PictureBox dotka;
        public bool hover = false;
         
        public Vertex(Point pos, int num, Panel panel_one)
        {
            this.pos = pos;
            this.num = num;
            
            dotka = new PictureBox();
            dotka.Size = new Size(30, 30);
            dotka.Location = new Point(pos.X - 15, pos.Y - 15);
            dotka.SizeMode = PictureBoxSizeMode.StretchImage;
            dotka.Image = Properties.Resources.dot;
            dotka.MouseEnter += Dotka_MouseEnter;
            dotka.MouseLeave += Dotka_MouseLeave;
            dotka.MouseClick += Dotka_MouseClick;
            dotka.Tag = this;
            panel_one.Controls.Add(dotka);
        }

        private void Dotka_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Form1.Clicked((Vertex)((PictureBox)sender).Tag);
            }
            else
            {
                //if () dotka.Dispose();
            }
        }

        private void Dotka_MouseEnter(object sender, EventArgs e)
        {
            hover = true;
            dotka.Location = new Point(dotka.Location.X - 3, dotka.Location.Y - 3);
            dotka.Size = new Size(35, 35);
        }

        private void Dotka_MouseLeave(object sender, EventArgs e)
        {
            dotka.Location = new Point(dotka.Location.X + 3, dotka.Location.Y + 3);
            dotka.Size = new Size(30, 30);
            hover = false;
        }
    }

    public class Edge
    {
        private int num;
        public int v_1;
        public int v_2;

        public Edge(int v_1, int v_2, int num)
        {
            this.v_1 = v_1;
            this.v_2 = v_2;
            this.num = num;
        }
    }
}
