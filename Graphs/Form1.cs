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
        bool drag = false;
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

                Bitmap bmpPrevState = new Bitmap(splitContainer.Panel1.Width, splitContainer.Panel1.Height);
                Graphics.FromImage(bmpPrevState).CopyFromScreen(PointToScreen(splitContainer.Panel1.Location), new Point(0, 0), splitContainer.Panel1.Size);
                penPrevState = new Pen(new TextureBrush(bmpPrevState), 12);
                penPrevState.EndCap = System.Drawing.Drawing2D.LineCap.Round;

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

        private void fixedUpdate_Tick(object sender, EventArgs e)
        {
            if (prevMousePos != splitContainer.Panel1.PointToClient(MousePosition))
            {
                Graphics g = splitContainer.Panel1.CreateGraphics();
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawLine(penPrevState, pos, new Point(prevMousePos.X, prevMousePos.Y));
                prevMousePos = splitContainer.Panel1.PointToClient(MousePosition);
                Pen whtPen = new Pen(Color.White, 10);
                whtPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                g.DrawLine(whtPen, pos, prevMousePos);
            }
        }
    }

    public class Vertex
    {
        public int num;
        public Point pos;
        PictureBox dotka;
         
        public Vertex(Point pos, int num, Panel panel_one)
        {
            this.pos = pos;
            this.num = num;
            
            dotka = new PictureBox();
            dotka.Size = new Size(30, 30);
            dotka.Location = new Point(pos.X - 15, pos.Y - 15);
            dotka.SizeMode = PictureBoxSizeMode.StretchImage;
            dotka.Image = Properties.Resources.dot;
            panel_one.Controls.Add(dotka);
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
