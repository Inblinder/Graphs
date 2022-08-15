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
        List<Label> weightLabels = new List<Label>();
        int[,] adjMatrix = new int[8, 8];       
        Vertex origin; Vertex termin;
        public bool drag = false;
        Pen penPrevState;
        TextureBrush prevState;
        Point prevMousePos;
        Point pos;
        Vertex hVertex = null;
        Edge hEdge = null;

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
                    termin = new Vertex(pos, vertices.Count + 1);
                    InitEdge(origin, termin);
                    AdjustMatrix(origin.num, termin.num, edges.Count, 1);
                    origin = termin;
                }
                else
                {
                    if (hEdge != null) { LeftClickedEdge(hEdge, pos); return; }
                    origin = new Vertex(pos, vertices.Count + 1);
                    drag = true;
                }

                vertices.Add(origin); // tvorit vertex pouze pokud je od ostatnich alespon nejak vzdaleny
                DrawVertex(g, pos, new Size(30, 30));
                PrintMatrix(vertices.Count);
                CapturePreviousState();
                dragUpdate.Start();
            }
            else
            {
                if (drag == false)
                {
                    if (hVertex != null) { DeleteVertex(hVertex); return; }
                    if (hEdge != null) { DeleteEdge(hEdge); return; }
                }
                else
                {
                    dragUpdate.Stop();
                    drag = false;
                    DrawEdge(g, penPrevState, pos, prevMousePos);
                }
            }
        }

        public void AdjustMatrix(int v_1, int v_2, int edge_count, int val, bool orient = false)
        {
            int size = adjMatrix.GetLength(0);
            if (vertices.Count >= size)
            {
                adjMatrix = new int[size * 2, size * 2];
                for (int i = 0; i < edge_count; i++)
                {
                    if (edges[i].oriented == true)
                    {
                        if (edges[i].ptsArrow[0] == edges[i].v2.pos) adjMatrix[edges[i].v1.num - 1, edges[i].v2.num - 1] = edges[i].weight;
                        else adjMatrix[edges[i].v2.num - 1, edges[i].v1.num - 1] = edges[i].weight;
                    }
                    else
                    {
                        adjMatrix[edges[i].v1.num - 1, edges[i].v2.num - 1] = edges[i].weight;
                        adjMatrix[edges[i].v2.num - 1, edges[i].v1.num - 1] = edges[i].weight;
                    }
                }
            }
            
            adjMatrix[v_1 - 1, v_2 - 1] = val;
            if (orient == false) adjMatrix[v_2 - 1, v_1 - 1] = val;
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

        public void InitEdge(Vertex v1, Vertex v2)
        {
            Edge edge = new Edge(v1, v2, GetTheMiddle(v1.pos, v2.pos));
            splitContainer.Panel1.Controls.Add(edge.weightLabel);
            edges.Add(edge);
        }

        public void CapturePreviousState()
        {
            TextureBrush TakeAScreenshot()
            {
                Bitmap bmpPrevState = new Bitmap(splitContainer.Panel1.Width, splitContainer.Panel1.Height);
                Graphics.FromImage(bmpPrevState).CopyFromScreen(PointToScreen(splitContainer.Panel1.Location), new Point(0, 0), splitContainer.Panel1.Size);
                return new TextureBrush(bmpPrevState);
            }

            prevState = TakeAScreenshot();
            penPrevState = new Pen(prevState, 15);
        }      

        public float EvalDistance(Point a, Point b)
        {
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        public void ScaleVector(int mag, ref int u1, ref int u2)
        {
            float k = (float)Math.Sqrt(mag*mag / ((float)u1*u1 + (float)u2*u2));
            u1 = (int)(k * u1); u2 = (int)(k * u2);
        }

        public Point GetTheMiddle(Point a, Point b)
        {
            Point vektor = new Point((b.X - a.X) / 2, (b.Y - a.Y) / 2);
            if (EvalDistance(new Point(a.X + vektor.X, a.Y + vektor.Y), b) < EvalDistance(a, b)) return new Point(a.X + vektor.X, a.Y + vektor.Y);
            else return new Point(a.X - vektor.X, a.Y - vektor.Y);
        }

        public void LeftClickedVertex(Vertex sender)
        {
            if (drag == true)
            {
                foreach (Edge edge in edges)
                    if (edge.v1 == origin && edge.v2 == sender || edge.v2 == origin && edge.v1 == sender) { return; } // nefunguje
                
                InitEdge(origin, sender);
                AdjustMatrix(origin.num, sender.num, edges.Count, 1);
            }
            else { drag = true; }

            origin = sender;
            pos = sender.pos;
            PrintMatrix(vertices.Count());
            PaintOver(splitContainer.Panel1.CreateGraphics()); // mozna se zamyslet jestli to tam nechat
            CapturePreviousState();
            dragUpdate.Start();
        }

        public void LeftClickedEdge(Edge sender, Point pos)
        {
            float fromV1 = EvalDistance(pos, sender.v1.pos);
            float fromV2 = EvalDistance(pos, sender.v2.pos);
            Vertex closerV;

            if (Math.Min(fromV1, fromV2) < 40)
            {
                Graphics g = splitContainer.Panel1.CreateGraphics();
                Point pt;
                int u1 = sender.v1.pos.X - sender.v2.pos.X;
                int u2 = sender.v1.pos.Y - sender.v2.pos.Y;
                ScaleVector(35, ref u1, ref u2);
                float lengthOfEdge = EvalDistance(sender.v1.pos, sender.v2.pos);

                if (fromV1 < fromV2)
                {
                    closerV = sender.v1;
                    if (EvalDistance(new Point(closerV.pos.X + u1, closerV.pos.Y + u2), sender.v2.pos) < lengthOfEdge) pt = new Point(closerV.pos.X + u1, closerV.pos.Y + u2);
                    else pt = new Point((closerV.pos.X - u1), (closerV.pos.Y - u2));
                }
                else
                {
                    closerV = sender.v2;
                    if (EvalDistance(new Point(closerV.pos.X + u1, closerV.pos.Y + u2), sender.v1.pos) < lengthOfEdge) pt = new Point(closerV.pos.X + u1, closerV.pos.Y + u2);
                    else pt = new Point((closerV.pos.X - u1), (closerV.pos.Y - u2));
                }

                ScaleVector(15, ref u1, ref u2);

                if (sender.oriented == true)
                { 
                    sender.oriented = false;
                    AdjustMatrix(sender.v1.num, sender.v2.num, edges.Count, 1);
                }
                else
                {                  
                    sender.oriented = true;
                    sender.ptsArrow = new Point[3] { closerV.pos, new Point(pt.X - u2, pt.Y + u1), new Point(pt.X + u2, pt.Y - u1) };
                    if (closerV == sender.v1) AdjustMatrix(sender.v1.num, sender.v2.num, edges.Count, 0, true);
                    else AdjustMatrix(sender.v2.num, sender.v1.num, edges.Count, 0, true);
                }

                PaintOver(g);
                CapturePreviousState();
            }
            else
            {
                 //doplnit
            }

            PrintMatrix(vertices.Count);
        }

        public void DeleteVertex(Vertex sender)
        {
            for (int i = edges.Count - 1; i >= 0; i--)
                if (edges[i].v1 == sender || edges[i].v2 == sender) { splitContainer.Panel1.Controls.Remove(edges[i].weightLabel); edges.RemoveAt(i); }

                    int removedNum = sender.num;
            vertices.Remove(sender);

            foreach (Vertex vertex in vertices)
                if (vertex.num > removedNum) vertex.num -= 1;

            PaintOver(splitContainer.Panel1.CreateGraphics());
            CapturePreviousState();
            PrintMatrix(vertices.Count());
        }

        public void DeleteEdge(Edge sender)
        {
            AdjustMatrix(sender.v1.num, sender.v2.num, edges.Count - 1, 0);
            //sender.oriented = false;
            splitContainer.Panel1.Controls.Remove(sender.weightLabel);
            edges.Remove(sender);
            PaintOver(splitContainer.Panel1.CreateGraphics());
            CapturePreviousState();
            PrintMatrix(vertices.Count());
        }

        public void PaintOver(Graphics g)
        {
            SolidBrush brush = new SolidBrush(Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(28)))), ((int)(((byte)(36))))));
            g.FillRectangle(brush, new Rectangle(0, 0, splitContainer.Panel1.Width, splitContainer.Panel1.Height));
            RedrawEdges(g);
            RedrawVertices(g);
        }

        public void RedrawEdges(Graphics g)
        {
            foreach (Edge edge in edges)
            { 
                DrawEdge(g, new Pen(Color.White, 10), edge.v1.pos, edge.v2.pos);
                if (edge.oriented)
                {
                    DrawEdge(g, new Pen(Color.White, 7), edge.ptsArrow[0], edge.ptsArrow[1]);
                    DrawEdge(g, new Pen(Color.White, 7), edge.ptsArrow[0], edge.ptsArrow[2]);
                }
            }
        }

        public void RedrawVertices(Graphics g)
        {
            foreach (Vertex vertex in vertices)
            {
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

        public Edge GetHoveredEdge(Point mPos)
        {
            float a; float b; float c; float dist; float denom; float distToMouseA; float distToMouseB;

            foreach(Edge edge in edges)
            {               
                a = edge.v2.pos.Y - edge.v1.pos.Y;
                b = -edge.v2.pos.X + edge.v1.pos.X;
                c = -a * edge.v1.pos.X - b * edge.v1.pos.Y;

                denom = (float)Math.Sqrt(a * a + b * b);
                dist = Math.Abs(a * mPos.X + b * mPos.Y + c) / denom;
                if (dist > 5) continue;

                distToMouseA = EvalDistance(edge.v1.pos, mPos);
                distToMouseB = EvalDistance(edge.v2.pos, mPos);

                if (denom > distToMouseA && denom > distToMouseB)
                    return edge;
            }

            return null;
        }

        public Vertex HoveredV(Vertex vertex)
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

        public Edge HoveredE(Edge edge)
        {
            if (drag == true || edge == hEdge) return edge;
            Graphics g = splitContainer.Panel1.CreateGraphics();

            if (edge == null)
            {
                DrawEdge(g, penPrevState, hEdge.v1.pos, hEdge.v2.pos);
                //hEdge.AdjustLabel(20, hEdge.weight);
                if (hEdge.oriented) { DrawEdge(g, new Pen(Color.White, 7), hEdge.ptsArrow[0], hEdge.ptsArrow[1]);  DrawEdge(g, new Pen(Color.White, 7), hEdge.ptsArrow[0], hEdge.ptsArrow[2]);}                
            }
            else
            {
                if (hEdge != null)
                {
                    DrawEdge(g, penPrevState, hEdge.v1.pos, hEdge.v2.pos);
                }

                //edge.AdjustLabel(30, edge.weight);
                DrawEdge(g, new Pen(Color.White, 13), edge.v1.pos, edge.v2.pos);
            }

            return edge;
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
            hVertex = HoveredV(GetHoveredVertex(mPos));
            if (hVertex != null && hEdge == null) return;
            hEdge = HoveredE(GetHoveredEdge(mPos));
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
        public Vertex v1;
        public Vertex v2;
        public bool oriented = false;
        public Point[] ptsArrow;
        public int weight = 1;
        public Label weightLabel;

        public Edge(Vertex v1, Vertex v2, Point loc)
        {
            this.v1 = v1;
            this.v2 = v2;

            weightLabel = new Label();
            weightLabel.Location = loc;
            weightLabel.ForeColor = Color.White;
            AdjustLabel(20, 1);
        }

        public void AdjustLabel(int scale, int w) // tuhle funkci predelat
        {
            weight = w;
            weightLabel.Text = $"{w}";
            //weightLabel.Font = new Font(SystemFonts.DefaultFont, );
            weightLabel.Size = new Size(scale, scale);
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
