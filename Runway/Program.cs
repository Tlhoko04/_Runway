/*
 * PROJECT: Haute Couture & VIP Fashion Week Unified Master Suite (RunwayChicSuite v3.0)
 * ARCHITECTURE: Clean C# with Modern Nullable Reference Types enabled (Zero Compiler Warnings).
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RunwayChicSuite
{
    #region Domain Models & Comparers
    public enum WardrobeCategory { HauteCouture, ReadyToWear, Bridal, LuxuryAccessories }

    // Natural Sorting (IComparable): Highest retail value first
    public class RunwayPiece : IComparable<RunwayPiece>
    {
        public int LookId { get; set; }
        public string Designer { get; set; }
        public string ItemName { get; set; }
        public WardrobeCategory Category { get; set; }
        public decimal Value { get; set; }
        public int DemandScore { get; set; }

        public RunwayPiece(int id, string designer, string item, WardrobeCategory cat, decimal val, int demand)
        {
            LookId = id;
            Designer = designer;
            ItemName = item;
            Category = cat;
            Value = val;
            DemandScore = demand;
        }

        public int CompareTo(RunwayPiece? other)
        {
            if (other is null) return 1;
            return other.Value.CompareTo(this.Value);
        }

        public override bool Equals(object? obj)
        {
            if (obj is RunwayPiece other)
                return this.LookId == other.LookId && this.Designer == other.Designer;
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(LookId, Designer);

        public override string ToString() =>
            $"[Look #{LookId:D3}] {Designer,-12} | {ItemName,-22} | {Category,-17} | {Value,9:C} | Heat: {DemandScore}%";
    }

    // Custom Strategy Sorting (IComparer): Demand ascending, then Value descending
    public class PieceDemandComparer : IComparer<RunwayPiece>
    {
        public int Compare(RunwayPiece? x, RunwayPiece? y)
        {
            if (x is null && y is null) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            int demandComparison = x.DemandScore.CompareTo(y.DemandScore);
            return demandComparison != 0 ? demandComparison : y.Value.CompareTo(x.Value);
        }
    }
    #endregion

    #region Custom Linear Structures

    // Custom Static List: Fixed Front-Row "Row A" Seating
    public class StaticList<T> : IEnumerable<T>
    {
        private readonly T?[] _seats;
        public int Count { get; private set; }
        public int Capacity { get; }

        public StaticList(int capacity)
        {
            Capacity = capacity;
            _seats = new T?[capacity];
            Count = 0;
        }

        public void Add(T item)
        {
            if (Count >= Capacity)
                throw new InvalidOperationException($"Row A is full! Max capacity {Capacity} reached.");
            _seats[Count++] = item;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
                return _seats[index]!;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++) yield return _seats[i]!;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    // Custom Dynamic List: Auto-Expanding Wardrobe Rack
    public class DynamicList<T> : IEnumerable<T>
    {
        private T?[] _items;
        public int Count { get; private set; }
        public int Capacity => _items.Length;

        public DynamicList(int initialCapacity = 2)
        {
            _items = new T?[initialCapacity];
            Count = 0;
        }

        public void Add(T item)
        {
            if (Count == _items.Length)
            {
                Array.Resize(ref _items, _items.Length * 2);
            }
            _items[Count++] = item;
        }

        public bool RemoveAt(int index)
        {
            if (index < 0 || index >= Count) return false;
            for (int i = index; i < Count - 1; i++)
            {
                _items[i] = _items[i + 1];
            }
            _items[Count - 1] = default;
            Count--;
            return true;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
                return _items[index]!;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++) yield return _items[i]!;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    // Custom Doubly Linked List with Nullable Node Pointers
    public class RunwayNode<T>
    {
        public T ModelLook { get; set; }
        public RunwayNode<T>? Previous { get; set; }
        public RunwayNode<T>? Next { get; set; }

        public RunwayNode(T look)
        {
            ModelLook = look;
            Previous = null;
            Next = null;
        }
    }

    public class CustomDoublyLinkedList<T> : IEnumerable<T>
    {
        public RunwayNode<T>? Head { get; private set; }
        public RunwayNode<T>? Tail { get; private set; }
        public int Count { get; private set; }

        public void AddLast(T item)
        {
            RunwayNode<T> newNode = new RunwayNode<T>(item);
            if (Head is null)
            {
                Head = newNode;
                Tail = newNode;
            }
            else
            {
                Tail!.Next = newNode;
                newNode.Previous = Tail;
                Tail = newNode;
            }
            Count++;
        }

        public bool Remove(T item)
        {
            RunwayNode<T>? current = Head;
            while (current != null)
            {
                if (EqualityComparer<T>.Default.Equals(current.ModelLook, item))
                {
                    if (current.Previous != null)
                        current.Previous.Next = current.Next;
                    else
                        Head = current.Next;

                    if (current.Next != null)
                        current.Next.Previous = current.Previous;
                    else
                        Tail = current.Previous;

                    Count--;
                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            RunwayNode<T>? current = Head;
            while (current != null)
            {
                yield return current.ModelLook;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void PrintRunwayExitWalk()
        {
            RunwayNode<T>? current = Tail;
            while (current != null)
            {
                Console.WriteLine("  <- [Turning Back on Catwalk]: " + current.ModelLook);
                current = current.Previous;
            }
        }
    }
    #endregion

    #region Advanced Non-Linear & Abstract Structures

    // DICTIONARY: O(1) Instant SKU Query
    public class AtelierVault
    {
        private readonly Dictionary<string, string> _vault = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public void RegisterPiece(string sku, string description) => _vault[sku] = description;

        public void QueryPiece(string sku)
        {
            if (_vault.TryGetValue(sku, out string? details))
                Console.WriteLine($"  [FOUND] SKU {sku}: {details}");
            else
                Console.WriteLine($"  [NOT FOUND] SKU {sku} does not exist in the digital vault.");
        }
    }

    // STACK (LIFO): Layering Garments
    public class BackstageLayeringStack
    {
        private readonly Stack<string> _layers = new Stack<string>();

        public void LayerGarment(string item)
        {
            _layers.Push(item);
            Console.WriteLine($"  + Layered on: {item}");
        }

        public void StripLayer()
        {
            if (_layers.Count == 0)
            {
                Console.WriteLine("  Model has returned to base undergarments.");
                return;
            }
            string peeled = _layers.Pop();
            Console.WriteLine($"  - Quick-Change Stripped: {peeled}");
        }

        public void PeekOuter()
        {
            if (_layers.Count > 0)
                Console.WriteLine($"  Current visible outermost piece: {_layers.Peek()}");
        }
    }

    // QUEUE (FIFO): Hair & Makeup Scheduling
    public class HairAndMakeupQueue
    {
        private readonly Queue<string> _stylingQueue = new Queue<string>();

        public void CheckInModel(string modelName)
        {
            _stylingQueue.Enqueue(modelName);
            Console.WriteLine($"  -> Check-In: {modelName} joined the glam queue.");
        }

        public void NextToChair()
        {
            if (_stylingQueue.Count == 0)
            {
                Console.WriteLine("  No models waiting in the glam queue.");
                return;
            }
            string currentModel = _stylingQueue.Dequeue();
            Console.WriteLine($"  💄 In the Chair: {currentModel} is now getting hair & makeup done.");
        }
    }

    // BINARY SEARCH TREE (BST): Valuations
    public class TreeNode
    {
        public decimal Value { get; set; }
        public string ItemName { get; set; }
        public TreeNode? Left { get; set; }
        public TreeNode? Right { get; set; }

        public TreeNode(decimal val, string name)
        {
            Value = val;
            ItemName = name;
            Left = null;
            Right = null;
        }
    }

    public class GarmentBinarySearchTree
    {
        public TreeNode? Root { get; private set; }

        public void Insert(decimal val, string name) => Root = InsertRec(Root, val, name);

        private TreeNode InsertRec(TreeNode? root, decimal val, string name)
        {
            if (root == null) return new TreeNode(val, name);
            if (val < root.Value) root.Left = InsertRec(root.Left, val, name);
            else if (val > root.Value) root.Right = InsertRec(root.Right, val, name);
            return root;
        }

        public void PrintInOrder(TreeNode? node)
        {
            if (node == null) return;
            PrintInOrder(node.Left);
            Console.WriteLine($"  {node.Value,9:C} - {node.ItemName}");
            PrintInOrder(node.Right);
        }
    }

    // GRAPH: Adjacency List Transit Map & BFS
    public class FashionTransitGraph
    {
        private readonly Dictionary<string, List<string>> _adjList = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        public void AddCity(string city)
        {
            if (!_adjList.ContainsKey(city)) _adjList[city] = new List<string>();
        }

        public void AddFlightRoute(string from, string to)
        {
            AddCity(from);
            AddCity(to);
            _adjList[from].Add(to);
            _adjList[to].Add(from);
        }

        public void TraverseLogisticsNetwork(string startCity)
        {
            if (!_adjList.ContainsKey(startCity))
            {
                Console.WriteLine($"City {startCity} not found in transit map.");
                return;
            }

            HashSet<string> visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Queue<string> queue = new Queue<string>();

            visited.Add(startCity);
            queue.Enqueue(startCity);

            Console.WriteLine($"BFS Logistics Routing starting from {startCity}:");
            while (queue.Count > 0)
            {
                string current = queue.Dequeue();
                Console.Write($" [{current}] -> ");

                foreach (string neighbor in _adjList[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
            Console.WriteLine("TERMINUS");
        }
    }
    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "RunwayChic Master Computer Science Suite";
            bool active = true;

            while (active)
            {
                Console.Clear();
                Console.WriteLine("========================================================================");
                Console.WriteLine("        VOGUE & RUNWAY CHIC: MASTER DATA STRUCTURES & ALGORITHMS        ");
                Console.WriteLine("========================================================================");
                Console.WriteLine(" [1] Vintage Trunk Inventory (ArrayList & Type Boxing)");
                Console.WriteLine(" [2] Atelier Collection Pipeline (Generic List<T>)");
                Console.WriteLine(" [3] VIP Guest Rosters (Union & Intersect Set Algebra)");
                Console.WriteLine(" [4] Curation Filters (Predicates, Anonymous Methods, & Lambdas)");
                Console.WriteLine(" [5] Showroom Valuation Sorting (IComparable & Custom IComparer)");
                Console.WriteLine(" [6] Front-Row 'Row A' Seating (Bounded Static Array Buffer)");
                Console.WriteLine(" [7] Expanding Fitting Rack (Dynamic Growing List)");
                Console.WriteLine(" [8] Catwalk Walk Queue (Bidirectional Doubly Linked List)");
                Console.WriteLine("------------------------------------------------------------------------");
                Console.WriteLine(" [D] Digital Vault (O(1) Hash Dictionary Lookup)");
                Console.WriteLine(" [S] Quick-Change Layering (LIFO Stack)");
                Console.WriteLine(" [Q] Glam Chair Arrivals (FIFO Queue)");
                Console.WriteLine(" [T] Valuations Hierarchy (Binary Search Tree - In-Order)");
                Console.WriteLine(" [G] Global Logistics Network (Graph & Breadth-First Search)");
                Console.WriteLine(" [X] Exit Terminal");
                Console.WriteLine("========================================================================");
                Console.Write("Select Subsystem: ");

                char key = char.ToUpper(Console.ReadKey().KeyChar);
                Console.WriteLine("\n");

                switch (key)
                {
                    case '1': DemoArrayList(); break;
                    case '2': DemoListBasics(); break;
                    case '3': DemoUnionIntersect(); break;
                    case '4': DemoFunctionalDelegates(); break;
                    case '5': DemoSorting(); break;
                    case '6': DemoStaticList(); break;
                    case '7': DemoDynamicList(); break;
                    case '8': DemoDoublyLinkedList(); break;
                    case 'D': DemoDictionary(); break;
                    case 'S': DemoStack(); break;
                    case 'Q': DemoQueue(); break;
                    case 'T': DemoTree(); break;
                    case 'G': DemoGraph(); break;
                    case 'X': active = false; Console.WriteLine("Wrapping Fashion Week. Goodbye!"); break;
                    default: Console.WriteLine("Invalid selection. Press any key to retry..."); Console.ReadKey(); break;
                }

                if (active)
                {
                    Console.Write("\nPress any key to return to Main Backstage Menu...");
                    Console.ReadKey();
                }
            }
        }

        #region Method Implementations

        static void DemoArrayList()
        {
            Console.WriteLine("--- Vintage Trunk: ArrayList (Non-Generic Boxing) ---");
            ArrayList trunk = new ArrayList();
            trunk.Add(new RunwayPiece(1, "Chanel", "Quilted Flap Bag", WardrobeCategory.LuxuryAccessories, 9500m, 99));
            trunk.Add("Dior Satin Red Lipstick #999");
            trunk.Add(18);

            Console.WriteLine($"Trunk holds {trunk.Count} un-typed items as generic System.Object references:\n");
            foreach (object item in trunk)
            {
                if (item is RunwayPiece look) Console.WriteLine($"  Unboxed Piece: {look}");
                else Console.WriteLine($"  Item [{item.GetType().Name}]: {item}");
            }
        }

        static void DemoListBasics()
        {
            Console.WriteLine("--- Atelier Inventory: Generic List<T> Pipeline ---");
            List<RunwayPiece> closet = GetSampleLooks();
            Console.WriteLine($"Count: {closet.Count} | Capacity: {closet.Capacity}");

            closet.Insert(0, new RunwayPiece(100, "Schiaparelli", "Anatomical Corset", WardrobeCategory.HauteCouture, 45000m, 98));
            Console.WriteLine($"[Index 0 Insert]: Now '{closet[0].Designer} - {closet[0].ItemName}'");

            closet.Remove(closet[2]);
            Console.WriteLine($"Final Show Closet Size: {closet.Count} pieces");
        }

        static void DemoUnionIntersect()
        {
            Console.WriteLine("--- Guest Roster: Set Operations (Paris vs. Milan VIPs) ---");
            List<string> paris = new List<string> { "Zendaya", "Rihanna", "Anne Hathaway", "Dua Lipa", "Kendall Jenner" };
            List<string> milan = new List<string> { "Anne Hathaway", "Dua Lipa", "Rosé", "Gigi Hadid", "Jennie Kim" };

            Console.WriteLine("Paris Roster: " + string.Join(", ", paris));
            Console.WriteLine("Milan Roster: " + string.Join(", ", milan));

            Console.WriteLine("\n[INTERSECT] Attending Both: " + string.Join(", ", paris.Intersect(milan)));
            Console.WriteLine("[UNION] Consolidated Master Roster: " + string.Join(", ", paris.Union(milan)));
        }

        static void DemoFunctionalDelegates()
        {
            Console.WriteLine("--- Curations: Predicates, Anonymous Methods & Lambdas ---");
            List<RunwayPiece> looks = GetSampleLooks();

            Predicate<RunwayPiece> museumTier = p => p.Value >= 25000m;
            Console.WriteLine($"1. Named Predicate (Value >= R25,000): Found {looks.FindAll(museumTier).Count} pieces.");

            List<RunwayPiece> couture = looks.FindAll(delegate (RunwayPiece p) { return p.Category == WardrobeCategory.HauteCouture; });
            Console.WriteLine($"2. Anonymous Method (Haute Couture Category): Found {couture.Count} pieces.");

            List<RunwayPiece> viral = looks.FindAll(p => p.DemandScore >= 95 && p.Value > 15000m);
            Console.WriteLine($"3. Lambda Filter (Demand >= 95% & Value > R15k): Found {viral.Count} pieces:");
            viral.ForEach(l => Console.WriteLine("   " + l));
        }

        static void DemoSorting()
        {
            Console.WriteLine("--- Showroom: Multi-Strategy Sorting ---");
            List<RunwayPiece> looks = GetSampleLooks();

            Console.WriteLine("1. Default Sort (IComparable -> Highest Retail Value):");
            looks.Sort();
            looks.ForEach(l => Console.WriteLine("  " + l));

            Console.WriteLine("\n2. Custom IComparer (PieceDemandComparer -> Lowest Demand First):");
            looks.Sort(new PieceDemandComparer());
            looks.ForEach(l => Console.WriteLine("  " + l));

            Console.WriteLine("\n3. Inline Lambda Sort (Alphabetical by Designer):");
            looks.Sort((a, b) => string.Compare(a.Designer, b.Designer, StringComparison.Ordinal));
            looks.ForEach(l => Console.WriteLine("  " + l));
        }

        static void DemoStaticList()
        {
            Console.WriteLine("--- VIP Front-Row: Custom Static List (Fixed 3-Seat Buffer) ---");
            StaticList<string> rowA = new StaticList<string>(3);
            rowA.Add("Anna Wintour (Vogue)");
            rowA.Add("Zendaya");
            rowA.Add("Law Roach");

            Console.WriteLine($"Row A Occupancy: {rowA.Count}/{rowA.Capacity}");
            for (int i = 0; i < rowA.Count; i++) Console.WriteLine($"  Seat {i + 1}: {rowA[i]}");

            try
            {
                Console.WriteLine("\nAttempting to add 4th celebrity to full 3-seat buffer...");
                rowA.Add("Surprise Guest");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Buffer Protection: " + ex.Message);
                Console.ResetColor();
            }
        }

        static void DemoDynamicList()
        {
            Console.WriteLine("--- Fitting Rack: Custom Dynamic List (Auto-Doubling Capacity) ---");
            DynamicList<string> rack = new DynamicList<string>(2);
            Console.WriteLine($"Initial Rack Capacity: {rack.Capacity} slots");

            rack.Add("Silk Slip Dress");
            rack.Add("Tulle Overskirt");
            Console.WriteLine($"Count: {rack.Count} | Capacity: {rack.Capacity}");

            rack.Add("Velvet Opera Coat");
            Console.WriteLine($"Added 3rd garment -> Geometric Doubling! New Capacity: {rack.Capacity} slots");
            Console.WriteLine("Rack: " + string.Join(", ", rack));
        }

        static void DemoDoublyLinkedList()
        {
            Console.WriteLine("--- Runway Lineup: Custom Doubly Linked List (Bidirectional Queue) ---");
            CustomDoublyLinkedList<string> catwalk = new CustomDoublyLinkedList<string>();
            catwalk.AddLast("Look 01: Bella Hadid in Jacquemus");
            catwalk.AddLast("Look 02: Kendall Jenner in Mugler");
            catwalk.AddLast("Look 03: Naomi Campbell in Valentino");

            Console.WriteLine("Forward Catwalk Walk (Head to Tail via Next):");
            foreach (var step in catwalk) Console.WriteLine("  -> " + step);

            Console.WriteLine("\nBackstage Swap: Removing Look 02 from middle of queue in O(1)...");
            catwalk.Remove("Look 02: Kendall Jenner in Mugler");

            Console.WriteLine("\nGrand Finale Exit Walk (Tail to Head via Previous):");
            catwalk.PrintRunwayExitWalk();
        }

        static void DemoDictionary()
        {
            Console.WriteLine("--- Digital Vault: Dictionary (O(1) Fast Lookups) ---");
            AtelierVault vault = new AtelierVault();
            vault.RegisterPiece("SKU-9901", "Dior Silk Organza Ballgown");
            vault.RegisterPiece("SKU-9902", "Chanel Quilted Lambskin Bag");
            vault.QueryPiece("SKU-9901");
            vault.QueryPiece("SKU-0000");
        }

        static void DemoStack()
        {
            Console.WriteLine("--- Quick-Change: Stack (LIFO Layering) ---");
            BackstageLayeringStack layers = new BackstageLayeringStack();
            layers.LayerGarment("Silk Slip");
            layers.LayerGarment("Structured Velvet Corset");
            layers.LayerGarment("Faux Fur Overcoat");
            layers.PeekOuter();

            Console.WriteLine("\nQuick-change peeling off top layer:");
            layers.StripLayer();
            layers.PeekOuter();
        }

        static void DemoQueue()
        {
            Console.WriteLine("--- Glam Station: Queue (FIFO Scheduling) ---");
            HairAndMakeupQueue glamQueue = new HairAndMakeupQueue();
            glamQueue.CheckInModel("Bella Hadid");
            glamQueue.CheckInModel("Gigi Hadid");
            glamQueue.CheckInModel("Anok Yai");

            Console.WriteLine("\nCalling next models in order of arrival:");
            glamQueue.NextToChair();
            glamQueue.NextToChair();
        }

        static void DemoTree()
        {
            Console.WriteLine("--- Valuations Hierarchy: Binary Search Tree (In-Order Traversal) ---");
            GarmentBinarySearchTree bst = new GarmentBinarySearchTree();
            bst.Insert(18000m, "Gucci Velvet Suit");
            bst.Insert(9500m, "Prada Cleo Bag");
            bst.Insert(42000m, "Schiaparelli Haute Gown");
            bst.Insert(12500m, "Saint Laurent Leather Blazer");

            Console.WriteLine("In-Order Traversal (Guaranteed Ascending Sorted Order):");
            bst.PrintInOrder(bst.Root);
        }

        static void DemoGraph()
        {
            Console.WriteLine("--- Global Tour: Graph Network & BFS Logistics Routing ---");
            FashionTransitGraph transit = new FashionTransitGraph();
            transit.AddFlightRoute("New York", "London");
            transit.AddFlightRoute("London", "Milan");
            transit.AddFlightRoute("London", "Paris");
            transit.AddFlightRoute("Milan", "Paris");
            transit.AddFlightRoute("Paris", "Tokyo");

            transit.TraverseLogisticsNetwork("New York");
        }

        private static List<RunwayPiece> GetSampleLooks()
        {
            return new List<RunwayPiece>
            {
                new RunwayPiece(101, "Versace", "Liquid Gold Slip Dress", WardrobeCategory.ReadyToWear, 18500m, 96),
                new RunwayPiece(102, "Valentino", "Pink PP Silk Gown", WardrobeCategory.HauteCouture, 32000m, 99),
                new RunwayPiece(103, "Mugler", "Spiral Tailored Trousers", WardrobeCategory.ReadyToWear, 11200m, 88),
                new RunwayPiece(104, "Vivienne Westwood", "Silk Taffeta Bridal Robe", WardrobeCategory.Bridal, 27500m, 97),
                new RunwayPiece(105, "Chanel", "Tweed Mini Bag w/ Pearls", WardrobeCategory.LuxuryAccessories, 9800m, 92)
            };
        }
        #endregion
    }
}