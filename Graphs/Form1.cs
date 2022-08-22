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
        int[,] adjMatrix = new int[8, 8]; // matice sousednosti
        Vertex origin; Vertex termin; // počátek a konec hran
        public bool drag = false;
        Pen penPrevState; TextureBrush prevState; // pera pro přemalovávání na předchozí stav
        Point prevMousePos; Point pos; 
        Vertex hVertex = null; Edge hEdge = null; // hovered objekty

        Edge writingOn = null; // hrana, kde přepisuji její váhu
        bool writingOnSecondWeight = false;

        Algorithms AlgObj;
        string[] algoNames = new string[9] { "DFS", "BFS", "Dijkstra", "Bridges", "Components", "SSK", "FW", "Jarnik", "Topological Sort" };
        int alg = 0; // na kolikátém algoritmu v pořadí zrovna jsem
        const int infty = 999999;

        public Form1()
        {
            InitializeComponent();
            this.Resize += Form1_Resize;
            this.LocationChanged += Form1_LocationChanged;
        }

        /************* DRIVING METHOD **************/
        private void Clicked(object sender, MouseEventArgs e) // spustí se po kliknutí na Panel1
        {
            Graphics g = splitContainer.Panel1.CreateGraphics();
            if (KeyPreview) { StopWriting(); return; } // pokud jsem zrovna přepisoval váhu hrany
 
            if (e.Button == MouseButtons.Left)
            {
                if (hVertex != null) // pokud mám myš na vrcholu
                {
                    if (drag)
                        foreach (Edge edge in edges) // zkontroluji, jestli už taková hrana neexistuje
                            if (edge.v1 == origin && edge.v2 == hVertex || edge.v2 == origin && edge.v1 == hVertex) return;

                    pos = e.Location;
                    LeftClickedVertex(hVertex); // dokončím, nebo začnu tovřit novou hranu z hovered vrcholu
                    return;
                }

                pos = e.Location;

                if (drag) // pokud jsem zrovna táhnul hranu
                {
                    termin = new Vertex(pos, vertices.Count + 1); // nastavím konečný vrchol na pozici myši
                    InitEdge(origin, termin); // vytvořím hranu
                    AdjustMatrix(origin.num, termin.num, 1); // a předělám matici
                    origin = termin; // přenastavím počáteční vrchol na konečný a z něho táhnu hranu
                }
                else
                {
                    if (hEdge != null) { LeftClickedEdge(hEdge, pos); return; } // pokud existovala hovered hrana
                    origin = new Vertex(pos, vertices.Count + 1); // nastavím počáteční vrchol hrany
                    drag = true; // tažení na true
                }

                vertices.Add(origin);
                comboVertices.Items.Add(origin.num); // přidám nový vrchol do source možností
                DrawVertex(g, pos, new Size(30, 30));
                CheckMatrixSize(); // zkontroluji, jestli mi nepřeteče matice
                matrix.Text = PrintMatrix(vertices.Count, adjMatrix); // vypíšu ji
                CapturePreviousState(); // zaznamenám stav obrazovky, který budu používat na přemalování
                dragUpdate.Start(); // spustím přemalovávání hrany, která má konec v pozici myši
            }
            else
            {
                if (drag == false)
                {
                    if (hVertex != null) { DeleteVertex(hVertex); return; } // pokud jsem klikl na hovered vertex, nebo
                    if (hEdge != null) { DeleteEdge(hEdge); return; } // na hovered edge, jiným než levým, odstraním
                }
                else
                {
                    dragUpdate.Stop(); // zastavím tažení hrany
                    drag = false;
                    DrawEdge(g, penPrevState, pos, prevMousePos);
                }
            }
        }

        /************* ALL ABOUT MATRIX **************/
        public void AdjustMatrix(int v_1, int v_2, int val, bool orient = false) // přepíšu hodnotu v matici sousednosti
        {
            adjMatrix[v_1 - 1, v_2 - 1] = val;
            if (orient == false) adjMatrix[v_2 - 1, v_1 - 1] = val;
        }

        public void CheckMatrixSize() // zkontroluji, jestli matice nepřetéká, pokud ano, dvakrát ji zvětším a přepíšu její hodnoty
        {
            int size = adjMatrix.GetLength(0);
            if (vertices.Count >= size)
            {
                adjMatrix = new int[size * 2, size * 2];
                for (int i = 0; i < edges.Count; i++)
                {
                    if (edges[i].oriented == true)
                    {
                        if (edges[i].doubleOriented == true)
                        {
                            if (edges[i].ptsArrow[0] == edges[i].v2.pos) { adjMatrix[edges[i].v1.num - 1, edges[i].v2.num - 1] = edges[i].weight; adjMatrix[edges[i].v2.num - 1, edges[i].v1.num - 1] = edges[i].weightSnd; }
                            else { adjMatrix[edges[i].v2.num - 1, edges[i].v1.num - 1] = edges[i].weight; adjMatrix[edges[i].v1.num - 1, edges[i].v2.num - 1] = edges[i].weightSnd; }
                        }
                        else
                        {
                            if (edges[i].ptsArrow[0] == edges[i].v2.pos) adjMatrix[edges[i].v1.num - 1, edges[i].v2.num - 1] = edges[i].weight;
                            else adjMatrix[edges[i].v2.num - 1, edges[i].v1.num - 1] = edges[i].weight;
                        }
                    }
                    else
                    {
                        adjMatrix[edges[i].v1.num - 1, edges[i].v2.num - 1] = edges[i].weight;
                        adjMatrix[edges[i].v2.num - 1, edges[i].v1.num - 1] = edges[i].weight;
                    }
                }
            }
        }

        public string PrintMatrix(int vertex_count, int[,] matrix) // vypíšu matici do stringu s respektem k nekonečnu
        {
            string m = "";
            for (int i = 0; i < vertex_count; i++)
                for (int j = 0; j < vertex_count; j++)
                    if (j < vertex_count - 1)
                    {
                        if (matrix[i, j] < infty - 10) m += $"{matrix[i, j]}  ";
                        else m += "inf  ";
                    }
                    else
                    {
                        if (matrix[i, j] < infty - 10) m += $"{matrix[i, j]}\n";
                        else m += "inf\n";
                    }
            return m;
        }

        public void InsertMatrix(string strMatrix) // funkce, která zpracuje string z boxu pro importování matic
        {
            void import(int size, List<int> numbers) // přepíšu čísla do validní matice sousednosti
            {
                adjMatrix = new int[size, size];
                for (int i = edges.Count - 1; i >= 0; i--) { splitContainer.Panel1.Controls.Remove(edges[i].weightLabel); edges[i] = null;} // odstraním labely vah hran
                vertices = new List<Vertex>();
                edges = new List<Edge>();
                Random r = new Random();

                for (int k = 0; k < size; k++) // každému vrcholu dám random pozici s podmínkou, že každý vrchol musí být od ostatních vzdálený alespoň na určitou vzdálenost
                {
                    Point randPos = new Point(r.Next(150, splitContainer.Panel1.Width - 150), r.Next(150, splitContainer.Panel1.Height - 150));
                    while (validDistances(randPos, 150) == false) randPos = new Point(r.Next(150, splitContainer.Panel1.Width - 150), r.Next(150, splitContainer.Panel1.Height - 150));
                    vertices.Add(new Vertex(randPos, vertices.Count + 1));

                }

                for (int i = 0; i < size; i++)
                {
                    int j = 0;

                    while (j < i)  // zapíšu čísla do matice sousednosti a přidám hrany
                    {
                        adjMatrix[i, j] = numbers[i * size + j]; adjMatrix[j, i] = numbers[j * size + i]; 

                        if (adjMatrix[i, j] != adjMatrix[j, i])
                        {
                            if (adjMatrix[j, i] == 0) InitEdge(vertices[i], vertices[j], adjMatrix[i, j], true);
                            else if (adjMatrix[i, j] == 0) InitEdge(vertices[j], vertices[i], adjMatrix[j, i], true);
                            else InitEdge(vertices[i], vertices[j], adjMatrix[i, j], true, true, adjMatrix[j, i]);
                        }
                        else if (adjMatrix[i, j] != 0) { InitEdge(vertices[i], vertices[j], adjMatrix[i, j]); }
                        j += 1;
                    }
                }

                PaintOver(splitContainer.Panel1.CreateGraphics()); // přemaluji celý graf
                CapturePreviousState(); // zachytím stav grafu
                comboVertices.Items.Clear(); // vymažu dosavadní source vrcholy
                for (int i = 1; i < size + 1; i++) comboVertices.Items.Add(i); // přidám source vrcholy
                matrix.Text = PrintMatrix(vertices.Count, adjMatrix); // vypíši novou matici sousednosti
            }

            void error(string msg) { errorMsg.Text = "Error:\n" + msg; } // funkce pro vypisování chyb

            int readNumber(ref int index) // čtu číslo, které by mělo následovat po ',' nebo '['
            {
                string num = "";

                while (index < strMatrix.Length && (Char.IsDigit(strMatrix[index]) || (num.Length == 0 && strMatrix[index] == '-')))
                {
                    num += strMatrix[index];
                    index += 1;
                }

                if (num == "") return 0;
                return int.Parse(num);
            }

            strMatrix = strMatrix.Replace(" ", ""); // vyčistím string
            if (strMatrix.Length < 2) error("elements");
            else { strMatrix = strMatrix.Remove(0, 1); strMatrix = strMatrix.Remove(strMatrix.Length - 1); } // odstraním [ a ] z konců stringu

            List<int> numbers = new List<int>(); // zde budu zapisovat přečtená čísla
            int count = 0; int index = 0; int rows = 0;

            while (index < strMatrix.Length) // čtu a kontroluji parametry
            {
                switch (strMatrix[index]) // na co jsem narazil
                {
                    case '[':
                        count += 1; index += 1; // dávám pozor na správné uzávorkování
                        if (count > 1) { error("brackets"); return; }
                        numbers.Add(readNumber(ref index));
                        break;
                    case ']':
                        count -= 1; index += 1; rows += 1; // ] by měla značit konec jednoho řádku
                        if (count < 0) { error("brackets"); return; }
                        break;
                    case ',':
                        index += 1;
                        if (index < strMatrix.Length && strMatrix[index] != '[')
                            numbers.Add(readNumber(ref index));
                        break;
                    default:
                        error("elements"); // pokud se vymyká z povolených znaků
                        return;
                }
            }

            if (count != 0 || index == 0 || rows * rows != numbers.Count) error("elements"); // pokud string nedodežel zásady matice
            else { import(rows, numbers); } // pokud ano, nahraji matici
        }

        public string MatrixToString(int vertex_count) // vypsání matice do stringu pro zkopírování na pozdější použití
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

        /************* GRAPH CONTROLS **************/
        public void InitEdge(Vertex v1, Vertex v2, int weight = 1, bool oriented = false, bool doubleOriented = false, int weightSnd = 1) // inicializace hrany
        {
            Edge edge = new Edge(v1, v2, GetTheMiddle(v1.pos, v2.pos), weight, false, weightSnd); // vytvořím Edge objekt, kde je weightLabel vprostřed hrany
            splitContainer.Panel1.Controls.Add(edge.weightLabel);
            edges.Add(edge);
            if (oriented == true) // pokud tvořím orientovanou hranu, pustím funkci která ji na ni přemaluje
            {
                LeftClickedEdge(edge, v2.pos);
                if (doubleOriented) LeftClickedEdge(edge, v1.pos);  
            } 
        }

        public void LeftClickedVertex(Vertex sender) // začnu tvořit nebo ukotvím hranu
        {
            if (drag == true) // ukotvím hranu
            { 
                InitEdge(origin, sender);
                AdjustMatrix(origin.num, sender.num, 1);
            }
            else { drag = true; }

            origin = sender;
            pos = sender.pos;
            vertexLabel.Visible = false;
            PaintOver(splitContainer.Panel1.CreateGraphics());
            CapturePreviousState();
            matrix.Text = PrintMatrix(vertices.Count, adjMatrix);
            dragUpdate.Start(); // začnu s přemalováváním hrany
        }

        public void LeftClickedEdge(Edge sender, Point pos) // začnu přepisovat váhu kliknuté hrany, nebo přemaluji orientovanost
        {
            float fromV1 = EvalDistance(pos, sender.v1.pos); // vyhodnotím vzdálenosti vrcholů od myši
            float fromV2 = EvalDistance(pos, sender.v2.pos); 

            if (Math.Min(fromV1, fromV2) < 40) // pokud se kliklo dostatečně blízko k jednomu z vrcholů
            {
                Graphics g = splitContainer.Panel1.CreateGraphics();
                Point pt; Vertex closerV; // pt - bod na hraně vzdálen od closerV (bližšího vrcholu) určitou vzdálenost
                int u1 = sender.v1.pos.X - sender.v2.pos.X; // naleznu vektor 'u' hrany
                int u2 = sender.v1.pos.Y - sender.v2.pos.Y;
                ScaleVector(35, ref u1, ref u2); // vektor si přeškáluji
                float lengthOfEdge = EvalDistance(sender.v1.pos, sender.v2.pos); // zjistím délku hrany

                if (fromV1 < fromV2)
                { // najdu bod, který je od bližšího vrcholu vzdálený o vektor 'u' a není od vzdálenějšího vrcholu dále než je délka hrany (aby ležel na hraně)
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

                ScaleVector(15, ref u1, ref u2); // přeškáluji normálový vektor

                if (sender.oriented == true)
                {
                    if (sender.doubleOriented == true) // pokud je dvojitě orientovaná, každé kliknutí způsobí přesun k jednoduché orientovanosti
                    {
                        if (closerV.pos == sender.ptsArrow[0]) // pokud je orientovanost jednoduchá, používají se vždy první tři údaje
                        {
                            for (int i = 0; i < 3; i++) sender.ptsArrow[i] = sender.ptsArrow[i + 3];
                            sender.ptsArrow[3] = new Point(-1, -1);
                            sender.weight = sender.weightSnd;
                        }                      

                        if (closerV == sender.v2) AdjustMatrix(sender.v1.num, closerV.num, 0, true);
                        else AdjustMatrix(sender.v2.num, closerV.num, 0, true);

                        sender.weightSnd = 1; // vyresetuji druhou váhu
                        sender.weightLabel.Text = $"{sender.weight}";
                        sender.weightLabel.Size = new Size((sender.weightLabel.Text.Length + 1) * 10, sender.weightLabel.Size.Height);


                        sender.doubleOriented = false;
                    }
                    else // při jednoduché orientovanosti mohou nastat dvě možnosti
                    {
                        if (sender.ptsArrow[0] == closerV.pos) // klikl jsem k orientovanému konci hrany
                        {
                            sender.oriented = false; // ztrácím orientovanost
                            AdjustMatrix(sender.v1.num, sender.v2.num, sender.weight);
                        }
                        else // klikl jsem k neorientovanému konci hrany
                        {
                            sender.doubleOriented = true; // získávám oboustrannou orientovanost
                            sender.ptsArrow[3] = closerV.pos; sender.ptsArrow[4] = new Point(pt.X - u2, pt.Y + u1); sender.ptsArrow[5] = new Point(pt.X + u2, pt.Y - u1);
                            
                            sender.weightLabel.Text = $"{sender.weight}, {sender.weightSnd}";
                            sender.weightLabel.Size = new Size((sender.weightLabel.Text.Length + 1) * 10, sender.weightLabel.Size.Height);

                            if (closerV == sender.v2) AdjustMatrix(sender.v1.num, closerV.num, sender.weightSnd, true);
                            else AdjustMatrix(sender.v2.num, closerV.num, sender.weightSnd, true);
                        }
                    }
                }
                else // ukotvím v bodech vzdálených o normálový vektor od pt čáry, směřující k bližímu vrcholu tak, že se nakreslí šipka
                {
                    sender.oriented = true;
                    sender.ptsArrow = new Point[6];
                    sender.ptsArrow[0] = closerV.pos; sender.ptsArrow[1] = new Point(pt.X - u2, pt.Y + u1); sender.ptsArrow[2] = new Point(pt.X + u2, pt.Y - u1);

                    if (closerV == sender.v1) { AdjustMatrix(sender.v1.num, sender.v2.num, 0, true); AdjustMatrix(sender.v2.num, sender.v1.num, sender.weight, true); }
                    else { AdjustMatrix(sender.v2.num, sender.v1.num, 0, true); AdjustMatrix(sender.v1.num, sender.v2.num, sender.weight, true); }
                }

                PaintOver(g);
                CapturePreviousState();
                matrix.Text = PrintMatrix(vertices.Count, adjMatrix);
            }
            else // jestli je kliknutí vzdáleno více než určitá vzdálenost je to signál pro přepisování váhy
            {
                sender.weightLabel.Text = "";
                writingOn = sender; // na jakou hranu píšu

                if (sender.doubleOriented) // zkontroluji, jestli nebudu přepisovat druhou váhu
                {
                    Point closerPos;
                    if (fromV1 < fromV2) closerPos = sender.v1.pos;
                    else closerPos = sender.v2.pos;

                    if (closerPos == sender.ptsArrow[3]) writingOnSecondWeight = true;
                }

                KeyPreview = true;
                writeTimer.Start(); // spustím psací timer
            }        
        }

        public void DeleteVertex(Vertex sender) // odstraním a přemaluji vrchol a k němu napojené hrany
        {
            for (int i = edges.Count - 1; i >= 0; i--)
                if (edges[i].v1 == sender || edges[i].v2 == sender) { splitContainer.Panel1.Controls.Remove(edges[i].weightLabel); edges.RemoveAt(i); }

            int removedNum = sender.num;
            vertices.Remove(sender);

            foreach (Vertex vertex in vertices) // přepíšu čísla vrcholů, které měly větší číslo než odstraňovaný
                if (vertex.num > removedNum) vertex.num -= 1;

            comboVertices.Items.RemoveAt(vertices.Count);
            vertexLabel.Visible = false;
            PaintOver(splitContainer.Panel1.CreateGraphics());
            CapturePreviousState();
            matrix.Text = PrintMatrix(vertices.Count, adjMatrix);
        }

        public void DeleteEdge(Edge sender) // odstraním a přemaluji hranu
        {
            AdjustMatrix(sender.v1.num, sender.v2.num, 0);
            sender.oriented = false; sender.doubleOriented = false;
            splitContainer.Panel1.Controls.Remove(sender.weightLabel);
            edges.Remove(sender);
            PaintOver(splitContainer.Panel1.CreateGraphics());
            CapturePreviousState();
            matrix.Text = PrintMatrix(vertices.Count, adjMatrix);
        }

        /************* DRAWING **************/
        public void PaintOver(Graphics g) // přemaluji vrcholy i hrany
        {
            void RedrawEdges(Graphics g)
            {
                foreach (Edge edge in edges)
                {
                    DrawEdge(g, new Pen(Color.White, 10), edge.v1.pos, edge.v2.pos);
                    if (edge.oriented)
                    {
                        if (edge.doubleOriented)
                        {
                            DrawEdge(g, new Pen(Color.White, 7), edge.ptsArrow[3], edge.ptsArrow[4]);
                            DrawEdge(g, new Pen(Color.White, 7), edge.ptsArrow[3], edge.ptsArrow[5]);
                        }

                        DrawEdge(g, new Pen(Color.White, 7), edge.ptsArrow[0], edge.ptsArrow[1]);
                        DrawEdge(g, new Pen(Color.White, 7), edge.ptsArrow[0], edge.ptsArrow[2]);
                    }
                }
            }

            void RedrawVertices(Graphics g)
            {
                foreach (Vertex vertex in vertices)
                {
                    DrawVertex(g, vertex.pos, new Size(30, 30));
                }
            }

            SolidBrush brush = new SolidBrush(Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(28)))), ((int)(((byte)(36))))));
            g.FillRectangle(brush, new Rectangle(0, 0, splitContainer.Panel1.Width, splitContainer.Panel1.Height)); // přemaluji celý Panel1 barvou pozadí
            RedrawEdges(g);
            RedrawVertices(g);
        }

        public void DrawEdge(Graphics g, Pen pen, Point pt1, Point pt2) // namaluji HighQuality Line specifickým perem se zaobleným koncem
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            g.DrawLine(pen, pt1, pt2);
        }

        public void DrawVertex(Graphics g, Point location, Size size, TextureBrush brush = null) // namaluji HighQuality Ellipse specificým Brush nebo TextureBrush
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            if (brush != null) g.FillEllipse(brush, new Rectangle(location.X - (size.Width / 2), location.Y - (size.Height / 2), size.Width, size.Height));
            else g.FillEllipse(Brushes.White, new Rectangle(location.X - (size.Width / 2), location.Y - (size.Height / 2), size.Width, size.Height));
        }

        /************* ALL ABOUT HOVER **************/
        public Vertex GetHoveredVertex(Point mPos) // zjistím, na kterém vrcholu mám kurzor (pokud mám)
        {
            foreach (Vertex vertex in vertices)
            {
                if (Math.Sqrt((mPos.X - vertex.pos.X) * (mPos.X - vertex.pos.X) + (mPos.Y - vertex.pos.Y) * (mPos.Y - vertex.pos.Y)) <= vertex.radius)
                    return vertex;
            }

            return null;
        }

        public Edge GetHoveredEdge(Point mPos) // zjistím, na které hraně mám vrchol (pokud mám)
        {
            float a; float b; float c; float dist; float denom; float distToMouseA; float distToMouseB;

            foreach(Edge edge in edges) // jak jsem vzdálený od přímky která vede hranou
            {               
                a = edge.v2.pos.Y - edge.v1.pos.Y;
                b = -edge.v2.pos.X + edge.v1.pos.X;
                c = -a * edge.v1.pos.X - b * edge.v1.pos.Y;

                denom = (float)Math.Sqrt(a * a + b * b);
                dist = Math.Abs(a * mPos.X + b * mPos.Y + c) / denom;
                if (dist > 7) continue;

                distToMouseA = EvalDistance(edge.v1.pos, mPos); // vzdálenost myši od vrcholů
                distToMouseB = EvalDistance(edge.v2.pos, mPos);

                if (denom > distToMouseA && denom > distToMouseB) // jestli je na hraně a ne pouze na přímce hrany
                    return edge;
            }

            return null;
        }

        public Vertex HoveredV(Vertex vertex) // ovládá škálování hovered vrcholu
        {
            if (vertex == hVertex) return vertex; // pokud jsem už vrchol škáloval, vrátím
            Graphics g = splitContainer.Panel1.CreateGraphics();

            if (vertex == null) // můj kurzor už není na hovered vrcholu
            {
                vertexLabel.Visible = false;
                hVertex.radius = 15;
                DrawVertex(g, hVertex.pos, new Size(42, 42), prevState);
            }
            else // můj kurzor je na vrcholu
            {
                if (hVertex != null) // přešel jsem z vrcholu na vrchol
                {
                    hVertex.radius = 15;
                    DrawVertex(g, hVertex.pos, new Size(42, 42), prevState);
                }
               
                vertexLabel.Location = new Point(vertex.pos.X - 10, vertex.pos.Y - 10);
                vertexLabel.Text = $"{vertex.num}";
                vertexLabel.Size = new Size((vertexLabel.Text.Length + 1) * 10, 20);
                vertexLabel.Visible = true; // zobrazím číslo vrcholu

                vertex.radius = 20;
                DrawVertex(g, vertex.pos, new Size(40, 40));
            }

            return vertex;
        }

        public Edge HoveredE(Edge edge) // ovládá škálování hovered hrany
        {
            if (drag == true || edge == hEdge) return edge; // neškáluji pokud zrovna táhnu hranu
            Graphics g = splitContainer.Panel1.CreateGraphics();

            if (edge == null) // nemám kurzor na hraně, přemaluji na počáteční stav
            {
                DrawEdge(g, penPrevState, hEdge.v1.pos, hEdge.v2.pos);
                if (hEdge.oriented)
                { 
                    if (hEdge.doubleOriented)
                    {
                        DrawEdge(g, new Pen(Color.White, 7), hEdge.ptsArrow[3], hEdge.ptsArrow[4]);
                        DrawEdge(g, new Pen(Color.White, 7), hEdge.ptsArrow[3], hEdge.ptsArrow[5]);
                    }

                    DrawEdge(g, new Pen(Color.White, 7), hEdge.ptsArrow[0], hEdge.ptsArrow[1]);
                    DrawEdge(g, new Pen(Color.White, 7), hEdge.ptsArrow[0], hEdge.ptsArrow[2]);
                }                
            }
            else
            {
                if (hEdge != null) // přešel jsem z hrany na hranu
                {
                    DrawEdge(g, penPrevState, hEdge.v1.pos, hEdge.v2.pos);
                }

                DrawEdge(g, new Pen(Color.White, 13), edge.v1.pos, edge.v2.pos);
            }

            return edge;
        }

        /************* DRIVING TIMERS **************/
        private void dragUpdate_Tick(object sender, EventArgs e) // timer pro přemalovávání tažené hrany
        {
            if (prevMousePos != splitContainer.Panel1.PointToClient(MousePosition)) // pokud nemám myš na stejném místě
            {
                Graphics g = splitContainer.Panel1.CreateGraphics();
                Pen whtPen = new Pen(Color.White, 10);

                if (hVertex != null) // pokud mám myš na vrcholu
                { 
                    if (hVertex.pos != prevMousePos) // pokud už zde není hrana namalovaná, přitáhnu konec hrany jako magnet ke středu vrcholu
                    {
                        DrawEdge(g, penPrevState, pos, new Point(prevMousePos.X, prevMousePos.Y));
                        DrawVertex(g, hVertex.pos, new Size(40, 40));
                        prevMousePos = hVertex.pos;
                    }
                }
                else // následuje pohyb myši
                { 
                    DrawEdge(g, penPrevState, pos, new Point(prevMousePos.X, prevMousePos.Y));
                    prevMousePos = splitContainer.Panel1.PointToClient(MousePosition);
                }

                DrawEdge(g, whtPen, pos, prevMousePos);
            }
        }

        private void fixedUpdate_Tick(object sender, EventArgs e) // kontroluje, jestli není kurzor na vrcholu nebo hraně
        {
            Point mPos = splitContainer.Panel1.PointToClient(MousePosition);
            hVertex = HoveredV(GetHoveredVertex(mPos));
            if (hVertex != null && hEdge == null) return; // pokud existuje hovered vrchol a neexistuje hovered hrana, vrátím
            hEdge = HoveredE(GetHoveredEdge(mPos));
        }

        /************* EVENT THINGS **************/
        private void fixedUpdate_Start(object sender, EventArgs e) { fixedUpdate.Start(); } // pokud kurzor vejde na Panel1, spustí se
        private void fixedUpdate_End(object sender, EventArgs e) { fixedUpdate.Stop(); } // pokud kurzor opustí Panel1
        private void copyBtn_Click(object sender, EventArgs e) { Clipboard.SetText(MatrixToString(vertices.Count)); } // zkopíruje vytvořenou matici sousednosti
        private void insertBtn_Click(object sender, EventArgs e) { if (drag == false) InsertMatrix(pasteBox.Text); } // pokud se netáhne hrana, předá insertlý text
        private void Form1_Resize(object sender, EventArgs e) { PaintOver(splitContainer.Panel1.CreateGraphics()); } // pokud se přeškáluje okno, přemaluje se graf
        private void Form1_LocationChanged(object sender, EventArgs e) { PaintOver(splitContainer.Panel1.CreateGraphics()); } // pokud se změní lokace okna, přemaluje se graf

        /************* METHODS FOR WEIGHTS **************/
        public void StopWriting() // zastaví psaní do weightLabelu (váha hrany)
        {
            KeyPreview = false;
            writeTimer.Stop();
            writingOn.weightLabel.Text = writingOn.weightLabel.Text.Replace("_", ""); // vyčistí

            if (writingOn.weightLabel.Text != "")// pokud je neprázdný, přečte číslo a uloží ho
            {
                if (writingOnSecondWeight) writingOn.weightSnd = int.Parse(writingOn.weightLabel.Text);
                else writingOn.weight = int.Parse(writingOn.weightLabel.Text);
            }

            if (writingOn.doubleOriented) writingOn.weightLabel.Text = $"{writingOn.weight}, {writingOn.weightSnd}"; // text nastavím na váhu hrany
            else writingOn.weightLabel.Text = $"{writingOn.weight}";

            if (writingOn.oriented == true) // pokud je přepisovaná hrana orientovaná, přepíšu pouze jedno místo v matici
            {
                if (writingOnSecondWeight)
                {
                    if (writingOn.ptsArrow[3] == writingOn.v2.pos) adjMatrix[writingOn.v1.num - 1, writingOn.v2.num - 1] = writingOn.weightSnd;
                    else adjMatrix[writingOn.v2.num - 1, writingOn.v1.num - 1] = writingOn.weightSnd;
                }
                else
                {
                    if (writingOn.ptsArrow[0] == writingOn.v2.pos) adjMatrix[writingOn.v1.num - 1, writingOn.v2.num - 1] = writingOn.weight;
                    else adjMatrix[writingOn.v2.num - 1, writingOn.v1.num - 1] = writingOn.weight;
                } 
            }
            else AdjustMatrix(writingOn.v1.num, writingOn.v2.num, writingOn.weight);

            writingOnSecondWeight = false;
            writingOn.weightLabel.Size = new Size((writingOn.weightLabel.Text.Length + 1) * 10, writingOn.weightLabel.Size.Height); // přeškáluji label
            matrix.Text = PrintMatrix(vertices.Count, adjMatrix);
        }
        
        private void writeTimer_Tick(object sender, EventArgs e) // kosmetický timer, který bliká s '_', pro efekt aktivního psaní
        {
            int lastIndex = writingOn.weightLabel.Text.Length - 1;
            if (writingOn.weightLabel.Text == "" || writingOn.weightLabel.Text[lastIndex] != '_') writingOn.weightLabel.Text += "_";
            else writingOn.weightLabel.Text = writingOn.weightLabel.Text.Replace("_", "");
        }
        
        private void Form1_KeyPress(object sender, KeyPressEventArgs e) // zachycuje psané znaky
        {
            int i;
            if (int.TryParse(e.KeyChar.ToString(), out i) || e.KeyChar == '-') // pokud je vstup číslem nebo minus
            {
                if (e.KeyChar == '-') { if (writingOn.weightLabel.Text == "" || writingOn.weightLabel.Text == "_") writingOn.weightLabel.Text = writingOn.weightLabel.Text.Replace("_", "") + "-"; }
                else writingOn.weightLabel.Text = writingOn.weightLabel.Text.Replace("_", "") + Convert.ToString(i);
            }
            else if (e.KeyChar == (char)Keys.Back) // backspace
            {
                string s = writingOn.weightLabel.Text.Replace("_", "");
                if (writingOn.weightLabel.Text.Length != 0) writingOn.weightLabel.Text = s.Remove(s.Length - 1);
            }
            else StopWriting(); // jiná klávesa psaní ukončí

            writingOn.weightLabel.Size = new Size((writingOn.weightLabel.Text.Length + 1) * 10, writingOn.weightLabel.Size.Height); // aktivně resizuju
        }

        /************* AUXILIARY FUNCTIONS **************/
        public bool validDistances(Point pos, int criticalDist) // zkontroluje, jestli jsou všechny vrcholy vzdáleny od pos alespoň o criticalDist
        {
            foreach (Vertex vertex in vertices)
            {
                if (EvalDistance(pos, vertex.pos) < criticalDist) return false;
            }

            return true;
        }

        public void CapturePreviousState() // zachytí stav tvořeného grafu
        {
            TextureBrush TakeAScreenshot()
            {
                Bitmap bmpPrevState = new Bitmap(splitContainer.Panel1.Width, splitContainer.Panel1.Height);
                Graphics.FromImage(bmpPrevState).CopyFromScreen(PointToScreen(splitContainer.Panel1.Location), new Point(0, 0), splitContainer.Panel1.Size);
                return new TextureBrush(bmpPrevState);
            }

            prevState = TakeAScreenshot(); // jako TextureBrush
            penPrevState = new Pen(prevState, 15); // jako Pen
        }

        public float EvalDistance(Point a, Point b) // vyhodnotí vzdálenost mezi dvěma body
        {
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        public void ScaleVector(int mag, ref int u1, ref int u2) // přeškáluje vektor na vzdálenost mag
        {
            float k = (float)Math.Sqrt(mag * mag / ((float)u1 * u1 + (float)u2 * u2));
            u1 = (int)(k * u1); u2 = (int)(k * u2);
        }

        public Point GetTheMiddle(Point a, Point b) // vrátí prostřední bod mezi 'a' a 'b'
        {
            Point vektor = new Point((b.X - a.X) / 2, (b.Y - a.Y) / 2);
            if (EvalDistance(new Point(a.X + vektor.X, a.Y + vektor.Y), b) < EvalDistance(a, b)) return new Point(a.X + vektor.X, a.Y + vektor.Y);
            else return new Point(a.X - vektor.X, a.Y - vektor.Y);
        }

        /************* METHODS FOR ALGORITHMS **************/
        public void switchLabels() // mení text a skrývá objekty při přepínání mezi algoritmy
        {
            switch (alg)
            {
                case 0: // DFS
                    algDescription.Text = "Depth - first search(DFS) is an algorithm for traversing or searching tree or graph data structures.The algorithm starts at the source node and explores as far as possible along each branch before backtracking.";
                    labelSource.Show();
                    comboVertices.Show();
                    break;
                case 1: // BFS
                    algDescription.Text = "Breadth-first search (BFS) is an algorithm for searching a tree data structure for a node that satisfies a given property. It starts at the source node and explores all nodes at the present depth prior to moving on to the nodes at the next depth level.";
                    break;
                case 2: // Dijkstra
                    algDescription.Text = "Given a graph and a source vertex in the graph, find the shortest paths from the source to all vertices in the given graph.";
                    labelSource.Show();
                    comboVertices.Show();
                    break;
                case 3: // Bridges
                    algDescription.Text = "An edge in an undirected connected graph is a bridge if removing it disconnects the graph. For a disconnected undirected graph, definition is similar, a bridge is an edge removing which increases number of disconnected components.";
                    labelSource.Hide();
                    comboVertices.Hide();
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
                    break;
                case 8: // Topological Sort
                    algDescription.Text = "Topological sorting for Directed Acyclic Graph (DAG) is a linear ordering of vertices such that for every directed edge u v, vertex u comes before v in the ordering.";
                    labelSource.Hide();
                    comboVertices.Hide();
                    break;
            }

            resultLabel.Text = "";
            resultMatrix.Text = "[...]";
        }

        private void rightBtn_Click(object sender, EventArgs e) // přepne na algoritmus vpravo
        {
            alg = (alg + 1) % 9;
            algLabel.Text = algoNames[alg];
            switchLabels();
        }

        private void leftBtn_Click(object sender, EventArgs e) // přepne na algoritmus vlevo
        {
            alg -= 1;
            if (alg < 0) alg += 9;
            algLabel.Text = algoNames[alg];
            switchLabels();
        }

        private void startBtn_Click(object sender, EventArgs e) // spustí algoritmus a vypíše výsledek
        {
            if (vertices.Count == 0) return;
            bool orientedGraph = false;
            foreach (Edge edge in edges) if (edge.oriented == true) orientedGraph = true; // zkontroluji, jestli je graf orientovaný
            int source = comboVertices.SelectedIndex; // jako source uloží index vybraného vrcholu v comboBoxu
            if (source == -1) source = 0;
            
            AlgObj = new Algorithms(adjMatrix, vertices.Count); // iniciuju nový objekt Algorithms, kterému předám matici sousednosti

            switch (alg) // spustím specifický algoritmus
            {
                case 0: // DFS
                    resultLabel.Text = "[ " + AlgObj.soloDFS(source) + "]";
                    break;
                case 1: // BFS
                    resultLabel.Text = "[ " + AlgObj.BFS(source) + "]";
                    break;
                case 2: // Dijkstra
                    Tuple<int[], int[]> resTuple = AlgObj.Dijkstra(source);
                    if (resTuple.Item1[0] == -1)
                        resultLabel.Text = "Contains negative edges!";
                    else
                        resultLabel.Text = "DistArr: [ " + string.Join(", ", resTuple.Item1) + " ]";
                        resultMatrix.Text = "PrevArr: [ " + string.Join(", ", resTuple.Item2) + " ]";
                    break;
                case 3: // Bridges
                    if (orientedGraph) { resultLabel.Text = "Applies only to undirected graphs!"; return; }
                    List<(int,int)> resList = AlgObj.Bridges();
                    if (resList.Count > 0) resultLabel.Text = string.Join("\n", resList);
                    else resultLabel.Text = "No bridges found!";
                    break;
                case 4: // Komponenty
                    resultMatrix.Text = AlgObj.Components();
                    break;
                case 5: // SSK
                   resultLabel.Text =  "[ " + string.Join(", ", AlgObj.SSK()) + " ]";
                    break;
                case 6: // FW
                    resultMatrix.Text = PrintMatrix(vertices.Count, AlgObj.FW());
                    break;
                case 7: // Jarnik
                    if (orientedGraph) { resultLabel.Text = "Applies only to undirected graphs!"; return; }
                    resultMatrix.Text = PrintMatrix(vertices.Count, AlgObj.Jarnik());
                    break;
                case 8: // Topological Sort
                    resultLabel.Text = "[ " + AlgObj.TopologicalSort() + " ]";
                    break;
            }
        }

        private void exampleBtn_Click(object sender, EventArgs e) // nahraje ukázkový graf pro specifický algoritmus
        {
            switch (alg)
            {
                case 0: // DFS
                    InsertMatrix("[[0,1,0,0,1,0,0],[1,0,1,1,0,0,0],[0,1,0,0,0,0,0],[0,1,0,0,0,0,0],[1,0,0,0,0,1,1],[0,0,0,0,1,0,0],[0,0,0,0,1,0,0]]");
                    break;
                case 1: // BFS
                    InsertMatrix("[[0,1,0,0,1,0,0],[1,0,1,1,0,0,0],[0,1,0,0,0,0,0],[0,1,0,0,0,0,0],[1,0,0,0,0,1,1],[0,0,0,0,1,0,0],[0,0,0,0,1,0,0]]");
                    break;
                case 2: // Dijkstra
                    InsertMatrix("[[0,1,0,3],[0,0,7,0],[2,0,0,3],[0,2,3,0]]");
                    break;
                case 3: // Bridges
                    InsertMatrix("[[0,1,1,1,0,0],[1,0,1,0,0,0],[1,1,0,0,0,0],[1,0,0,0,1,1],[0,0,0,1,0,1],[0,0,0,1,1,0]]");
                    break;
                case 4: // Komponenty
                    InsertMatrix("[[0,1,1,0,0,0,0],[1,0,1,0,0,0,0],[1,1,0,0,0,0,0],[0,0,0,0,1,1,0],[0,0,0,1,0,1,0],[0,0,0,1,1,0,0],[0,0,0,0,0,0,0]]");
                    break;
                case 5: // SSK
                    InsertMatrix("[[0,0,1,0],[1,0,0,1],[0,1,0,1],[0,0,0,0]]");
                    break;
                case 6: // FW
                    InsertMatrix("[[0,3,0,4],[3,0,0,10],[0,2,0,0],[0,0,1,0]]");
                    break;
                case 7: // Jarnik
                    InsertMatrix("[[0,1,3,0,7],[1,0,2,5,0],[3,2,0,8,7],[0,5,8,0,6],[7,0,7,6,0]]");
                    break;
                case 8: // Topological Sort
                    InsertMatrix("[[0,1,1,0,0],[0,0,1,1,0],[0,0,0,1,1],[0,0,0,0,0],[0,0,0,0,0]]");
                    break;
            }
        }
    }


    /// <summary>
    /// Třída vrcholu.
    /// </summary>
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

    /// <summary>
    /// Třída hrany.
    /// </summary>
    public class Edge
    {
        public Vertex v1;
        public Vertex v2;
        public bool oriented = false; public bool doubleOriented = false;
        public Point[] ptsArrow; // zaznamenává dvakrát po třech bodech - vrchol, do kterého jde orientovaná hrana a dva body odkud se kreslí šipka
        public int weight; public int weightSnd;
        public Label weightLabel;

        public Edge(Vertex v1, Vertex v2, Point loc, int weight = 1, bool oriented = false, int weightSnd = 1, bool doubleOriented = false)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.oriented = oriented; this.doubleOriented = doubleOriented;
            this.weight = weight; this.weightSnd = weightSnd;

            weightLabel = new Label();
            weightLabel.Location = loc;
            weightLabel.ForeColor = Color.White;
            weightLabel.Text = $"{weight}";
            weightLabel.Size = new Size((weightLabel.Text.Length + 1) * 10, 20);
        }
    }


    /// <summary>
    /// Třída binární haldy s prvky tvaru immuntable tuple integerů,
    /// která zaznamenává pozici každého prvku.
    /// </summary>
    public class Heap
    {
        public List<(int, int)> heap = new List<(int, int)>{ (0, 0) }; // první prvek nepoužívá
        public int[] pos;

        public Heap(int maxKey)
        {
            pos = new int[maxKey + 1]; // jaký bude maximální klíč
        }

        public void Insert((int, int) element) // insertne nový prvek
        {
            heap.Add(element);
            int j = heap.Count - 1;
            pos[element.Item1] = j;
            BubbleUp(j);
        }

        private void BubbleUp(int j) // bublání směrem nahoru
        {
            while (j > 1 && heap[j].Item2 < heap[j / 2].Item2)
            {
                pos[heap[j].Item1] = j / 2; pos[heap[j / 2].Item1] = j;

                (int, int) temp = heap[j];
                heap[j] = heap[j / 2]; heap[j / 2] = temp;
                j = j / 2;
            }
        }

        public int ExtractMin() // popne minimum a přebublá dolu
        {
            if (heap.Count == 1) return -1;

            int min = heap[1].Item1; pos[min] = 0;
            heap[1] = heap[heap.Count - 1]; pos[heap[1].Item1] = 1;
            heap.RemoveAt(heap.Count - 1);

            int j = 1;
            while (2 * j < heap.Count)
            {
                int n = 2 * j;
                if (n < heap.Count - 1)
                {
                    if (heap[n + 1].Item2 < heap[n].Item2) n += 1;

                    if (heap[j].Item2 > heap[n].Item2)
                    {
                        pos[heap[j].Item1] = n; pos[heap[n].Item1] = j;

                        (int, int) temp = heap[j];
                        heap[j] = heap[n]; heap[n] = temp;
                        j = n;
                    }
                    else break;
                }
                else break;
            }

            return min;
        }

        public void DecreaseKey(int key, int value) // změní hodnotu u specifického klíče
        {
            int index = pos[key];
            if (index == 0) return;

            heap[index] = (key, value);
            BubbleUp(index);
        }

        public bool Empty() // vrátí false pokud je halda neprázdná
        {
            if (heap.Count > 1) return false;
            else return true;
        }
    }

    /// <summary>
    /// Třída obsahující specifické grafové algoritmy komunikující s Form1.
    /// </summary>
    public class Algorithms
    {
        int[,] adjM; int[][] nasled;
        bool containsNegativeEdges;
        int n; const int infty = 999999;

        public Algorithms(int[,] adjMatrix, int numVertices)
        {
            adjM = GetMatrix(adjMatrix, numVertices); // uloží určitou část matice sousednosti
            nasled = seznamNasledniku(adjM); // vytvoří seznam následníků
            n = adjM.GetLength(0); // uloží počet vrcholů
        }

        int[,] GetMatrix(int[,] adjMatrix, int numVertices) // přepíše specifický čtverec matice sousednosti
        {
            int[,] matrix = new int[numVertices, numVertices];

            for (int i = 0; i < numVertices; i++)
            {
                for (int j = 0; j < numVertices; j++) matrix[i, j] = adjMatrix[i, j];
            }
            return matrix;
        }

        int[][] seznamNasledniku(int[,] adjMatrix) // přepíše matici sousednosti do seznamu následníků
        {
            int size = adjMatrix.GetLength(0);
            int[][] jag = new int[size][];
            List<int> temp; // dočasný List pro každou řádku matice

            for (int i = 0; i < size; i++)
            {
                temp = new List<int>();

                for (int j = 0; j < size; j++)
                {
                    if (adjMatrix[i, j] != 0) temp.Add(j);
                    if (adjMatrix[i, j] < 0) containsNegativeEdges = true;
                }

                jag[i] = new int[temp.Count];

                for (int k = 0; k < temp.Count; k++)
                {
                    jag[i][k] = temp[k];
                }
            }

            return jag;
        }

        bool relax(int u, int v, ref int[] dist, ref int[] prev) // relaxování hrany
        {
            if (dist[u] + adjM[u, v] < dist[v])
            {
                dist[v] = dist[u] + adjM[u, v];
                prev[v] = u;
                return true;
            }

            return false;
        }

        Tuple<string, int[], int[], List<int>, List<(int, int)>, List<(int, int)>, List<(int, int)>, List<(int, int)>> DFS() // rozšířený podrobný DFS
        {
            int c = 0; int order = 0; // komponenta a pořadí
            int[] opened = new int[n]; int[] pred = new int[n]; int[] low = new int[n];
            List<int> ends = new List<int>(); // vrcholy podle uzavření
            string stromy = ""; // DFS stromy

            int[] p = new int[n]; int[] k = new int[n]; // casy otevreni a zavreni

            List<(int, int)> zpet = new List<(int, int)>(); // zpětné hrany
            List<(int, int)> dopred = new List<(int, int)>(); // dopředné
            List<(int, int)> pric = new List<(int, int)>(); // příčné
            List<(int, int)> bridges = new List<(int, int)>(); // mosty

            void Visit(int u, ref int order, ref string stromy) // rekurzivní funkce DFS
            {
                opened[u] = 1; order += 1; p[u] = order; low[u] = infty;

                foreach (int neigh in nasled[u])
                {
                    if (opened[neigh] == 0) // stromová
                    {
                        pred[neigh] = u;
                        stromy += $", {neigh + 1}";
                        Visit(neigh, ref order, ref stromy);

                        if (low[neigh] >= p[neigh]) bridges.Add((u + 1, neigh + 1)); // je mostem

                        low[u] = Math.Min(low[u], low[neigh]); // upravím parametr pro hledání mostů
                    }
                    else
                    {
                        if (opened[neigh] == 1)
                        {
                            if (neigh != pred[u]) // pokud není falešná zpětná
                            {
                                low[u] = Math.Min(low[u], p[neigh]);
                                zpet.Add((u, neigh)); // zpětná
                            }
                        }
                        else
                        {
                            if (p[u] < p[neigh]) dopred.Add((u, neigh)); // dopředná
                            else pric.Add((u, neigh)); // příčná
                        }
                    }
                }

                ends.Add(u);
                opened[u] = -1; order += 1; k[u] = order;
            }

            for (int i = 0; i < n; i++)
            {
                if (opened[i] == 0) // pokud jsem vrchol 'i' nenavštívil
                {
                    c += 1; // zvýším počet komponent
                    stromy += $"\nstrom_{c}: {i + 1}";
                    Visit(i, ref order, ref stromy);
                }
            }

            return new Tuple<string, int[], int[], List<int>, List<(int, int)>, List<(int, int)>, List<(int, int)>, List<(int, int)>>(stromy, p, k, ends, zpet, bridges, dopred, pric);
        }

        public string soloDFS(int source) // jednoduchý DFS algoritmus
        {
            string order = ""; int last = 0;
            int[] stack = new int[(n * (n - 1)) / 2]; // přesněji ((n - 1)*(n - 2) / 2) + 1
            bool[] seen = new bool[n];
            stack[0] = source;

            while (last != -1)
            {
                int vertex = stack[last]; last -= 1;
                if (seen[vertex] == false) order += $"{vertex + 1} ";
                seen[vertex] = true;

                foreach (int neigh in nasled[vertex])
                {
                    if (seen[neigh] == false)
                    {
                        last += 1;
                        stack[last] = neigh;
                    }
                }
            }

            return order;
        }

        public string BFS(int source) // BFS algoritmus
        {
            string order = ""; int first = 0; int last = 0;
            int[] queue = new int[(n * (n - 1)) / 2];
            bool[] seen = new bool[n];
            queue[0] = source;
            // udržuji first a last pro efekt fronty
            while (first <= last)
            {
                int vertex = queue[first % queue.Length];
                first += 1;
                if (seen[vertex] == false) order += $"{vertex + 1} ";
                seen[vertex] = true;              

                foreach (int neigh in nasled[vertex])
                {
                    if (seen[neigh] == false)
                    {
                        last += 1;
                        queue[last % queue.Length] = neigh;
                    } 
                }
            }

            return order; // vracím pouze pořadí vrcholů jako string
        }

        public Tuple<int[], int[]> Dijkstra (int source) // algoritmus pro nalezení nejkratších vzdáleností od vybraného bodu
        {
            if (containsNegativeEdges) return new Tuple<int[], int[]>(new int[] { -1 }, new int[] { -1 }); // graf nesmí obsahovat záporné hrany
            int[] dist = new int[n];
            int[] prev = new int[n]; prev[source] = -1;
            Heap Q = new Heap(n - 1);

            for (int v = 0; v < n; v++)
            {
                if (v != source) dist[v] = infty; // vzdálenost od source nastavím na nekonečna
                Q.Insert((v, dist[v])); // vložím prvky do haldy
            }
                
            while (Q.Empty() == false)
            {
                int u = Q.ExtractMin();

                foreach (int neigh in nasled[u]) // relaxuju pro každého souseda
                {
                    if (relax(u, neigh, ref dist, ref prev)) Q.DecreaseKey(neigh, dist[neigh]);
                }
            }

            return new Tuple<int[], int[]>(dist, prev);
        }

        public List<(int, int)> Bridges() // vrací pouze mosty s využitím podrobného DFS
        {
            return DFS().Item6;
        }

        public string Components() // vrací pouze komponenty s využitím podrobného DFS
        {
            return DFS().Item1;
        }

        public int[] SSK() // algoritmus pro nalezení silně souvislých komponent
        {
            int[][] transpose(int[][] graph) // funkce pro transponování grafu
            {
                int[][] Gt = new int[n][];
                List<List<int>> temp = new List<List<int>>();
                for (int i = 0; i < n; i++) temp.Add(new List<int>()); // vytvořím jagged array z Listů

                for (int i = 0; i < n; i++)
                {
                    foreach (int vertex in graph[i]) temp[vertex].Add(i);
                }

                for (int k = 0; k < n; k++)
                {
                    Gt[k] = new int[temp[k].Count];

                    for (int j = 0; j < temp[k].Count; j++)
                    {
                        Gt[k][j] = temp[k][j]; // přepíši do normálního jagged array jako seznam následníků
                    }
                }

                return Gt;
            }

            List<int> ends = DFS().Item4; // uložím vrcholy tak, jak se zavíraly při DFS
            int[][] Gt = transpose(nasled); int c = 0;
            bool[] seen = new bool[n];
            List<int> stack = new List<int>();
            int[] sc_components = new int[n]; // zde uložím výsledné silně souvislé komponenty

            for (int i = Gt.GetLength(0) - 1; i >= 0; i--) // odzadu
            {
                if (seen[ends[i]]) continue; // pokud jsem vrchol už navštívil, přeskočím ho

                seen[ends[i]] = true;
                c += 1;
                stack.Add(ends[i]);

                while (stack.Count > 0) // každý DFS strom je teď jednou komponentou
                {
                    int u = stack[stack.Count - 1];
                    stack.RemoveAt(stack.Count - 1);
                    seen[u] = true;
                    sc_components[u] = c;

                    foreach (int neigh in Gt[u])
                    {
                        if (seen[neigh] == false)
                        {
                            stack.Add(neigh);
                        }
                    }
                }
            }

            return sc_components;
        }

        public int[,] FW () // Floyd-Warshallův algoritmus pro nalezení vzdáleností mezi každými dvěma vrcholy
        {
            int[,] D = new int[n, n];

            for (int i = 0; i < n; i++) // nastavím vzdálenosti mezi body, které nejsou spojeny hranou, na nekonečno
            {
                for (int j = 0; j < n; j++)
                {
                    if (adjM[i, j] == 0 && i != j) D[i, j] = infty;
                    else D[i, j] = adjM[i, j];
                }
            }

            for (int k = 0; k < n; k++) 
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        D[i, j] = Math.Min(D[i, j], D[i, k] + D[k, j]);
                    }
                }
            }

            return D;
        }

        public int[,] Jarnik() // Jarníkův nebo také Primův algoritmus pro nalezení minimální kostry
        {
            int[,] spanningTree = new int[n, n]; // výslednou kostru ukládám jako matici sousednosti
            int[] prev = new int[n];
            int[] key = new int[n];
            Heap Q = new Heap(n - 1); // abych zlepšil časovou složitost

            for (int i = 0; i < n; i++) // inicializace
            {
                key[i] = infty;
                prev[i] = -1;
                Q.Insert((i, key[i]));
            }

            key[0] = 0;
            Q.DecreaseKey(0, 0);

            while (Q.Empty() == false)
            {
                int u = Q.ExtractMin();
                // pokud má vrchol 'u' předka, je společně s ním hranou minimální kostry grafu
                if (prev[u] != -1) { spanningTree[u, prev[u]] = adjM[u, prev[u]]; spanningTree[prev[u], u] = adjM[prev[u], u]; }

                foreach (int neigh in nasled[u]) // upravuji "minimální váhy"
                {
                    if (Q.pos[neigh] != 0 && adjM[u, neigh] < key[neigh])
                    {
                        key[neigh] = adjM[u, neigh];
                        prev[neigh] = u;
                        Q.DecreaseKey(neigh, key[neigh]);
                    }
                }
            }

            return spanningTree;
        }

        public string TopologicalSort() // topologické uspořádání
        {
            void topoRecursion(int vertex, bool[] seen, List<int> reversed) // prvně nalezne vrcholy v pořadí jak končí DFS
            {
                seen[vertex] = true;

                foreach (int neigh in nasled[vertex])
                {
                    if (seen[neigh] == false) topoRecursion(neigh, seen, reversed);
                }

                reversed.Add(vertex + 1);
            }

            if (DFS().Item5.Count > 0) return "Contains a cycle!"; // zkontroluji, jestli je DAG

            string output = ""; // výsledek vrátím string vrcholů ve správném pořadí
            bool[] seen = new bool[n];
            List<int> reversed = new List<int>(); // výsledek algoritmu je tento obrácený List

            for (int i = 0; i < n; i++)
            {
                if (seen[i] == false) topoRecursion(i, seen, reversed);
            }

            for (int k = reversed.Count - 1; k >= 0; k--) // obrácený List je výsledkem
            {
                output += $"{reversed[k]} ";
            }

            return output;
        }
    }
}
