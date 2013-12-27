using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using GraphX.Controls;
using GraphX.DesignerExampleData;
using GraphX.GraphSharp.Algorithms.EdgeRouting;
using GraphX.GraphSharp.Algorithms.Layout;
using GraphX.GraphSharp.Algorithms.OverlapRemoval;
using GraphX.Models;
using Microsoft.Win32;
using QuickGraph;
using YAXLib;

namespace GraphX
{
    public class GraphArea<TVertex, TEdge, TGraph>  : GraphAreaBase, IDisposable
        where TVertex : VertexBase
        where TEdge : EdgeBase<TVertex>
        where TGraph : BidirectionalGraph<TVertex, TEdge>
    { 

        #region My properties

        #region AlgoithmFactory
        /// <summary>
        /// Provides different algorithm creation methods
        /// </summary>
        public AlgorithmFactory<TVertex, TEdge, TGraph> AlgorithmFactory { get; private set; }

        #endregion

        #region AlgorithmStorage
        /// <summary>
        /// Get algorithm storage that contain all currently defined algorithms by type (default or external)
        /// </summary>
        private AlgorithmStorage<TVertex, TEdge> AlgorithmStorage { get; set; }
        #endregion

        #region IsEdgeRoutingEnabled
        /// <summary>
        /// Value overloaded for extensibility purposes. Indicates if ER will be performed on Compute().
        /// </summary>
        public override bool IsEdgeRoutingEnabled
        {
            get
            {
                return (ExternalEdgeRoutingAlgorithm == null && DefaultEdgeRoutingAlgorithm != EdgeRoutingAlgorithmTypeEnum.None) || ExternalEdgeRoutingAlgorithm != null;
            }
        }
        #endregion

        #region StateStorage
        /// <summary>
        /// Provides methods for saving and loading graph layout states
        /// </summary>
        public StateStorage<TVertex, TEdge, TGraph> StateStorage { get; private set; }
        #endregion

        Dictionary<TEdge, EdgeControl> _edgeslist = new Dictionary<TEdge, EdgeControl>();
        Dictionary<TVertex, VertexControl> _vertexlist = new Dictionary<TVertex, VertexControl>();

        /// <summary>
        /// Added edge controls FOR READ ONLY
        /// </summary>
        public IDictionary<TEdge, EdgeControl> EdgesList
        {
            get { return _edgeslist; }
        }
        /// <summary>
        /// Added vertex controls FOR READ ONLY
        /// </summary>
        public IDictionary<TVertex, VertexControl> VertexList
        {
            get { return _vertexlist; }
        }

        /// <summary>
        /// Main graph object
        /// </summary>
        public TGraph Graph { get; set; }

        /// <summary>
        /// Gets or sets edge curving technique for smoother edges. Default value is false.
        /// </summary>
        public override bool EdgeCurvingEnabled { get; set; }

        /// <summary>
        /// This is roughly the length of each line segment in the polyline
        /// approximation to a continuous curve in WPF units.  The smaller the
        /// number the smoother the curve, but slower the performance. Default is 8.
        /// </summary>
        public override double EdgeCurvingTolerance { get; set; }

        /// <summary>
        /// Radius of a self-loop edge, which is drawn as a circle. Default is 20.
        /// </summary>
        public override double EdgeSelfLoopCircleRadius { get; set; }

        /// <summary>
        /// Offset from the corner of the vertex. Useful for custom vertex shapes. Default is 10,10.
        /// </summary>
        public override Point EdgeSelfLoopCircleOffset { get { return _edgeselfloopoffset; } set { _edgeselfloopoffset = value; } }
        private Point _edgeselfloopoffset = new Point(10,10);

        /// <summary>
        /// Show self looped edges on vertices. Default value is true.
        /// </summary>
        public override bool EdgeShowSelfLooped { get; set; }

        /// <summary>
        /// *WIP*
        /// Gets or sets if methods such as Add/Remove Vertex/Edge should also update Graph property allowing Graph data auto maintainance. 
        /// Note that if Graph property is null and this var value is True then an exception will be thrown while trying to use specified methods.
        /// Default value is True.
        /// </summary>
        private bool UpdateGraphOnDataOperations { get; set; }

        #endregion

        #region Dependency properties

        #region DP - LayoutAlgorithm
        public static readonly DependencyProperty LayoutAlgorithmProperty = DependencyProperty.Register("LayoutAlgorithm", typeof(ILayoutAlgorithm<TVertex,TEdge,TGraph>),
                                        typeof(GraphArea<TVertex, TEdge, TGraph>), new PropertyMetadata(null));
        /// <summary>
        ///Gets or the layout calculation algorithm.
        /// </summary>
        public ILayoutAlgorithm<TVertex, TEdge, TGraph> LayoutAlgorithm
        {
            get { return (ILayoutAlgorithm<TVertex, TEdge, TGraph>)GetValue( LayoutAlgorithmProperty ); }
            set { SetValue(LayoutAlgorithmProperty, value); }
        }
        #endregion

        #region DP - ExternalOverlapRemovalAlgorithm
        public static readonly DependencyProperty ExternalOverlapRemovalAlgorithmProperty = DependencyProperty.Register("ExternalOverlapRemovalAlgorithm", typeof(IExternalOverlapRemoval<TVertex>),
                                        typeof(GraphArea<TVertex, TEdge, TGraph>), new PropertyMetadata(null));
        /// <summary>
        ///Gets or sets user-defined external vertex overlap removal algorithm that implements IOverlapRemovalAlgorithm interface
        ///This one will be used instead of default if specified
        /// </summary>
        public IExternalOverlapRemoval<TVertex> ExternalOverlapRemovalAlgorithm
        {
            get { return (IExternalOverlapRemoval<TVertex>)GetValue(ExternalOverlapRemovalAlgorithmProperty); }
            set { SetValue(ExternalOverlapRemovalAlgorithmProperty, value); }
        }
        #endregion

        #region DP - ExternalEdgeRoutingAlgorithm
        public static readonly DependencyProperty ExternalEdgeRoutingAlgorithmProperty = DependencyProperty.Register("ExternalEdgeRoutingAlgorithm", typeof(IExternalEdgeRouting<TVertex, TEdge>),
                                        typeof(GraphArea<TVertex, TEdge, TGraph>), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets external edge routing algorithm
        /// </summary>
        public IExternalEdgeRouting<TVertex, TEdge> ExternalEdgeRoutingAlgorithm
        {
            get { return (IExternalEdgeRouting<TVertex, TEdge>)GetValue(ExternalEdgeRoutingAlgorithmProperty); }
            set { SetValue(ExternalEdgeRoutingAlgorithmProperty, value); }
        }
        #endregion
        
        #region DP - DefaulOverlapRemovalAlgorithm
        public static readonly DependencyProperty DefaultOverlapRemovalAlgorithmProperty = DependencyProperty.Register("DefaultOverlapRemovalAlgorithm", typeof(OverlapRemovalAlgorithmTypeEnum),
                                        typeof(GraphArea<TVertex, TEdge, TGraph>), new PropertyMetadata(OverlapRemovalAlgorithmTypeEnum.FSA));
        /// <summary>
        /// Gets or sets default overlap removal algorithm type
        /// </summary>
        public OverlapRemovalAlgorithmTypeEnum DefaultOverlapRemovalAlgorithm
        {
            get { return (OverlapRemovalAlgorithmTypeEnum)GetValue(DefaultOverlapRemovalAlgorithmProperty); }
            set { SetValue(DefaultOverlapRemovalAlgorithmProperty, value); }
        }
        #endregion

        #region DP - DefaultEdgeRoutingAlgorithm
        public static readonly DependencyProperty DefaultEdgeRoutingAlgorithmProperty = DependencyProperty.Register("DefaultEdgeRoutingAlgorithm", typeof(EdgeRoutingAlgorithmTypeEnum),
                                        typeof(GraphArea<TVertex, TEdge, TGraph>), new PropertyMetadata(EdgeRoutingAlgorithmTypeEnum.None));
        /// <summary>
        /// Gets or sets default edge routing algorithm type
        /// </summary>
        public EdgeRoutingAlgorithmTypeEnum DefaultEdgeRoutingAlgorithm
        {
            get { return (EdgeRoutingAlgorithmTypeEnum)GetValue(DefaultEdgeRoutingAlgorithmProperty); }
            set { SetValue(DefaultEdgeRoutingAlgorithmProperty, value); }
        }
        #endregion

        #region DP - DefaulOverlapRemovalAlgorithmParams
        public static readonly DependencyProperty DefaultOverlapRemovalAlgorithmParamsProperty = DependencyProperty.Register("DefaultOverlapRemovalAlgorithmParams", typeof(IOverlapRemovalParameters),
                                        typeof(GraphArea<TVertex, TEdge, TGraph>), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets default OR algorithm parameters
        /// </summary>
        public IOverlapRemovalParameters DefaultOverlapRemovalAlgorithmParams
        {
            get { return (IOverlapRemovalParameters)GetValue(DefaultOverlapRemovalAlgorithmParamsProperty); }
            set { SetValue(DefaultOverlapRemovalAlgorithmParamsProperty, value); }
        }
        #endregion

        #region DP - DefaultEdgeRoutingAlgorithmParams
        public static readonly DependencyProperty DefaultEdgeRoutingAlgorithmParamsProperty = DependencyProperty.Register("DefaultEdgeRoutingAlgorithmParams", typeof(IEdgeRoutingParameters),
                                        typeof(GraphArea<TVertex, TEdge, TGraph>), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets default ER algorithm parameters
        /// </summary>
        public IEdgeRoutingParameters DefaultEdgeRoutingAlgorithmParams
        {
            get { return (IEdgeRoutingParameters)GetValue(DefaultEdgeRoutingAlgorithmParamsProperty); }
            set { SetValue(DefaultEdgeRoutingAlgorithmParamsProperty, value); }
        }
        #endregion

        #region DP - AsyncAlgorithmCompute
        public static readonly DependencyProperty AsyncComputeProperty =
            DependencyProperty.Register("AsyncAlgorithmCompute", typeof(bool), typeof(GraphArea<TVertex, TEdge, TGraph>), new UIPropertyMetadata(false));

        /// <summary>
        /// Compute all algorithms in a separate thread
        /// </summary>
        public bool AsyncAlgorithmCompute
        {
            get { return (bool)GetValue(AsyncComputeProperty); }
            set { SetValue(AsyncComputeProperty, value); }
        }
        #endregion

        #endregion

        public GraphArea()
        {
            Graph = (TGraph)Activator.CreateInstance(typeof(TGraph));
            AlgorithmFactory = new AlgorithmFactory<TVertex, TEdge, TGraph>();
            AlgorithmStorage = new AlgorithmStorage<TVertex, TEdge>( null, null);
            StateStorage = new StateStorage<TVertex, TEdge, TGraph>(this);

            EdgeCurvingTolerance = 8;
            EdgeSelfLoopCircleRadius = 10;
            EdgeShowSelfLooped = true;
            UpdateGraphOnDataOperations = false;

            #region Designer Data
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                //var dic = new ResourceDictionary() { Source = new Uri(@"pack://application:,,,/GraphX;component/Themes/Generic.xaml") };
                //Resources.MergedDictionaries.Clear();
                //Resources.MergedDictionaries.Add(dic);

                var vc = new VertexDataExample(1, "Johnson B.C");
                var ctrl = new VertexControl(vc) { RootArea = this };
                SetX(ctrl, 0, true); SetY(ctrl, 0, true);
                Children.Add(ctrl);

                var vc2 = new VertexDataExample(2, "Manson J.C");
                var ctrl2 = new VertexControl(vc2) { RootArea = this };
                SetX(ctrl2, 800, true); SetY(ctrl2, 0, true);
                Children.Add(ctrl2);

                var vc3 = new VertexDataExample(1, "Franklin A.J");
                var ctrl3 = new VertexControl(vc3) { RootArea = this };
                SetX(ctrl3, 400, true); SetY(ctrl3, 800, true);
                Children.Add(ctrl3);

                UpdateLayout();
                var edge = new EdgeDataExample<VertexDataExample>(vc, vc2, 1) { Text = "One" };
                var edgectrl = new EdgeControl(ctrl, ctrl2, edge);
                Children.Add(edgectrl);

                edge = new EdgeDataExample<VertexDataExample>(vc2, vc3, 1) { Text = "Two" };
                edgectrl = new EdgeControl(ctrl2, ctrl3, edge);
                Children.Add(edgectrl);

                edge = new EdgeDataExample<VertexDataExample>(vc3, vc, 1) { Text = "Three" };
                edgectrl = new EdgeControl(ctrl3, ctrl, edge);
                Children.Add(edgectrl);
            }
            #endregion
        }

        #region Edge & vertex controls operations

        /// <summary>
        /// Sets drag mode for all vertices
        /// </summary>
        /// <param name="isEnabled">Is drag mode enabled</param>
        /// <param name="updateEdgesOnMove">Is edges update enabled while dragging (use this if you have edge routing algorithms enabled)</param>
        public void SetVerticesDrag(bool isEnabled, bool updateEdgesOnMove = false)
        {
            foreach (var item in VertexList)
            {
                DragBehaviour.SetIsDragEnabled(item.Value, isEnabled);
                DragBehaviour.SetUpdateEdgesOnMove(item.Value, updateEdgesOnMove);
            }
        }

        /// <summary>
        /// Sets math shape for all vertices
        /// </summary>
        /// <param name="shape">Selected math shape</param>
        public void SetVerticesMathShape(VertexShape shape)
        {
            foreach (var item in VertexList)
                item.Value.MathShape = shape;
        }

        /// <summary>
        /// Returns all existing VertexControls added into the layout.
        /// </summary>
        /// <returns></returns>
        public override ICollection<VertexControl> GetAllVertexControls() { return VertexList.Values; }

        #region Remove controls

        /// <summary>
        /// Remove all visual vertices
        /// </summary>
        public void RemoveAllVertices()
        {
            foreach (var item in _vertexlist)
            {
                if (DeleteAnimation != null)
                    DeleteAnimation.AnimateVertex(item.Value);
                else
                {
                    item.Value.Clean();
                    Children.Remove(item.Value);
                }
            }
            _vertexlist.Clear();
        }

        /// <summary>
        /// Remove all visual edges
        /// </summary>
        public void RemoveAllEdges()
        {
            foreach (var item in _edgeslist)
            {
                if (DeleteAnimation != null)
                    DeleteAnimation.AnimateEdge(item.Value);
                else
                {
                    item.Value.Clean();
                    Children.Remove(item.Value);
                }
            }
            _edgeslist.Clear();
        }

        /// <summary>
        /// Remove vertex from layout
        /// </summary>
        /// <param name="vertexData">Vertex data object</param>
        public void RemoveVertex(TVertex vertexData)
        {
            if (UpdateGraphOnDataOperations && Graph == null) throw new GX_InvalidDataException("RemoveVertex() -> UpdateGraphOnDataOperations property is set and Graph property is null!");
            if (vertexData == null || !_vertexlist.ContainsKey(vertexData)) return;

            VertexControl ctrl = _vertexlist[vertexData];
            _vertexlist.Remove(vertexData);

            if (DeleteAnimation != null)
                DeleteAnimation.AnimateVertex(ctrl);
            else
            {
                Children.Remove(ctrl);
                ctrl.Clean();
            }
        }

        /// <summary>
        /// Remove edge from layout
        /// </summary>
        /// <param name="edgeData">Edge data object</param>
        public void RemoveEdge(TEdge edgeData)
        {
            if (UpdateGraphOnDataOperations && Graph == null) throw new GX_InvalidDataException("RemoveEdge() -> UpdateGraphOnDataOperations property is set and Graph property is null!");
            if (edgeData == null || !_edgeslist.ContainsKey(edgeData)) return;

            var ctrl = _edgeslist[edgeData];
            _edgeslist.Remove(edgeData);
            
            if (DeleteAnimation != null)
                DeleteAnimation.AnimateEdge(ctrl);
            else
            {
                Children.Remove(ctrl);
                ctrl.Clean();
            }
            //if (UpdateGraphOnDataOperations && Graph != null)
            //    if (Graph.ContainsEdge(edgeData))
            //        Graph.RemoveEdge(edgeData);
        }
        #endregion

        #region Add controls
        /// <summary>
        /// Add vertex to layout
        /// </summary>
        /// <param name="vertexData">Vertex data object</param>
        /// <param name="vertexControl">Vertex visual control object</param>
        public void AddVertex(TVertex vertexData, VertexControl vertexControl)
        {
            //if (UpdateGraphOnDataOperations && Graph == null) throw new GX_InvalidDataException("AddVertex() -> UpdateGraphOnDataOperations property is set and Graph property is null!");
            if (vertexControl == null || vertexData == null) return;
            vertexControl.RootArea = this;
            if (_vertexlist.ContainsKey(vertexData)) throw new GX_InvalidDataException("AddVertex() -> Vertex with the same data has already been added to layout!"); 
            _vertexlist.Add(vertexData, vertexControl);
            Children.Add(vertexControl);
            if (UpdateGraphOnDataOperations)
            {
                if (Graph.ContainsVertex(vertexData)) throw new GX_InvalidDataException("AddVertex() -> Vertex data already exists in the Graph! Can't add it automatically!");
                else Graph.AddVertex(vertexData);
            }
        }



        /// <summary>
        /// Add an edge to layout. Edge is added into the end of the visual tree causing it to be rendered above all vertices.
        /// </summary>
        /// <param name="edgeData">Edge data object</param>
        /// <param name="edgeControl">Edge visual control</param>
        public void AddEdge(TEdge edgeData, EdgeControl edgeControl)
        {
            if (UpdateGraphOnDataOperations && Graph == null) throw new GX_InvalidDataException("AddEdge() -> UpdateGraphOnDataOperations property is set and Graph property is null!");
            if (edgeControl == null || edgeData == null) return;
            if (_edgeslist.ContainsKey(edgeData)) throw new GX_InvalidDataException("AddEdge() -> An edge with the same data has already been added to layout!"); 
            edgeControl.RootArea = this;
            _edgeslist.Add(edgeData, edgeControl);
            Children.Add(edgeControl);
            if (UpdateGraphOnDataOperations)
            {
                if (Graph.ContainsEdge(edgeData)) throw new GX_InvalidDataException("AddEdge() -> Edge data already exists in the Graph! Can't add it automatically!");
                else Graph.AddEdge(edgeData);
            }
        }

        /// <summary>
        /// Insert an edge to layout at specified position. By default, edge is inserted into the begining of the visual tree causing it to be rendered below all of the vertices.
        /// </summary>
        /// <param name="edgeData">Edge data object</param>
        /// <param name="edgeControl">Edge visual control</param>
        /// <param name="num">Insert position</param>
        public void InsertEdge(TEdge edgeData, EdgeControl edgeControl, int num = 0)
        {
            if (UpdateGraphOnDataOperations && Graph == null) throw new GX_InvalidDataException("InsertEdge() -> UpdateGraphOnDataOperations property is set and Graph property is null!");
            if (edgeControl == null || edgeData == null) return;
            if (_edgeslist.ContainsKey(edgeData)) throw new GX_InvalidDataException("AddEdge() -> An edge with the same data has already been added!"); 
            edgeControl.RootArea = this;
            _edgeslist.Add(edgeData, edgeControl);
            try
            {
                Children.Insert(num, edgeControl);
            }
            catch (Exception ex)
            {
                throw new GX_GeneralException(ex.Message + ". Probably you have an error in edge template.");
            }
            if (UpdateGraphOnDataOperations)
            {
                if (Graph.ContainsEdge(edgeData)) throw new GX_InvalidDataException("InsertEdge() -> Edge data already exists in the Graph! Can't add it automatically!");
                else Graph.AddEdge(edgeData);
            }
        }
        #endregion

        #endregion

        #region Automatic data ID storage and resolving
        private int dataIdCounter;
        private int getNextUniqueID()
        {
            while (dataIdsCollection.Contains(dataIdCounter))
            {
                dataIdCounter++;
            }
            dataIdsCollection.Add(dataIdCounter);
            return dataIdCounter;
        }

        private HashSet<int> dataIdsCollection = new HashSet<int>();

        #endregion

        #region GenerateGraph

        #region Sizes operations
        /// <summary>
        /// Get vertex control sizes
        /// </summary>
        public Dictionary<TVertex, Size> GetVertexSizes()
        {          
            //measure if needed and get all vertex sizes
            if (!IsMeasureValid) Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var vertexSizes = new Dictionary<TVertex, Size>(VertexList.Count);
            //go through the vertex presenters and get the actual layout positions
            foreach (var vc in VertexList) vertexSizes[vc.Key] = new Size(vc.Value.ActualWidth, vc.Value.ActualHeight);
            return vertexSizes;
        }

        /// <summary>
        /// Get visual vertex size rectangles (can be used by some algorithms)
        /// </summary>
        /// <param name="positions">Vertex positions collection (auto filled if null)</param>
        /// <param name="vertexSizes">Vertex sizes collection (auto filled if null)</param>
        public Dictionary<TVertex, Rect> GetVertexSizeRectangles(IDictionary<TVertex, Point> positions = null, Dictionary<TVertex, Size> vertexSizes = null)
        {
            if (vertexSizes == null) vertexSizes = GetVertexSizes();
            if (positions == null) positions = GetVertexPositions();
            var rectangles = new Dictionary<TVertex, Rect>();
            foreach (var vertex in Graph.Vertices)
            {
                Point position; Size size;
                if (!positions.TryGetValue(vertex, out position) || !vertexSizes.TryGetValue(vertex, out size)) continue;
                rectangles[vertex] = new Rect(position.X - size.Width * (float)0.5, position.Y - size.Height * (float)0.5, size.Width, size.Height);
            }
            return rectangles;
        }

        /// <summary>
        /// Returns all vertices positions list
        /// </summary>
        public Dictionary<TVertex, Point> GetVertexPositions()
        {
            var points = new Dictionary<TVertex, Point>();
            foreach (var vertex in VertexList)
                points.Add(vertex.Key, vertex.Value.GetPosition());
            return points;
        }

        #endregion

        #region PreloadVertexes()
        /// <summary>
        /// Preload visual vertexes - can be used for custom external algorithm usages
        /// </summary>
        /// <param name="graph">Data graph</param>
        /// <param name="mainVertex">Optional main vertex</param>
        /// <param name="dataContextToDataItem">Sets DataContext property to vertex data item of the control</param>
        public void PreloadVertexes(TGraph graph, bool dataContextToDataItem = true)
        {
            //clear edge and vertex controls
            RemoveAllVertices();
            RemoveAllEdges();

            //preload vertex controls
            foreach (var it in graph.Vertices)
            {
                var vc = new VertexControl(it) { DataContext = dataContextToDataItem ? it : null, Visibility = System.Windows.Visibility.Hidden }; // make them invisible (there is no layout positions yet calculated)
                AddVertex(it, vc);
            }
            //assign graph
            Graph = graph;
        }
        #endregion

        #region RelayoutGraph()
        private void _relayoutGraph()
        {
            if (VertexList.Count == 0 || Graph == null) return; // no vertexes == no edges
            Dictionary<TVertex, Size> vertexSizes = null;
            Dictionary<TVertex, Point> vertexPos = null;

            ILayoutAlgorithm<TVertex,TEdge,TGraph> alg = null; //layout algorithm
            Dictionary<TVertex, Rect> rectangles = null; //rectangled size data
            IExternalOverlapRemoval<TVertex> overlap = null;//overlap removal algorithm
            IExternalEdgeRouting<TVertex, TEdge> eralg = null;

            var dispatcher = EnableWinFormsHostingMode ? Dispatcher : Application.Current.Dispatcher;

            dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                UpdateLayout(); // Update layout so we can get actual control sizes
                vertexSizes = GetVertexSizes();
                vertexPos = GetVertexPositions();

                //setup layout algorithm
                alg = LayoutAlgorithm;
                if( alg == null ) alg = new RandomLayoutAlgorithm<TVertex,TEdge,TGraph>();

                //setup overLap removal algorithm
                //OR - if default OR algo selected and enabled or we are using external OR algo
                if( ExternalOverlapRemovalAlgorithm != null || DefaultOverlapRemovalAlgorithm != OverlapRemovalAlgorithmTypeEnum.None )
                {
                    //setup overlap removal algorithm
                    if (ExternalOverlapRemovalAlgorithm == null)
                    {
                        // create default OR
                        overlap = AlgorithmFactory.CreateOverlapRemovalAlgorithm(DefaultOverlapRemovalAlgorithm, null, DefaultOverlapRemovalAlgorithmParams);
                    }
                    else
                    {
                        overlap = ExternalOverlapRemovalAlgorithm;
                    }
                }

                //Edge Routing algorithm
                if (ExternalEdgeRoutingAlgorithm == null && DefaultEdgeRoutingAlgorithm != EdgeRoutingAlgorithmTypeEnum.None)
                {
                    eralg = AlgorithmFactory.CreateEdgeRoutingAlgorithm( DefaultEdgeRoutingAlgorithm, new Rect(DesiredSize), Graph, null, null, DefaultEdgeRoutingAlgorithmParams);
                }
                else if (ExternalEdgeRoutingAlgorithm != null)
                {
                    eralg = ExternalEdgeRoutingAlgorithm;
                    //eralg.VertexPositions = resultCoords;
                    // eralg.VertexSizes = rectangles;
                }
            }));
            if (alg == null) return;
            var resultCoords = alg.Compute( new CancellationToken(), Graph, 
                                                v => { Point p; return vertexPos.TryGetValue( v, out p ) ? p : new Point( Double.NaN, Double.NaN ); }, 
                                                v => { Size s; return vertexSizes.TryGetValue( v, out s ) ? s : new Size( Double.NaN, Double.NaN ); } );
            if (_worker != null) _worker.ReportProgress(33, 0);

            //overlap removal
            if (overlap != null)
            {
                //generate rectangle data from sizes
                dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    rectangles = GetVertexSizeRectangles(resultCoords, vertexSizes);
                }));
                overlap.Rectangles = rectangles;
                overlap.Compute();
                resultCoords = new Dictionary<TVertex, Point>();
                foreach (var res in overlap.Rectangles)
                    resultCoords.Add(res.Key, new Point((res.Value.Left + res.Value.Size.Width * 0.5), (res.Value.Top + res.Value.Size.Height * 0.5)));
                if (_worker != null) _worker.ReportProgress(66, 1);
            }


            dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                AlgorithmStorage = new AlgorithmStorage<TVertex, TEdge>(overlap, eralg);

                if (MoveAnimation != null)
                {
                    MoveAnimation.CleanupBaseData();
                    MoveAnimation.Cleanup();
                }
                //setup vertex positions from result data
                foreach (var item in resultCoords)
                {
                    if (!VertexList.ContainsKey(item.Key)) continue;
                    var vc = VertexList[item.Key];

                    GraphAreaBase.SetFinalX(vc, item.Value.X);
                    GraphAreaBase.SetFinalY(vc, item.Value.Y);

                    if (MoveAnimation == null || double.IsNaN(GraphAreaBase.GetX(vc)))
                        vc.SetPosition(item.Value, false);
                    else MoveAnimation.AddVertexData(vc, item.Value);
                    vc.Visibility = System.Windows.Visibility.Visible; //show vertexes with layout positions assigned
                }
                if (MoveAnimation != null) 
                {
                    if (MoveAnimation.VertexStorage.Count > 0)
                        MoveAnimation.RunVertexAnimation();

                    foreach (var item in EdgesList.Values)
                        MoveAnimation.AddEdgeData(item);
                    if (MoveAnimation.EdgeStorage.Count > 0)
                        MoveAnimation.RunEdgeAnimation();
                }
                UpdateLayout(); //need to update before edge routing
            }));

            //Edge Routing
            if (eralg != null)
            {
                dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    var size = Parent is ZoomControl ? (Parent as ZoomControl).Presenter.ContentSize : DesiredSize;
                    eralg.AreaRectangle = new Rect(TopLeft.X, TopLeft.Y, size.Width, size.Height);
                    rectangles = GetVertexSizeRectangles(resultCoords, vertexSizes);
                }));
                eralg.VertexPositions = resultCoords;
                eralg.VertexSizes = rectangles;
                eralg.Compute();
                if (eralg.EdgeRoutes != null)
                    foreach (var item in eralg.EdgeRoutes)
                        item.Key.RoutingPoints = item.Value;
                if (_worker != null) _worker.ReportProgress(99, 1);
                dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    UpdateLayout();
                }));
            }
        }

        protected BackgroundWorker _worker;
        /// <summary>
        /// Relayout graph using the same vertexes
        /// </summary>
        /// <param name="generateAllEdges">Generate all available edges for graph</param>
        public void RelayoutGraph(bool generateAllEdges = false)
        {
            _relayoutGraphMain(generateAllEdges);
        }


        private void _relayoutGraphMain(bool generateAllEdges = false, bool standalone = true)
        {
            if (AsyncAlgorithmCompute)
            {
                CancelRelayout();
                _worker = new BackgroundWorker
                {
                    WorkerSupportsCancellation = true,
                    WorkerReportsProgress = true
                };
                _worker.DoWork += ((sender, e) =>
                       {
                           _relayoutGraph();
                       });
                _worker.ProgressChanged += ((s, e) =>
                        {
                            int value = (int)e.UserState;
                            switch (value)
                            {
                                case 0: OnLayoutCalculationFinished(); break;
                                case 1: OnOverlapRemovalCalculationFinished(); break;
                                case 2: OnEdgeRoutingCalculationFinished(); break;
                            }
                        });
                _worker.RunWorkerCompleted += ((s, e) =>
                        {
                            if (generateAllEdges)
                            {
                                if (EdgesList.Count == 0) GenerateAllEdges(System.Windows.Visibility.Visible);
                                else UpdateAllEdges();
                            }
                            if (!standalone) OnGenerateGraphFinished();
                            else OnRelayoutFinished();

                            _worker = null;
                        });
                _worker.RunWorkerAsync();
            }
            else
            {
                _relayoutGraph();
                if (generateAllEdges)
                {
                    if (EdgesList.Count == 0) GenerateAllEdges(System.Windows.Visibility.Visible);
                    else UpdateAllEdges();
                }
                if (!standalone) OnGenerateGraphFinished();
                else OnRelayoutFinished();
            }  
        }
        #endregion

        /// <summary>
        /// Cancel all undergoing async calculations
        /// </summary>
        public void CancelRelayout()
        {
            if (_worker != null && _worker.IsBusy && _worker.WorkerSupportsCancellation)
                _worker.CancelAsync();
        }

        /// <summary>
        /// Generate visual graph
        /// </summary>
        /// <param name="graph">Data graph</param>
        /// <param name="generateAllEdges">Generate all available edges for graph</param>
        /// <param name="autoAssignMissingDataID">Generate missing IDs for edge and vertex data supplied in "graph" param</param>
        /// <param name="dataContextToDataItem">Sets visual edge and vertex controls DataContext property to vertex data item of the control (Allows prop binding in xaml templates)</param>
        public void GenerateGraph(TGraph graph, bool generateAllEdges = false, bool autoAssignMissingDataID = true, bool dataContextToDataItem = true)
        {
            if (autoAssignMissingDataID)
            {
                autoresolveIds(graph);
            }

            PreloadVertexes(graph, dataContextToDataItem);
            _relayoutGraphMain(generateAllEdges, false);
        }

        /// <summary>
        /// Generate visual graph using Graph property (it must be set before this method is called)
        /// </summary>
        /// <param name="generateAllEdges">Generate all available edges for graph</param>
        /// <param name="autoAssignMissingDataID">Generate missing IDs for edge and vertex data supplied in "graph" param</param>
        /// <param name="dataContextToDataItem">Sets visual edge and vertex controls DataContext property to vertex data item of the control (Allows prop binding in xaml templates)</param>
        public void GenerateGraph(bool generateAllEdges = false, bool autoAssignMissingDataID = true, bool dataContextToDataItem = true)
        {
            if (Graph == null)
                throw new InvalidDataException("GraphArea.GenerateGraph() -> Graph property is null while trying to generate graph!");
            else GenerateGraph(Graph, generateAllEdges, autoAssignMissingDataID, dataContextToDataItem);
        }

        private void autoresolveIds(TGraph graph = null)
        {
            if (graph == null) graph = Graph;
            if (graph == null) return;
            dataIdsCollection.Clear();
            dataIdCounter = 1;
            int count = graph.Vertices.Count();
            TVertex element;
            for (int i = 0; i < count; i++)
            {
                element = graph.Vertices.ElementAt(i);
                if (element.ID != -1 && !dataIdsCollection.Contains(element.ID)) 
                    dataIdsCollection.Add(element.ID);
            }
            foreach (var item in graph.Vertices.Where(a => a.ID == -1))
                item.ID = getNextUniqueID();

            dataIdsCollection.Clear();
            dataIdCounter = 1;
            count = graph.Edges.Count();
            TEdge element2;
            for (int i = 0; i < count; i++)
            {
                element2 = graph.Edges.ElementAt(i);
                if (element2.ID != -1 && !dataIdsCollection.Contains(element2.ID))
                    dataIdsCollection.Add(element2.ID);
            }
            foreach (var item in graph.Edges.Where(a => a.ID == -1))
                item.ID = getNextUniqueID();
        }
        #endregion 

        #region Generate Edges (ForVertex, All ... and stuff)

        #region ComputeEdgeRoutesByVertex()
        /// <summary>
        /// Compute new edge routes for all edges of the vertex
        /// </summary>
        /// <param name="vc">Vertex visual control</param>
        /// <param name="VertexDataNeedUpdate">If vertex data inside edge routing algorthm needs to be updated</param>
        public override void ComputeEdgeRoutesByVertex(VertexControl vc, bool vertexDataNeedUpdate = true)
        {
            if (vc == null) return;
            var vdata = vc.Vertex as TVertex;
            if (vdata == null) return;
            var list = new List<TEdge>();
            IEnumerable<TEdge> edges;
            Graph.TryGetInEdges(vdata, out edges);
            list.AddRange(edges);
            Graph.TryGetOutEdges(vdata, out edges);
            list.AddRange(edges);

            if( vertexDataNeedUpdate )
            {
                var position = vc.GetPosition();
                var size = new Rect(position.X - vc.ActualWidth * (float)0.5, position.Y - vc.ActualHeight * (float)0.5, vc.ActualWidth, vc.ActualHeight);
                AlgorithmStorage.EdgeRouting.UpdateVertexData(vdata, position, size);
            }

            foreach (var item in list)
                item.RoutingPoints = AlgorithmStorage.EdgeRouting.ComputeSingle(item);
        }
        #endregion

        #region GenerateAllEdges()
        /// <summary>
        /// Generates all possible valid edges for Graph vertexes
        /// </summary>
        /// <param name="defaultVisibility">Default edge visibility on layout</param>
        public void GenerateAllEdges(Visibility defaultVisibility = System.Windows.Visibility.Visible)
        {
            RemoveAllEdges();
            foreach (var item in Graph.Edges)
            {
                if(item.Source == null || item.Target == null) continue;
                if (!VertexList.ContainsKey(item.Source) || !VertexList.ContainsKey(item.Target)) continue;
                var edgectrl = new EdgeControl(VertexList[item.Source], VertexList[item.Target], item) { Visibility = defaultVisibility };
                InsertEdge(item, edgectrl);  
                //setup path
                edgectrl.PrepareEdgePath();
            }

            if(EnableParallelEdges)
                ParallelizeEdges();
        }

        private void ParallelizeEdges()
        {
            var usedIds = EdgesList.Count > 20 ? new HashSet<int>() as ICollection<int> : new List<int>();
            //var usedIds = new List<int>();
            foreach (var item in EdgesList)
            {
                if (usedIds.Contains(item.Key.ID) || !item.Value.CanBeParallel) continue;
                var list = new List<EdgeControl>();
                list.Add(item.Value);
                foreach (var edge in EdgesList)
                {
                    if (item.Key.ID == edge.Key.ID) continue;
                    if (edge.Value.CanBeParallel && item.Key.Source.ID == edge.Key.Source.ID && item.Key.Target.ID == edge.Key.Target.ID)
                        list.Add(edge.Value);
                }

                //do stuff
                if (list.Count > 1)
                {
                    bool viceversa = false;
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].SourceOffset = (viceversa ? ParallelEdgeDistance : -ParallelEdgeDistance) * (1 + (i / 2));
                        list[i].TargetOffset = -list[i].SourceOffset;
                        viceversa = !viceversa;
                    }
                }

                list.ForEach(a => usedIds.Add((a.Edge as TEdge).ID));
                list.Clear();
            }
        }



        #endregion

        #region GenerateEdgesForVertex()
        /// <summary>
        /// Generates and displays edges for specified vertex
        /// </summary>
        /// <param name="vc">Vertex control</param>
        /// <param name="edgeType">Type of edges to display</param>
        /// <param name="defaultVisibility">Default edge visibility on layout</param>
        public override void GenerateEdgesForVertex(VertexControl vc, EdgesType edgeType, Visibility defaultVisibility = System.Windows.Visibility.Visible)
        {
            RemoveAllEdges();

            TEdge[] inlist = null;
            TEdge[] outlist = null;
            switch (edgeType)
            {
                case EdgesType.Out:
                    outlist = Graph.OutEdges(vc.Vertex as TVertex).ToArray();
                    break;
                case EdgesType.In:
                    inlist = Graph.InEdges(vc.Vertex as TVertex).ToArray();
                    break;
                default:
                    outlist = Graph.OutEdges(vc.Vertex as TVertex).ToArray();
                    inlist = Graph.InEdges(vc.Vertex as TVertex).ToArray();
                    break;
            }
            if (inlist != null)
                foreach (var item in inlist)
                {
                    var ctrl = new EdgeControl(VertexList[item.Source], vc, item) { Visibility = defaultVisibility };
                    InsertEdge(item, ctrl);
                    ctrl.PrepareEdgePath();
                }
            if (outlist != null)
                foreach (var item in outlist)
                {
                    var ctrl = new EdgeControl(vc, VertexList[item.Target], item) { Visibility = defaultVisibility };
                    InsertEdge(item, ctrl);
                    ctrl.PrepareEdgePath();
                }
        }
        #endregion

        /// <summary>
        /// Sets all edges dash style
        /// </summary>
        /// <param name="style">Selected style</param>
        public void SetEdgesDashStyle(EdgeDashStyle style)
        {
            foreach (var item in EdgesList)
                item.Value.DashStyle = style;
        }

        /// <summary>
        /// Update visual appearance for all possible visual edges
        /// </summary>
        public void UpdateAllEdges()
        {
            foreach (var ec in EdgesList.Values)
            {
                ec.PrepareEdgePath();
                ec.InvalidateVisual();
            }
            if (EnableParallelEdges)
                ParallelizeEdges();
        }

        /// <summary>
        /// Show or hide all edges arrows. Default value is True.
        /// </summary>
        /// <param name="value">Boolean value</param>
        public void ShowAllEdgesArrows(bool value = true)
        {
            foreach (var item in EdgesList.Values.ToList())
                item.ShowArrows = value;
        }

        /// <summary>
        /// Show or hide all edges labels.
        /// </summary>
        /// <param name="value">Boolean value</param>
        public void ShowAllEdgesLabels(bool value = true)
        {
            foreach (var item in EdgesList.Values.ToList())
                item.ShowLabel = value;
            InvalidateVisual();
        }

        #endregion

        #region GetRelatedControls
        /// <summary>
        /// Get controls related to specified control 
        /// </summary>
        /// <param name="vc">Original control</param>
        /// <param name="resultType">Type of resulting related controls</param>
        /// <param name="edgesType">Optional edge controls type</param>
        public override List<IGraphControl> GetRelatedControls(IGraphControl ctrl, GraphControlType resultType = GraphControlType.VertexAndEdge, EdgesType edgesType = EdgesType.Out)
        {
            if(Graph == null) 
            {
                Debug.Write("Graph property not set while using GetRelatedControls method!");
                return null;
            }

            var list = new List<IGraphControl>();
            var vc = ctrl as VertexControl;
            if (vc != null)
            {
                //if (vc.Vertex == null) return null;
                List<TEdge> edgesInList = null;
                List<TEdge> edgesOutList = null;
                if (edgesType == EdgesType.In || edgesType == EdgesType.All)
                {
                    IEnumerable<TEdge> inEdges;
                    Graph.TryGetInEdges(vc.Vertex as TVertex, out inEdges);
                    edgesInList = inEdges == null? null : inEdges.ToList();
                }

                if (edgesType == EdgesType.Out || edgesType == EdgesType.All)
                {
                    IEnumerable<TEdge> outEdges;
                    Graph.TryGetOutEdges(vc.Vertex as TVertex, out outEdges);
                    edgesOutList = outEdges == null ? null : outEdges.ToList();
                }

                if (resultType == GraphControlType.Edge || resultType == GraphControlType.VertexAndEdge)
                {
                    if (edgesInList != null)
                        foreach (var item in edgesInList)
                            if (EdgesList.ContainsKey(item)) list.Add(EdgesList[item]);
                    if (edgesOutList != null)
                        foreach (var item in edgesOutList)
                            if(EdgesList.ContainsKey(item)) list.Add(EdgesList[item]);
                }
                if (resultType == GraphControlType.Vertex || resultType == GraphControlType.VertexAndEdge)
                {
                    if (edgesInList != null)
                        foreach (var item in edgesInList)
                            if (VertexList.ContainsKey(item.Source)) list.Add(VertexList[item.Source]);
                    if (edgesOutList != null)
                        foreach (var item in edgesOutList)
                            if (VertexList.ContainsKey(item.Target)) list.Add(VertexList[item.Target]);
                }
                return list;
            }
            var ec = ctrl as EdgeControl;
            if (ctrl != null)
            {
                var edge = (TEdge)ec.Edge;
                if (resultType == GraphControlType.Edge) return list;
                else
                {
                    if (VertexList.ContainsKey(edge.Target)) list.Add(VertexList[edge.Target]);
                    if (VertexList.ContainsKey(edge.Source)) list.Add(VertexList[edge.Source]);
                }
            }
            return list;
        }
        #endregion


        #region Save

        public void SaveIntoFile(string filename, bool autoAssignMissingDataID = true)
        {
            if (autoAssignMissingDataID)
            {
                autoresolveIds();
            }

            var dlist = new List<DataSaveModel>();
            foreach (var item in VertexList) //ALWAYS serialize vertices first
            {
                dlist.Add(new DataSaveModel() { Position = item.Value.GetPosition(), Data = item.Key });
                if (item.Key.ID == -1) throw new GX_InvalidDataException("SaveIntoFile() -> All vertex datas must have positive unique ID!");
            }
            foreach (var item in EdgesList)
            {
               // item.Key.RoutingPoints = new Point[] { new Point(0, 123), new Point(12, 12), new Point(10, 234.5) };
                dlist.Add(new DataSaveModel() { Position = new Point(), Data = item.Key });
                if (item.Key.ID == -1) throw new GX_InvalidDataException("SaveIntoFile() -> All edge datas must have positive unique ID!");
            }

            var serializer = new YAXSerializer(typeof(List<DataSaveModel>));
           // string someString = serializer.Serialize(dlist);

            using (var textWriter = new StreamWriter(filename))
            {
                serializer.Serialize(dlist, textWriter);
                textWriter.Close();
            }

           // var gr = (List<DataSaveModel>)serializer.Deserialize(someString);
        }

        public void LoadFromFile(string filename)
        {
            RemoveAllEdges();
            RemoveAllVertices();

            var deserializer = new YAXSerializer(typeof(List<DataSaveModel>));
            using (var textReader = new StreamReader(filename))
            {
                var data = (List<DataSaveModel>)deserializer.Deserialize(textReader);
                if (Graph == null) Graph = Activator.CreateInstance<TGraph>();                
                else Graph.Clear();

                var vlist = data.Where(a => a.Data is TVertex);
                foreach (var item in vlist)
                {
                    var vertexdata = item.Data as TVertex;
                    var ctrl = new VertexControl(vertexdata); ctrl.SetPosition(item.Position, true);
                    AddVertex(vertexdata, ctrl);
                    Graph.AddVertex(vertexdata);
                }
                var elist = data.Where(a => a.Data is TEdge);
                foreach (var item in elist)
                {
                    var edgedata = item.Data as TEdge;
                    var sourceid = edgedata.Source.ID; var targetid = edgedata.Target.ID;
                    var datasource = VertexList.Keys.FirstOrDefault(a => a.ID == sourceid); var datatarget = VertexList.Keys.FirstOrDefault(a => a.ID == targetid);

                    edgedata.Source = datasource;
                    edgedata.Target = datatarget;

                    if (datasource == null || datatarget == null)
                        throw new GX_SerializationException("LoadFromFile() -> Serialization logic is broken! Vertex not found. All vertices must be processed before edges!");
                    var ecc = new EdgeControl() { Edge = edgedata, Source = VertexList[datasource], Target = VertexList[datatarget] };
                    InsertEdge(edgedata, ecc);
                    Graph.AddEdge(edgedata);
                    //update edge layout and shapes manually
                    //to correctly draw arrows in any case except they are manually disabled
                    UpdateLayout();
                    ecc.OnApplyTemplate();
                }
            }
        }

        #endregion

        #region Save/Load visual - OBSOLETE
        /// <summary>
        /// Save visual graph data into file. Note that the same vertex and edge data must present in the Graph on file load.
        /// </summary>
        /// <param name="filename">Filename</param>
        /*public void SaveVisual(string filename)
        {
            var input = new List<VisualSaveModel>();

            foreach (var item in VertexList)
                input.Add(new VisualSaveModel() { Type = 1, SourceID = item.Key.ID, X = GraphAreaBase.GetX(item.Value), Y = GraphAreaBase.GetY(item.Value) });
            foreach (var item in EdgesList)
                input.Add(new VisualSaveModel() { Type = 0, SourceID = item.Key.Source.ID, TargetID = item.Key.Target.ID, RoutingPoints = item.Key.RoutingPoints });

            var serializer = new XmlSerializer(typeof(List<VisualSaveModel>));
            using (var textWriter = new StreamWriter(filename))
            {
                serializer.Serialize(textWriter, input);
                textWriter.Close();
            }

        }*/

        /// <summary>
        /// Load visual graph from filename using preloaded Graph. Note that the same vertex and edge data must present in the Graph to load all data sucessfuly.
        /// </summary>
        /// <param name="filename">Filename</param>
        /*public void LoadVisual(string filename)
        {
            RemoveAllEdges();
            RemoveAllVertices();

            var deserializer = new XmlSerializer(typeof(List<VisualSaveModel>));
            using (var textReader = new StreamReader(filename))
            {
                var data = (List<VisualSaveModel>)deserializer.Deserialize(textReader);
                foreach (var item in data)
                {
                    if (item.Type == 1)
                    {
                        var vertexdata = Graph.Vertices.FirstOrDefault(a => a.ID == item.SourceID);// getVertexMethod(item.ID);
                        if (vertexdata == null)
                            Debug.WriteLine(string.Format("LoadVisual() -> Vertex data {0} not found in currently loaded Graph!", item.SourceID));
                        var ctrl = new VertexControl(vertexdata); ctrl.SetPosition(new Point(item.X, item.Y), true);
                        AddVertex(vertexdata, ctrl);
                    }
                    else if (item.Type == 0)
                    {
                        var edgedata = Graph.Edges.FirstOrDefault(a => a.Source.ID == item.SourceID && a.Target.ID == item.TargetID);// getEdgeMethod(item.ID);
                        if (edgedata == null) Debug.WriteLine(string.Format("LoadVisual() -> Edge data {0}-{1} not found in currently loaded Graph!", item.SourceID, item.TargetID));
                        if (!VertexList.ContainsKey(edgedata.Source) || !VertexList.ContainsKey(edgedata.Target))
                            Debug.WriteLine("LoadVisual() -> Vertex control not found while loading edge!");
                        InsertEdge(edgedata, new EdgeControl() { Edge = edgedata, DataContext = edgedata, Source = VertexList[edgedata.Source], Target = VertexList[edgedata.Target] });
                        
                    }
                }
                textReader.Close();
            }
        }*/

        /// <summary>
        /// Load visual graph from filename using methods for getting vertex and edge data by ID
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="getVertexMethod">Method to get vertex data</param>
        /// <param name="getEdgeMethod">Method to get edge data</param>
        /// <param name="bypassMissing">Bypass missing data</param>
        /*public void LoadVisual(string filename, Func<int, TVertex> getVertexMethod, Func<int, int, TEdge> getEdgeMethod, bool bypassMissing = true)
        {
            RemoveAllEdges();
            RemoveAllVertices();
            BidirectionalGraph<TVertex, TEdge> bdgraph;
            try
            {
                Graph = (TGraph)Activator.CreateInstance(typeof(TGraph));
                bdgraph = Graph as BidirectionalGraph<TVertex, TEdge>;
            }
            catch { throw new Exception("LoadVisual() -> Can't create graph instance. Check if graph type implements parameterless constructor or if it have BidirectionalGraph type."); }

            var deserializer = new XmlSerializer(typeof(List<VisualSaveModel>));
            using (var textReader = new StreamReader(filename))
            {
                var data = (List<VisualSaveModel>)deserializer.Deserialize(textReader);
                //vertices
                foreach (var item in data.Where(a=> a.Type == 1))
                {
                    var vertexdata = getVertexMethod(item.SourceID);
                    if (vertexdata == null)
                    {
                        if (!bypassMissing) throw new Exception("LoadVisual() -> Vertex data not received!");
                        else Debug.WriteLine("LoadVisual() -> Vertex data not received!");
                    }
                    else
                    {
                        bdgraph.AddVertex(vertexdata);
                        var ctrl = new VertexControl(vertexdata);
                        ctrl.SetPosition(new Point(item.X, item.Y), true);
                        AddVertex(vertexdata, ctrl);
                    }
                }
                //edges
                foreach (var item in data.Where(a=> a.Type == 0))
                {
                    var edgedata = getEdgeMethod(item.SourceID, item.TargetID);
                    if (edgedata == null)
                    {
                        if (!bypassMissing) throw new Exception("LoadVisual() -> Edge data not received!");
                        else Debug.WriteLine("LoadVisual() -> Edge data not received!");
                    }
                    else
                    {
                        bdgraph.AddEdge(edgedata);
                        if (VertexList.ContainsKey(edgedata.Source) && VertexList.ContainsKey(edgedata.Target))
                            InsertEdge(edgedata, new EdgeControl() { Edge = edgedata, DataContext = edgedata, Source = VertexList[edgedata.Source], Target = VertexList[edgedata.Target] });
                        else if (!bypassMissing) throw new Exception("LoadVisual() -> Vertex control not found while loading edge!");
                             else Debug.WriteLine("LoadVisual() -> Vertex control not found while loading edge!");
                    }
                }
                Graph = bdgraph as TGraph;
                textReader.Close();
            }
        }*/
        #endregion

        #region Export and printing

        /// <summary>
        /// Export current graph layout into the PNG image file. layout will be saved in full size.
        /// </summary>
        public void ExportAsPNG(bool useZoomControlSurface = false)
        {
            ExportAsImage(ImageType.PNG, useZoomControlSurface);
        }

        /// <summary>
        /// Export current graph layout into the JPEG image file. layout will be saved in full size.
        /// </summary>
        /// <param name="quality">Optional image quality parameter</param>   
        public void ExportAsJPEG(bool useZoomControlSurface = false, int quality = 100)
        {
            ExportAsImage(ImageType.JPEG, useZoomControlSurface, PrintHelper.DefaultDPI, quality);
        }

        /// <summary>
        /// Export current graph layout into the chosen image file and format. layout will be saved in full size.
        /// </summary>
        /// <param name="itype">Image format</param>
        /// <param name="dpi">Optional image DPI parameter</param>
        /// <param name="useZoomControlSurface">Use zoom control parent surface to render bitmap (only visible zoom content will be exported)</param>
        /// <param name="quality">Optional image quality parameter (for JPEG)</param>   
        public void ExportAsImage(ImageType itype, bool useZoomControlSurface = false, double dpi = PrintHelper.DefaultDPI, int quality = 100)
        {
            string fileExt;
            string fileType = itype.ToString();
            switch (itype)
            {
                case ImageType.PNG: fileExt = "*.png";
                    break;
                case ImageType.JPEG: fileExt = "*.jpg";
                    break;
                case ImageType.BMP: fileExt = "*.bmp";
                    break;
                case ImageType.GIF: fileExt = "*.gif";
                    break;
                case ImageType.TIFF: fileExt = "*.tiff";
                    break;
                default: throw new GX_InvalidDataException("ExportAsImage() -> Unknown output image format specified!");
            }

            var dlg = new SaveFileDialog() { Filter = String.Format("{0} Image File ({1})|{1}", fileType, fileExt), Title = String.Format("Exporting graph as {0} image...", fileType) };
            if (dlg.ShowDialog() == true)
            {
                PrintHelper.ExportToImage(this, new Uri(dlg.FileName), itype, useZoomControlSurface, dpi, quality);
            }
        }

        
        /// <summary>
        /// Print current visual graph layout
        /// </summary>
        /// <param name="description">Optional header description</param>
        public void PrintDialog(string description = "")
        {
            PrintHelper.ShowPrintPreview(this, description);
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            StateStorage.Dispose();
            Graph = null;
            ExternalEdgeRoutingAlgorithm = null;
            ExternalOverlapRemovalAlgorithm = null;
            MoveAnimation = null;
            DeleteAnimation = null;
            MouseOverAnimation = null;
        }

        /// <summary>
        /// Clear graph visual layout (all edges and vertices)
        /// </summary>
        public void ClearLayout()
        {
            RemoveAllEdges();
            RemoveAllVertices();
        }
        #endregion
    }
}
