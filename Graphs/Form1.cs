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
        Vertex origin; Vertex termin;
        public bool drag = false;
        Pen penPrevState;
        TextureBrush prevState;
        Point prevMousePos;
        Point pos;
        Vertex hVertex = null;
        Edge hEdge = null;
        Edge writingOn = null;

        Algorithms AlgObj = new Algorithms(); // mozna zauvazovat o tom, jestli neudelat Constructor, kterej by rovnou nebral matici sousednosti a convertil na seznam nasled
        string[] algoNames = new string[9] { "DFS", "BFS", "Dijkstra", "Bridges", "Components", "SSK", "FW", "Jarnik", "Topological Sort" };
        int alg = 0;


        public Form1()
        {
            InitializeComponent();
        }

        private void Clicked(object sender, MouseEventArgs e)
        {
            Graphics g = splitContainer.Panel1.CreateGraphics();
            if (KeyPreview) { StopWriting(); return; }
 
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

                vertices.Add(origin); // tvorit vertex pouze pokud je od ostatnich alespon nejak vzdaleny + vyresit přetékání labelů + hover weights a auto-resizing
                DrawVertex(g, pos, new Size(30, 30));
                matrix.Text = PrintMatrix(vertices.Count, adjMatrix);
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

        public string PrintMatrix(int vertex_count, int[,] matrix)
        {
            string m = "";
            for (int i = 0; i < vertex_count; i++)
                for (int j = 0; j < vertex_count; j++)
                    if (j < vertex_count - 1) m += $"{matrix[i, j]}  ";
                    else m += $"{matrix[i, j]}\n";
            return m;
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
            matrix.Text = PrintMatrix(vertices.Count, adjMatrix);
            PaintOver(splitContainer.Panel1.CreateGraphics());
            CapturePreviousState();
            dragUpdate.Start();
        }

        public void LeftClickedEdge(Edge sender, Point pos)
        {
            float fromV1 = EvalDistance(pos, sender.v1.pos);
            float fromV2 = EvalDistance(pos, sender.v2.pos);            

            if (Math.Min(fromV1, fromV2) < 40)
            {
                Graphics g = splitContainer.Panel1.CreateGraphics();
                Point pt; Vertex closerV;
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
                matrix.Text = PrintMatrix(vertices.Count, adjMatrix);
            }
            else
            {
                sender.weightLabel.Text = "";
                writingOn = sender;
                KeyPreview = true;
                writeTimer.Start();
            }        
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
            matrix.Text = PrintMatrix(vertices.Count, adjMatrix);
        }

        public void DeleteEdge(Edge sender)
        {
            AdjustMatrix(sender.v1.num, sender.v2.num, edges.Count - 1, 0);
            sender.oriented = false;
            splitContainer.Panel1.Controls.Remove(sender.weightLabel);
            edges.Remove(sender);
            PaintOver(splitContainer.Panel1.CreateGraphics());
            CapturePreviousState();
            matrix.Text = PrintMatrix(vertices.Count, adjMatrix);
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
                //hEdge.weightLabel.Size = new Size();
                if (hEdge.oriented) { DrawEdge(g, new Pen(Color.White, 7), hEdge.ptsArrow[0], hEdge.ptsArrow[1]);  DrawEdge(g, new Pen(Color.White, 7), hEdge.ptsArrow[0], hEdge.ptsArrow[2]);}                
            }
            else
            {
                if (hEdge != null)
                {
                    DrawEdge(g, penPrevState, hEdge.v1.pos, hEdge.v2.pos);
                }

                //edge.weightLabel.Size = new Size();
                DrawEdge(g, new Pen(Color.White, 13), edge.v1.pos, edge.v2.pos);
            }

            return edge;
        }

        public void StopWriting() // nemůžu mít obousměrny hrany (pouze neorientované -> změnit!)
        {
            KeyPreview = false;
            writeTimer.Stop();
            writingOn.weightLabel.Text = writingOn.weightLabel.Text.Replace("_", "");

            if (writingOn.weightLabel.Text == "") { writingOn.weightLabel.Text = Convert.ToString(writingOn.weight); return; }
            else writingOn.weight = int.Parse(writingOn.weightLabel.Text);

            if (writingOn.oriented == true)
            {
                if (writingOn.ptsArrow[0] == writingOn.v2.pos) adjMatrix[writingOn.v1.num - 1, writingOn.v2.num - 1] = writingOn.weight;
                else adjMatrix[writingOn.v2.num - 1, writingOn.v1.num - 1] = writingOn.weight;
            }
            else AdjustMatrix(writingOn.v1.num, writingOn.v2.num, edges.Count, writingOn.weight);

            matrix.Text = PrintMatrix(vertices.Count, adjMatrix);
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
        
        // weights //
        private void writeTimer_Tick(object sender, EventArgs e)
        {
            int lastIndex = writingOn.weightLabel.Text.Length - 1;
            if (writingOn.weightLabel.Text == "" || writingOn.weightLabel.Text[lastIndex] != '_') writingOn.weightLabel.Text += "_";
            else writingOn.weightLabel.Text = writingOn.weightLabel.Text.Replace("_", "");
        }
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            int i;
            //if (e.KeyChar == (char)13) label1.Text = "pressed enter key";

            if (int.TryParse(e.KeyChar.ToString(), out i)) writingOn.weightLabel.Text = writingOn.weightLabel.Text.Replace("_", "") + Convert.ToString(i);
            else StopWriting();
        }

        // algs //
        public void switchLabels()
        {
            switch (alg)
            {
                case 0: // DFS
                    algDescription.Text = "Depth - first search(DFS) is an algorithm for traversing or searching tree or graph data structures.The algorithm starts at the root node(selecting some arbitrary node as the root node in the case of a graph) and explores as far as possible along each branch before backtracking.";
                    labelSource.Show();
                    break;
                case 1: // BFS
                    algDescription.Text = "Breadth-first search (BFS) is an algorithm for searching a tree data structure for a node that satisfies a given property. It starts at the tree root and explores all nodes at the present depth prior to moving on to the nodes at the next depth level.";
                    break;
                case 2: // Dijkstra
                    algDescription.Text = "Given a graph and a source vertex in the graph, find the shortest paths from the source to all vertices in the given graph.";
                    labelSource.Show();
                    break;
                case 3: // Bridges
                    algDescription.Text = "An edge in an undirected connected graph is a bridge if removing it disconnects the graph. For a disconnected undirected graph, definition is similar, a bridge is an edge removing which increases number of disconnected components.";
                    labelSource.Hide();
                    break;
                case 4: // Komponenty
                    algDescription.Text = "A component of an undirected graph is a connected subgraph that is not part of any larger connected subgraph.";
                    break;
                case 5: // SSK
                    algDescription.Text = "A directed graph is strongly connected if there is a path between all pairs of vertices. A strongly connected component (SCC) of a directed graph is a maximal strongly connected subgraph.";
                    break;
                case 6: // FW
                    algDescription.Text = "The Floyd Warshall Algorithm is for solving the All Pairs Shortest Path problem. The problem is to find shortest distances between every pair of vertices in a given edge weighted directed Graph.";
                    // vysledna matice
                    break;
                case 7: // Jarnik
                    algDescription.Text = "In computer science, Jarnik's algorithm (also known as Prim's algorithm) is a greedy algorithm that finds a minimum spanning tree for a weighted undirected graph.";
                    // jak zobrazit minimalni kostru?
                    break;
                case 8: // Topological Sort
                    algDescription.Text = "Topological sorting for Directed Acyclic Graph (DAG) is a linear ordering of vertices such that for every directed edge u v, vertex u comes before v in the ordering.";
                    labelSource.Hide();
                    break;
            }
        }
        private void rightBtn_Click(object sender, EventArgs e)
        {
            alg = (alg + 1) % 9;
            algLabel.Text = algoNames[alg];
            switchLabels();
        }
        private void leftBtn_Click(object sender, EventArgs e)
        {
            alg -= 1;
            if (alg < 0) alg += 9;
            algLabel.Text = algoNames[alg];
            switchLabels();
        }
        private void startBtn_Click(object sender, EventArgs e)
        {
            switch (alg)
            {
                case 0: // DFS
                    AlgObj.DFS();
                    break;
                case 1: // BFS
                    AlgObj.BFS();
                    break;
                case 2: // Dijkstra
                    AlgObj.Dijkstra(adjMatrix, 1);
                    break;
                case 3: // Bridges
                    AlgObj.Bridges();
                    break;
                case 4: // Komponenty
                    AlgObj.Components();
                    break;
                case 5: // SSK
                    AlgObj.SSK();
                    break;
                case 6: // FW
                    resultMatrix.Text = PrintMatrix(vertices.Count, AlgObj.FW(adjMatrix)); // poresit jak vyhandlovat nekonečna
                    break;
                case 7: // Jarnik
                    AlgObj.Jarnik();
                    break;
                case 8: // Topological Sort
                    AlgObj.TopologicalSort();
                    break;

            // checknout správnosti algoritmů
            }
        }
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
            weightLabel.Size = new Size(20, 20);
            weightLabel.Text = "1";
        }
    }

    public class Heap
    {
        public List<(int, int)> heap = new List<(int, int)>{ (0, 0) };

        public void Insert((int, int) element)
        {
            heap.Add(element);
            int j = heap.Count - 1;
            BubbleUp(j);
        }

        public void BubbleUp(int j)
        {
            while (j > 1 && heap[j].Item2 < heap[j / 2].Item2)
            {
                (int, int) temp = heap[j];
                heap[j] = heap[j / 2]; heap[j / 2] = temp;
                j = j / 2;
            }
        }

        public int ExtractMin()
        {
            if (heap.Count == 1) return -1;

            int zrus = heap[1].Item1;
            heap[1] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            int j = 1;
            while (2 * j < heap.Count)
            {
                int n = 2 * j;
                if (n < heap.Count - 1) {
                    if (heap[n + 1].Item2 < heap[n].Item2) n += 1;

                    if (heap[j].Item2 > heap[n].Item2)
                    {
                        (int, int) temp = heap[j];
                        heap[j] = heap[n]; heap[n] = temp;
                        j = n;
                    }
                    else break;
                }
            }

            return zrus;
        }

        public void DecreaseKey(int key, int value)
        {
            int index = -1;

            for (int i = 0; i < heap.Count; i++) // nejak chytřeji - tohle by srazilo na O(n)
            {
                if (heap[i].Item1 == key) { index = i; break; }
            }

            if (index == -1) return;
            heap[index] = (key, value);

            BubbleUp(index);
        }
    }

    public class Algorithms
    {
        const int infty = 999999;

        public bool relax(int u, int v, int[,] weight, ref int[] dist, ref int[] prev)
        {
            if (dist[u] + weight[u, v] < dist[v])
            {
                dist[v] = dist[u] + weight[u, v];
                prev[v] = u;
                return true;
            }

            return false;
        }

        public int[][] seznamNasledniku(int[,] adjMatrix)
        {
            int size = adjMatrix.GetLength(0);
            int[][] jag = new int[size][];
            List<int> temp;

            for (int i = 0; i < size; i++)
            {
                temp = new List<int>();

                for (int j = 0; j < size; j++)
                {
                    if (adjMatrix[i, j] != 0) temp.Add(adjMatrix[i, j]);
                }

                jag[i] = new int[temp.Count];

                for (int k = 0; k < temp.Count; k++)
                {
                    jag[i][k] = temp[k];
                }
            }

            return jag;
        }

        public void DFS() { }

        public void BFS() { }

        public Tuple<int[], int[]> Dijkstra (int[,] adjMatrix, int source)
        {
            int[][] naslednici = seznamNasledniku(adjMatrix);
            int[] dist = new int[adjMatrix.GetLength(0)];
            int[] prev = new int[adjMatrix.GetLength(0)];
            Heap Q = new Heap();

            for (int v = 0; v < adjMatrix.GetLength(0); v++)
            {
                if (v != source) dist[v] = infty;
                Q.Insert((v, dist[v]));
            }
                
            while (Q.heap.Count != 1)
            {
                int u = Q.ExtractMin();
                foreach (int neigh in naslednici[u])
                {
                    if (relax(u, neigh, adjMatrix, ref dist, ref prev)) Q.DecreaseKey(neigh, dist[neigh]);
                }
            }

            return new Tuple<int[], int[]>(dist, prev);
        } // nemuzu zapor hrany

        public void Bridges() { }

        public void Components() { }

        public void SSK() { }

        public int[,] FW (int[,] adjMatrix)
        {
            int size = adjMatrix.GetLength(0);
            int[,] D = new int[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (adjMatrix[i, j] == 0 && i != j) D[i, j] = infty;
                    else D[i, j] = adjMatrix[i, j];
                }
            }

            for (int k = 0; k < size; k++)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (D[i, k] + D[k, j] < D[i, j])
                        {
                            D[i, j] = D[i, k] + D[k, j];
                        }
                    }
                }
            }

            return D;
        }

        public void Jarnik() { }

        public void TopologicalSort() { }
    }
}
