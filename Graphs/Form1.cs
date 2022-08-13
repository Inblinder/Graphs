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
        TextureBrush prevState;
        Point prevMousePos;
        Point pos;
        Vertex hVertex = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Clicked(object sender, MouseEventArgs e)
        {
            Graphics g = splitContainer.Panel1.CreateGraphics();          

            if (e.Button == MouseButtons.Left)
            {
                pos = e.Location;

                if (hVertex != null) { LeftClickedVertex(hVertex); return; }

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

                vertices.Add(new Vertex(pos, vertices.Count + 1));
                DrawVertex(g, pos, new Size(30, 30));
                PrintMatrix(vertices.Count);
                prevState = TakeAScreenshot();
                penPrevState = new Pen(prevState, 15);
                dragUpdate.Start();
            }
            else
            {
                if (hVertex != null) { DeleteVertex(hVertex); return; }

                dragUpdate.Stop();
                drag = false;
                DrawEdge(g, penPrevState, pos, prevMousePos);
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

        public string MatrixToString(int vertex_count)
        {
            string m = "[";
            for (int i = 0; i < vertex_count; i++)
            {
                m += $"[";
                for (int j = 0; j < vertex_count - 1; j++)
                    m += $"{adjMatrix[i, j]},";

                if (i == vertex_count - 1) m += $"{adjMatrix[i, vertex_count - 1]}]]";
                else m += $"{adjMatrix[i, vertex_count - 1]}],";
            }

            return m;
        }

        public TextureBrush TakeAScreenshot()
        {
            Bitmap bmpPrevState = new Bitmap(splitContainer.Panel1.Width, splitContainer.Panel1.Height);
            Graphics.FromImage(bmpPrevState).CopyFromScreen(PointToScreen(splitContainer.Panel1.Location), new Point(0, 0), splitContainer.Panel1.Size);
            return new TextureBrush(bmpPrevState);
        }

        public void LeftClickedVertex(Vertex sender)
        {
            if (drag == true)
            {
                edges.Add(new Edge(origin, sender.num, edges.Count + 1));
                AdjustMatrix(origin, sender.num, edges.Count);
            }
            else drag = true;

            origin = sender.num;
            pos = sender.pos;
            PrintMatrix(vertices.Count());
            prevState = TakeAScreenshot();
            penPrevState = new Pen(prevState, 15);
            dragUpdate.Start();
        }

        public void DeleteVertex(Vertex sender)
        {
            if (drag != true)
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    if (edges[i].v_1 == sender.num || edges[i].v_2 == sender.num)
                    {
                        edges.RemoveAt(i); // mozna jinak zaznamenavat edges abych zlepsil cas
                       // mazat edges
                    }
                }

                // přemazat vertex
                vertices.Remove(sender);
                // zmenit cisla vrcholů
                RedrawEdges();
                PrintMatrix(vertices.Count());
            }
        }

        public void RedrawEdges()
        {
            Graphics g = splitContainer.Panel1.CreateGraphics();

            foreach (Edge edge in edges) DrawEdge(g, new Pen(Color.White, 10), edge.pos1, edge.pos2);
        }

        public void RedrawVertices()
        {
            Graphics g = splitContainer.Panel1.CreateGraphics();

            foreach (Vertex vertex in vertices)
            {
                //DrawVertex(g, hVertex.pos, new Size(42, 42), prevState);
                DrawVertex(g, vertex.pos, new Size(30, 30));
            }
        }

        public void DrawEdge(Graphics g, Pen pen, Point pt1, Point pt2)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            g.DrawLine(pen, pt1, pt2);
        }

        public void DrawVertex(Graphics g, Point location, Size size, TextureBrush brush = null)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            if (brush != null) g.FillEllipse(brush, new Rectangle(location.X - (size.Width / 2), location.Y - (size.Height / 2), size.Width, size.Height));
            else g.FillEllipse(Brushes.White, new Rectangle(location.X - (size.Width / 2), location.Y - (size.Height / 2), size.Width, size.Height));
        }

        public Vertex GetHoveredVertex(Point mPos)
        {
            foreach (Vertex vertex in vertices)
            {
                if (Math.Sqrt((mPos.X - vertex.pos.X) * (mPos.X - vertex.pos.X) + (mPos.Y - vertex.pos.Y) * (mPos.Y - vertex.pos.Y)) <= vertex.radius)
                    return vertex;
            }

            return null;
        }

        public Vertex Hovered(Vertex vertex)
        {
            if (vertex == hVertex) return vertex;
            Graphics g = splitContainer.Panel1.CreateGraphics();

            if (vertex == null)
            {
                hVertex.radius = 15;
                DrawVertex(g, hVertex.pos, new Size(42, 42), prevState);
            }
            else
            {
                if (hVertex != null)
                {
                    hVertex.radius = 15;
                    DrawVertex(g, hVertex.pos, new Size(42, 42), prevState);
                }
                vertex.radius = 20;
                DrawVertex(g, vertex.pos, new Size(40, 40));
            }

            return vertex;
        }

        private void dragUpdate_Tick(object sender, EventArgs e)
        {
            if (prevMousePos != splitContainer.Panel1.PointToClient(MousePosition))
            {
                Graphics g = splitContainer.Panel1.CreateGraphics();
                Pen whtPen = new Pen(Color.White, 10);

                if (hVertex != null)
                { 
                    if (hVertex.pos != prevMousePos)
                    {
                        DrawEdge(g, penPrevState, pos, new Point(prevMousePos.X, prevMousePos.Y));
                        DrawVertex(g, hVertex.pos, new Size(40, 40));
                        prevMousePos = hVertex.pos;
                    }
                }
                else { DrawEdge(g, penPrevState, pos, new Point(prevMousePos.X, prevMousePos.Y)); prevMousePos = splitContainer.Panel1.PointToClient(MousePosition); }

                DrawEdge(g, whtPen, pos, prevMousePos);
            }
        }

        private void fixedUpdate_Tick(object sender, EventArgs e)
        {
            Point mPos = splitContainer.Panel1.PointToClient(MousePosition);
            hVertex = Hovered(GetHoveredVertex(mPos));

            // nebo na jedné z edges
            foreach (Edge edge in edges) { }
        }

        private void fixedUpdate_Start(object sender, EventArgs e) { fixedUpdate.Start(); }
        private void fixedUpdate_End(object sender, EventArgs e) { fixedUpdate.Stop(); }
        private void copyBtn_Click(object sender, EventArgs e) { Clipboard.SetText(MatrixToString(vertices.Count)); }
    }

    public class Vertex
    {
        public int num;
        public int radius = 15;
        public Point pos;
         
        public Vertex(Point pos, int num)
        {
            this.pos = pos;
            this.num = num;
        }
    }

    public class Edge
    {
        private int num;
        public int v_1;
        public int v_2;
        public Point pos1;
        public Point pos2;

        public Edge(int v_1, int v_2, int num)
        {
            this.v_1 = v_1;
            this.v_2 = v_2;
            this.num = num;
        }
    }

    public class Algorithms
    {
        void DFS() { }

        void BFS() { }

        void Dijkstra() { }

        void Bridges() { }

        void Komponenty() { }

        void SSK() { }

        void FW() { }

        void Jarnik() { }

        void TopologicalSort() { }
    }
}
