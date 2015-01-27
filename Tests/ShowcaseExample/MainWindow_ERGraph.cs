using GraphX.GraphSharp.Algorithms.EdgeRouting;
using GraphX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GraphX.Xceed.Wpf.Toolkit.Zoombox;
using GraphX.GraphSharpComponents.EdgeRouting;
using GraphX.GraphSharp.Algorithms.OverlapRemoval;

namespace ShowcaseExample
{
    public partial class MainWindow
    {
        private PathFinderEdgeRoutingParameters ergpf;
        public PathFinderEdgeRoutingParameters erg_PFERParameters { get { return ergpf; } set { ergpf = value; OnPropertyChanged("erg_PFERParameters"); } }
        private SimpleERParameters ergsimple;
        public SimpleERParameters erg_SimpleERParameters { get { return ergsimple; } set { ergsimple = value; OnPropertyChanged("erg_SimpleERParameters"); } }
        private BundleEdgeRoutingParameters ergbundle;
        public BundleEdgeRoutingParameters erg_BundleEdgeRoutingParameters { get { return ergbundle; } set { ergbundle = value; OnPropertyChanged("erg_BundleEdgeRoutingParameters"); } }

        private void ERGraph_Constructor()
        {
            erg_showEdgeArrows.IsChecked = true;
            erg_BundleEdgeRoutingParameters = (BundleEdgeRoutingParameters)erg_Area.AlgorithmFactory.CreateEdgeRoutingParameters(EdgeRoutingAlgorithmTypeEnum.Bundling);
            erg_SimpleERParameters = (SimpleERParameters)erg_Area.AlgorithmFactory.CreateEdgeRoutingParameters(EdgeRoutingAlgorithmTypeEnum.SimpleER);
            erg_PFERParameters = (PathFinderEdgeRoutingParameters)erg_Area.AlgorithmFactory.CreateEdgeRoutingParameters(EdgeRoutingAlgorithmTypeEnum.PathFinder);

            erg_pfprm_formula.ItemsSource = Enum.GetValues(typeof(PathFindAlgorithm)).Cast<PathFindAlgorithm>();
            erg_pfprm_formula.SelectedIndex = 0;

            erg_but_randomgraph.Click += erg_but_randomgraph_Click;
            erg_but_relayout.Click += erg_but_relayout_Click;
            erg_useExternalERAlgo.Checked += erg_useExternalERAlgo_Checked;
            erg_useExternalERAlgo.Unchecked += erg_useExternalERAlgo_Checked;
            erg_dashstyle.ItemsSource = Enum.GetValues(typeof(EdgeDashStyle)).Cast<EdgeDashStyle>();
            erg_dashstyle.SelectedIndex = 0;
            erg_dashstyle.SelectionChanged += erg_dashstyle_SelectionChanged;
            erg_eralgo.ItemsSource = Enum.GetValues(typeof(EdgeRoutingAlgorithmTypeEnum)).Cast<EdgeRoutingAlgorithmTypeEnum>();
            erg_eralgo.SelectedIndex = 0;
            erg_eralgo.SelectionChanged += erg_eralgo_SelectionChanged;
            erg_prmsbox.Visibility = System.Windows.Visibility.Collapsed;
            erg_recalculate.Checked += erg_recalculate_Checked;
            erg_recalculate.Unchecked += erg_recalculate_Checked;
            erg_randomizeAll.Click += erg_randomizeAll_Click;
            erg_showEdgeArrows.Checked += erg_showEdgeArrows_Checked;
            erg_showEdgeArrows.Unchecked += erg_showEdgeArrows_Checked;
            erg_showEdgeLabels.Checked += erg_showEdgeLabels_Checked;
            erg_showEdgeLabels.Unchecked += erg_showEdgeLabels_Checked;
            erg_randomizeArrows.Click += erg_randomizeArrows_Click;
            erg_useCurves.Checked += erg_useCurves_Checked;
            erg_useCurves.Unchecked += erg_useCurves_Checked;
            Zoombox.SetViewFinderVisibility(erg_zoomctrl, System.Windows.Visibility.Visible);

            erg_Area.UseNativeObjectArrange = true;
        }




        void erg_useCurves_Checked(object sender, RoutedEventArgs e)
        {
            //update edge curving property
            erg_Area.EdgeCurvingEnabled = (bool)erg_useCurves.IsChecked;
            erg_Area.UpdateAllEdges();
        }

        void erg_randomizeArrows_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in erg_Area.EdgesList.Values.ToList())
                item.ShowArrows = Convert.ToBoolean(Rand.Next(0, 2));
        }

        void erg_showEdgeLabels_Checked(object sender, RoutedEventArgs e)
        {
            erg_Area.ShowAllEdgesLabels((bool)erg_showEdgeLabels.IsChecked);
            erg_Area.InvalidateVisual();
        }

        void erg_showEdgeArrows_Checked(object sender, RoutedEventArgs e)
        {
            erg_Area.ShowAllEdgesArrows((bool)erg_showEdgeArrows.IsChecked);
        }


        void erg_randomizeAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in erg_Area.EdgesList.Values.ToList())
            {
                var sarr = Enum.GetValues(typeof(EdgeDashStyle));
                item.DashStyle = (EdgeDashStyle)sarr.GetValue(Rand.Next(0, sarr.Length - 1));
            }
        }

        void erg_recalculate_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in erg_Area.GetAllVertexControls())
                DragBehaviour.SetUpdateEdgesOnMove(item, (bool)erg_recalculate.IsChecked);

        }

        void erg_dashstyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            erg_Area.SetEdgesDashStyle((EdgeDashStyle)erg_dashstyle.SelectedItem);
        }

        private void erg_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = CustomHelper.IsIntegerInput(e.Text);
        }

        private void erg_to1_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = CustomHelper.IsDoubleInput(e.Text);
            if (e.Handled) return;
            double res = 0.0;
            if(double.TryParse((sender as TextBox).Text, out res))
                if (res < 0.0 || res > 1.0) e.Handled = false;
        }

        private void erg_tominus1_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = CustomHelper.IsDoubleInput(e.Text);
            if (e.Handled) return;
            double res = 0.0;
            if (double.TryParse((sender as TextBox).Text, out res))
                if (res < -1.0 || res >0.0) e.Handled = false;
        }


        void erg_eralgo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            erg_recalculate.IsEnabled = true;
            if ((EdgeRoutingAlgorithmTypeEnum)erg_eralgo.SelectedItem == EdgeRoutingAlgorithmTypeEnum.None)
                erg_prmsbox.Visibility = System.Windows.Visibility.Collapsed;
            else
            {
                //clean prms
                erg_prmsbox.Visibility = System.Windows.Visibility.Visible;
                if ((EdgeRoutingAlgorithmTypeEnum)erg_eralgo.SelectedItem == EdgeRoutingAlgorithmTypeEnum.SimpleER)
                {
                    simpleer_prms_dp.Visibility = System.Windows.Visibility.Visible;
                    bundleer_prms_dp.Visibility = System.Windows.Visibility.Collapsed;
                    pfer_prms_dp.Visibility = System.Windows.Visibility.Collapsed;
                }
                if ((EdgeRoutingAlgorithmTypeEnum)erg_eralgo.SelectedItem == EdgeRoutingAlgorithmTypeEnum.PathFinder)
                {
                    simpleer_prms_dp.Visibility = System.Windows.Visibility.Collapsed;
                    bundleer_prms_dp.Visibility = System.Windows.Visibility.Collapsed;
                    pfer_prms_dp.Visibility = System.Windows.Visibility.Visible;
                }
                if ((EdgeRoutingAlgorithmTypeEnum)erg_eralgo.SelectedItem == EdgeRoutingAlgorithmTypeEnum.Bundling)
                {
                    simpleer_prms_dp.Visibility = System.Windows.Visibility.Collapsed;
                    bundleer_prms_dp.Visibility = System.Windows.Visibility.Visible;
                    pfer_prms_dp.Visibility = System.Windows.Visibility.Collapsed;
                    //bundling doesn't support single edge routing
                    erg_recalculate.IsChecked = false;
                    erg_recalculate.IsEnabled = false;
                }
            }
        }

        void erg_useExternalERAlgo_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (erg_useExternalERAlgo.IsChecked == true)
            {
                if (erg_Area.Graph == null) erg_but_randomgraph_Click(null, null);
                gg_Area.ExternalEdgeRoutingAlgorithm = gg_Area.AlgorithmFactory.CreateEdgeRoutingAlgorithm(EdgeRoutingAlgorithmTypeEnum.SimpleER, new Rect(erg_Area.DesiredSize), erg_Area.Graph, null, null, null);
            }
            else gg_Area.ExternalEdgeRoutingAlgorithm = null;
        }

        void erg_but_relayout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((EdgeRoutingAlgorithmTypeEnum)erg_eralgo.SelectedItem == EdgeRoutingAlgorithmTypeEnum.SimpleER)
                erg_Area.GenerateGraph(erg_Area.Graph);
            else if ((EdgeRoutingAlgorithmTypeEnum)erg_eralgo.SelectedItem == EdgeRoutingAlgorithmTypeEnum.PathFinder)
                erg_Area.RelayoutGraph();
            else if ((EdgeRoutingAlgorithmTypeEnum)erg_eralgo.SelectedItem == EdgeRoutingAlgorithmTypeEnum.Bundling)
                CalcBundling();
            erg_Area.GenerateAllEdges(Visibility.Visible);
            erg_recalculate_Checked(null, null);
            erg_showEdgeArrows_Checked(null, null);
            erg_showEdgeLabels_Checked(null, null);
        }

        private Dictionary<DataVertex, Point> GenerateRandomVertices(GraphExample graph, int index, int count, int minX, int maxX, int minY, int maxY)
        {
            var list = graph.Vertices.ToList();
            var VertexPositions = new Dictionary<DataVertex, Point>();
            for (int i = index; i < index + count; i++)
            {
                var vertex = list[i];
                var vc = new VertexControl(vertex);
                erg_Area.AddVertex(vertex, vc);
                VertexPositions[vertex] = new Point(Rand.Next(minX, maxX), Rand.Next(minY, maxY));
                vc.SetPosition(VertexPositions[vertex], true);
            }
            return VertexPositions;
        }

        void erg_but_randomgraph_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            if ((EdgeRoutingAlgorithmTypeEnum)erg_eralgo.SelectedItem != EdgeRoutingAlgorithmTypeEnum.Bundling)
            {
                var gr = GenerateDataGraph(30);
                erg_Area.DefaultEdgeRoutingAlgorithm = (EdgeRoutingAlgorithmTypeEnum)erg_eralgo.SelectedItem;
                switch ((EdgeRoutingAlgorithmTypeEnum)erg_eralgo.SelectedItem)
                {
                    case EdgeRoutingAlgorithmTypeEnum.SimpleER:
                        erg_Area.DefaultEdgeRoutingAlgorithmParams = erg_SimpleERParameters;
                        break;
                    case EdgeRoutingAlgorithmTypeEnum.PathFinder:
                        erg_Area.DefaultEdgeRoutingAlgorithmParams = erg_PFERParameters;
                       // erg_Area.SideExpansionSize = new Size(erg_PFERParameters.SideGridOffset, erg_PFERParameters.SideGridOffset);
                        break;
                }
                
                erg_Area.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
                erg_Area.DefaultOverlapRemovalAlgorithmParams = erg_Area.AlgorithmFactory.CreateOverlapRemovalParameters(OverlapRemovalAlgorithmTypeEnum.FSA);
                var oprm = erg_Area.DefaultOverlapRemovalAlgorithmParams as OverlapRemovalParameters;
                //oprm.HorizontalGap = 100;
                //oprm.VerticalGap = 100;
                //erg_Area.ExternalEdgeRoutingAlgorithm = new SimpleEdgeRouting<DataVertex, DataEdge,  GraphExample>(gr as GraphExample, erg_Area.GetVertexPositions(), erg_Area.GetVertexSizeRectangles());

                erg_Area.GenerateGraph(gr);
                erg_zoomctrl.FitToBounds();//.ZoomToFill();
                erg_Area.GenerateAllEdges();
                foreach (var item in erg_Area.GetAllVertexControls())
                    DragBehaviour.SetIsDragEnabled(item, true);
                erg_recalculate_Checked(null, null);
                erg_showEdgeArrows_Checked(null, null);
                erg_showEdgeLabels_Checked(null, null);

                return;
            }

            erg_Area.RemoveAllEdges();
            erg_Area.RemoveAllVertices();
            //generate graph
            var graph = new GraphExample();
            foreach (var item in DataSource.Take(120))
                graph.AddVertex(new DataVertex(item.Text) { ID = item.ID });

            var vlist = graph.Vertices.ToList();
            foreach (var item in vlist)
            {
                if (Rand.Next(0, 50) > 25) continue;
                var vertex2 = vlist[Rand.Next(0, graph.VertexCount - 1)];
                graph.AddEdge(new DataEdge(item, vertex2) { ToolTipText = string.Format("{0} -> {1}", item, vertex2) });
            } 
           /* graph.AddEdge(new DataEdge(vlist[0], vlist[3], 1));
            graph.AddEdge(new DataEdge(vlist[1], vlist[4], 1));
            graph.AddEdge(new DataEdge(vlist[2], vlist[5], 1));

            graph.AddEdge(new DataEdge(vlist[0], vlist[6], 1));
            graph.AddEdge(new DataEdge(vlist[1], vlist[7], 1));
            graph.AddEdge(new DataEdge(vlist[2], vlist[8], 1));
            //generate vertex positions
            var VertexPositions = new Dictionary<DataVertex, Point>();

            VertexPositions.Add(vlist[6], new Point(5000, 1000));
            VertexPositions.Add(vlist[7], new Point(5000, 1100));
            VertexPositions.Add(vlist[8], new Point(5000, 1300));

            VertexPositions.Add(vlist[0], new Point(100, 100));
            VertexPositions.Add(vlist[1], new Point(300, 100));
            VertexPositions.Add(vlist[2], new Point(200, 300));

            VertexPositions.Add(vlist[3], new Point(10000, 1000));
            VertexPositions.Add(vlist[4], new Point(10300, 1000));
            VertexPositions.Add(vlist[5], new Point(10200, 1300));*/
            //generate vertices

            var VertexPositions = new Dictionary<DataVertex, Point>();
            foreach (var item in GenerateRandomVertices(graph, 0, 40, 0, 2000, 0, 2000))
                VertexPositions.Add(item.Key, item.Value);
            foreach (var item in GenerateRandomVertices(graph, 40, 40, 5000, 7000, 3000, 4000))
                VertexPositions.Add(item.Key, item.Value);
            foreach (var item in GenerateRandomVertices(graph, 80, 40, 500, 2500, 6000, 9000))
                VertexPositions.Add(item.Key, item.Value);
            erg_Area.Graph = graph;
            UpdateLayout();


            CalcBundling();



            //generate edges
            erg_Area.GenerateAllEdges(Visibility.Visible);
            foreach (var item in erg_Area.GetAllVertexControls())
                DragBehaviour.SetIsDragEnabled(item, true);
            erg_recalculate_Checked(null, null);
            erg_showEdgeArrows_Checked(null, null);
            erg_showEdgeLabels_Checked(null, null);
            erg_zoomctrl.FitToBounds();
        }

        private void CalcBundling()
        {
            //perform edge routing
            IExternalEdgeRouting<DataVertex, DataEdge> era = null;
            IEdgeRoutingParameters prms = null;
            switch ((EdgeRoutingAlgorithmTypeEnum)erg_eralgo.SelectedItem)
            {
                case EdgeRoutingAlgorithmTypeEnum.Bundling:
                    prms = erg_BundleEdgeRoutingParameters;
                    era = new BundleEdgeRouting<DataVertex, DataEdge, GraphExample>(new Rect(erg_Area.DesiredSize), erg_Area.Graph as GraphExample, erg_Area.GetVertexPositions(), erg_Area.GetVertexSizeRectangles(), prms);
                    break;
                /*case EdgeRoutingAlgorithmTypeEnum.SimpleER:
                    prms = erg_SimpleERParameters;
                    era = new SimpleEdgeRouting<DataVertex, DataEdge, GraphExample>(graph, VertexPositions, erg_Area.GetVertexSizeRectangles(VertexPositions), prms);
                    break;*/
                case EdgeRoutingAlgorithmTypeEnum.None:
                    break;

            }
            if (era != null)
            {
                era.Compute();
                foreach (var item in erg_Area.Graph.Edges)
                {
                    if (era.EdgeRoutes.ContainsKey(item))
                        item.RoutingPoints = era.EdgeRoutes[item];
                }
            }
        }
    }
}
