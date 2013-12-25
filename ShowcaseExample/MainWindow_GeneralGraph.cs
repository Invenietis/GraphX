using GraphX;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using ShowcaseExample.Models;
using GraphX.Xceed.Wpf.Toolkit.Zoombox;
using GraphX.GraphSharp.Algorithms.Layout.Simple.Hierarchical;
using GraphX.GraphSharp.Algorithms.OverlapRemoval;
using GraphX.GraphSharp.Algorithms.Layout.Compound.FDP;

namespace ShowcaseExample
{
    public partial class MainWindow
    {
        private void GeneralGraph_Constructor()
        {
            //gg_Area.DefaulOverlapRemovalAlgorithm
            gg_layalgo.ItemsSource = Enum.GetValues(typeof(LayoutAlgorithmTypeEnum)).Cast<LayoutAlgorithmTypeEnum>();
            gg_layalgo.SelectedIndex = 0;
            gg_oralgo.ItemsSource = Enum.GetValues(typeof(OverlapRemovalAlgorithmTypeEnum)).Cast<OverlapRemovalAlgorithmTypeEnum>();
            gg_oralgo.SelectedIndex = 0;
            gg_eralgo.ItemsSource = Enum.GetValues(typeof(EdgeRoutingAlgorithmTypeEnum)).Cast<EdgeRoutingAlgorithmTypeEnum>();
            gg_eralgo.SelectedIndex = 2;
            gg_but_randomgraph.Click += gg_but_randomgraph_Click;
            gg_zoomctrl.MouseMove += gg_zoomctrl_MouseMove;
            gg_vertexCount.Text = "30";
            gg_async.Checked += gg_async_Checked;
            gg_async.Unchecked += gg_async_Checked;
            gg_Area.RelayoutFinished += gg_Area_RelayoutFinished;
            gg_Area.GenerateGraphFinished += gg_Area_GenerateGraphFinished;
            gg_Area.UseNativeObjectArrange = false;
            gg_Area.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;

            /*gg_zoomctrl.ViewStackMode = Xceed.Wpf.Toolkit.Zoombox.ZoomboxViewStackMode.Disabled;
            Zoombox.SetViewFinderVisibility(gg_zoomctrl, System.Windows.Visibility.Visible);
            gg_zoomctrl.IsAnimated = false;
            gg_Area.dirtySpeedHack = false;*///////////////////

            //gg_zoomctrl.AnimationLength = TimeSpan.FromSeconds(0);
            gg_zoomctrl.IsAnimationDisabled = false;
            gg_zoomctrl.MaxZoomDelta = 2;

           // gg_Area.DefaultLayoutAlgorithmParams = gg_Area.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.EfficientSugiyama);
           // ((EfficientSugiyamaLayoutParameters)gg_Area.DefaultLayoutAlgorithmParams).WidthPerHeight = 0;

            this.Loaded += GG_Loaded;


        }

        void GG_Loaded(object sender, RoutedEventArgs e)
        {
            GG_RegisterCommands();
        }

        #region Commands

        #region GGRelayoutCommand

        private bool GGRelayoutCommandCanExecute(object sender)
        {
            return true;// gg_Area.Graph != null && gg_Area.VertexList.Count > 0;
        }

        private void GGRelayoutCommandExecute(object sender)
        {
            if (gg_Area.AsyncAlgorithmCompute)
                gg_loader.Visibility = System.Windows.Visibility.Visible;
            gg_Area.RelayoutGraph(true);
            //renew edges if edge routing is enabled
            //if (gg_Area.ExternalEdgeRoutingAlgorithm != null || gg_Area.DefaultEdgeRoutingAlgorithm != EdgeRoutingAlgorithmTypeEnum.None)
            //    gg_Area.GenerateAllEdges();
        }
        #endregion

        #region SaveStateCommand
        public static RoutedCommand SaveStateCommand = new RoutedCommand();
        private void SaveStateCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = gg_Area.Graph != null && gg_Area.VertexList.Count > 0;
        }

        private void SaveStateCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (gg_Area.StateStorage.ContainsState("exampleState"))
                gg_Area.StateStorage.RemoveState("exampleState");
            gg_Area.StateStorage.SaveState("exampleState", "My example state");
        }
        #endregion

        #region LoadStateCommand
        public static RoutedCommand LoadStateCommand = new RoutedCommand();
        private void LoadStateCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = gg_Area.StateStorage.ContainsState("exampleState");
        }

        private void LoadStateCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (gg_Area.StateStorage.ContainsState("exampleState"))
                gg_Area.StateStorage.LoadState("exampleState");
        }
        #endregion

        #region SaveLayoutCommand
        public static RoutedCommand SaveLayoutCommand = new RoutedCommand();
        private void SaveLayoutCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = gg_Area.Graph != null && gg_Area.VertexList.Count > 0;
        }

        private void SaveLayoutCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new SaveFileDialog() { Filter = "All files|*.*", Title = "Select layout file name", FileName = "laytest.xml" };
            if (dlg.ShowDialog() == true)
            {
                //gg_Area.SaveVisual(dlg.FileName);
                gg_Area.SaveIntoFile(dlg.FileName);
            }
        }
        #endregion

        #region LoadLayoutCommand

        #region Save / load visual graph example

        /// <summary>
        /// Temporary storage for example vertex data objects used on save/load mechanics
        /// </summary>
        private Dictionary<int, DataVertex> exampleVertexStorage = new Dictionary<int, DataVertex>();
        public DataVertex gg_getVertex(int id)
        {
            var item = DataSource.FirstOrDefault(a => a.ID == id);
            if (item == null) item = new Models.DataItem() { ID = id, Text = id.ToString() };
            exampleVertexStorage.Add(id, new DataVertex() { ID = item.ID, Text = item.Text, DataImage = new BitmapImage(new Uri(@"pack://application:,,,/GraphX;component/Images/help_black.png", UriKind.Absolute)) { CacheOption = BitmapCacheOption.OnLoad } });
            return exampleVertexStorage.Last().Value;
        }

        public DataEdge gg_getEdge(int ids, int idt)
        {
            return new DataEdge(exampleVertexStorage.ContainsKey(ids) ? exampleVertexStorage[ids] : null,
                exampleVertexStorage.ContainsKey(idt) ? exampleVertexStorage[idt] : null);
        }

        #endregion

        public static RoutedCommand LoadLayoutCommand = new RoutedCommand();
        private void LoadLayoutCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;// System.IO.File.Exists("laytest.xml");
        }

        private void LoadLayoutCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new OpenFileDialog() { Filter = "All files|*.*", Title = "Select layout file", FileName = "laytest.xml" };
            if (dlg.ShowDialog() == true)
            {
                exampleVertexStorage.Clear();
                try
                {
                    //gg_Area.LoadVisual(dlg.FileName, gg_getVertex, gg_getEdge);
                    gg_Area.LoadFromFile(dlg.FileName);
                    EnableDrag();
                    //gg_Area.UpdateLayout();
                    gg_Area.UpdateAllEdges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Failed to load layout file:\n {0}", ex.ToString()));
                }
            }
        }
        #endregion

        void GG_RegisterCommands()
        {
            CommandBindings.Add(new CommandBinding(SaveStateCommand, SaveStateCommandExecute, SaveStateCommandCanExecute));
            gg_saveState.Command = SaveStateCommand;
            CommandBindings.Add(new CommandBinding(LoadStateCommand, LoadStateCommandExecute, LoadStateCommandCanExecute));
            gg_loadState.Command = LoadStateCommand;

            CommandBindings.Add(new CommandBinding(SaveLayoutCommand, SaveLayoutCommandExecute, SaveLayoutCommandCanExecute));
            gg_saveLayout.Command = SaveLayoutCommand;
            CommandBindings.Add(new CommandBinding(LoadLayoutCommand, LoadLayoutCommandExecute, LoadLayoutCommandCanExecute));
            gg_loadLayout.Command = LoadLayoutCommand;

            gg_but_relayout.Command = new SimpleCommand(GGRelayoutCommandCanExecute, GGRelayoutCommandExecute);
        }


        #endregion

        void gg_async_Checked(object sender, RoutedEventArgs e)
        {
            gg_Area.AsyncAlgorithmCompute = (bool)gg_async.IsChecked;
        }

        void gg_zoomctrl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            /*var ex = new Point(0,0);
            var fix = new Point(0, 0);
            if (gg_Area.VertexList.Count > 0)
            {
                ex = new Point(GraphAreaBase.GetX(gg_Area.VertexList.Values.ToList()[0]), GraphAreaBase.GetY(gg_Area.VertexList.Values.ToList()[0]));

                //fix = gg_Area.VertexList.Values.ToList()[0].TranslatePoint(ex, gg_zoomctrl);
                var tmp = gg_Area.PointToScreen(ex);
                //fix = this.PointFromScreen(tmp);

                fix = gg_zoomctrl.TranslatePoint(ex, gg_Area.VertexList.Values.ToList()[0]);
                
                //fix = new Point(fix.X - gg_zoomctrl.TranslateX, fix.Y - gg_zoomctrl.TranslateY);

               
            }
            var pos = Mouse.GetPosition(gg_zoomctrl);
            debug.Text = string.Format("Zoom: {0} - {1}\nFix: {2} - {3}\n{4} - {5}", pos.X, pos.Y, fix.X, fix.Y,gg_zoomctrl.TranslateX, gg_zoomctrl.TranslateY);*/



        }
        private void gg_saveAsPngImage_Click(object sender, RoutedEventArgs e)
        {
            gg_Area.ExportAsImage(ImageType.PNG);
        }

        private void gg_printlay_Click(object sender, RoutedEventArgs e)
        {
            gg_Area.PrintDialog("GraphX layout printing");
        }

        private void gg_vertexCount_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = CustomHelper.IsIntegerInput(e.Text) && Convert.ToInt32(e.Text) <= datasourceSize;
        }

        private void gg_layalgo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            gg_Area.DefaultLayoutAlgorithm = (LayoutAlgorithmTypeEnum)gg_layalgo.SelectedItem; 
            if (gg_Area.Graph == null) gg_but_randomgraph_Click(null, null);
            else gg_Area.RelayoutGraph();
            gg_Area.GenerateAllEdges();
        }

        private void gg_useExternalLayAlgo_Checked(object sender, RoutedEventArgs e)
        {
            if (gg_useExternalLayAlgo.IsChecked == true)
            {
                var graph = gg_Area.Graph == null ? GenerateDataGraph(Convert.ToInt32(gg_vertexCount.Text)) : gg_Area.Graph;
                gg_Area.Graph = graph;
                AssignExternalLayoutAlgorithm(graph);
            }
            else gg_Area.ExternalLayoutAlgorithm = null;
        }

        private void AssignExternalLayoutAlgorithm(BidirectionalGraph<DataVertex, DataEdge> graph)
        {
            gg_Area.ExternalLayoutAlgorithm = gg_Area.AlgorithmFactory.CreateLayoutAlgorithm(LayoutAlgorithmTypeEnum.ISOM, graph, null);
        }

        private void gg_oralgo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            gg_Area.DefaultOverlapRemovalAlgorithm = (OverlapRemovalAlgorithmTypeEnum)gg_oralgo.SelectedItem;
            if (gg_Area.Graph == null) gg_but_randomgraph_Click(null, null);
            else gg_Area.RelayoutGraph();
            gg_Area.GenerateAllEdges();
        }

        private void gg_useExternalORAlgo_Checked(object sender, RoutedEventArgs e)
        {
            if (gg_useExternalORAlgo.IsChecked == true)
            {
                gg_Area.ExternalOverlapRemovalAlgorithm = gg_Area.AlgorithmFactory.CreateOverlapRemovalAlgorithm(OverlapRemovalAlgorithmTypeEnum.FSA, new Dictionary<DataVertex, Rect>(), null);
            }
            else gg_Area.ExternalOverlapRemovalAlgorithm = null;
        }

        private void gg_eralgo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            gg_Area.DefaultEdgeRoutingAlgorithm = (EdgeRoutingAlgorithmTypeEnum)gg_eralgo.SelectedItem;
            if (gg_Area.Graph == null) gg_but_randomgraph_Click(null, null);
            else gg_Area.RelayoutGraph();
            gg_Area.GenerateAllEdges();
        }

        private void gg_useExternalERAlgo_Checked(object sender, RoutedEventArgs e)
        {
            if (gg_useExternalERAlgo.IsChecked == true)
            {
                var graph = gg_Area.Graph == null ? GenerateDataGraph(Convert.ToInt32(gg_vertexCount.Text)) : gg_Area.Graph;
                gg_Area.Graph = graph;
                gg_Area.ExternalEdgeRoutingAlgorithm = gg_Area.AlgorithmFactory.CreateEdgeRoutingAlgorithm(EdgeRoutingAlgorithmTypeEnum.SimpleER, new Rect(gg_Area.DesiredSize), graph, null, null,  null);
            }
            else gg_Area.ExternalEdgeRoutingAlgorithm = null;
        }

        void gg_Area_RelayoutFinished(object sender, EventArgs e)
        {
            if (gg_Area.AsyncAlgorithmCompute)
                gg_loader.Visibility = System.Windows.Visibility.Collapsed;
            gg_zoomctrl.ZoomToFill();
            gg_zoomctrl.Mode = GraphX.Controls.ZoomControlModes.Custom;
        }

        /// <summary>
        /// Use this event in case we have chosen async computation
        /// </summary>
        void gg_Area_GenerateGraphFinished(object sender, EventArgs e)
        {

            gg_Area.GenerateAllEdges();
            if (gg_Area.AsyncAlgorithmCompute)
                gg_loader.Visibility = System.Windows.Visibility.Collapsed;

           /* gg_zoomctrl.FitToBounds();
            gg_zoomctrl.ZoomOn = ZoomboxZoomOn.Content;
            gg_zoomctrl.KeepContentInBounds = false;

            double scale = gg_zoomctrl.Scale; 
            gg_Area.UpdateLayout();

            gg_zoomctrl.FillToBounds();
            gg_zoomctrl.Scale = scale;*////////////////////////
            gg_zoomctrl.ZoomToFill();
            gg_zoomctrl.Mode = GraphX.Controls.ZoomControlModes.Custom;
        }

        private void gg_but_randomgraph_Click(object sender, RoutedEventArgs e)
        {
            gg_Area.Children.Clear();
            var graph = GenerateDataGraph(Convert.ToInt32(gg_vertexCount.Text));
            //assign graph again as we need to update Graph param inside and i have no independent examples
            if(gg_Area.ExternalLayoutAlgorithm != null)
                AssignExternalLayoutAlgorithm(graph);

            /////////////////
            /*graph = new GraphExample();
            var v = new DataVertex("1");
            graph.AddVertex(v);
            for (int i = 0; i < 5; i++)
            {
                var vv = new DataVertex((10 + i).ToString());
                graph.AddVertex(vv);
                graph.AddEdge(new DataEdge(v, vv));
            }
            var v2 = new DataVertex("2");
            graph.AddVertex(v2);
            for (int i = 0; i < 5; i++)
            {
                var vv = new DataVertex((20 + i).ToString());
                graph.AddVertex(vv);
                graph.AddEdge(new DataEdge(v2, vv));
            }
            //graph.AddEdge(new DataEdge(v, v2));
            gg_Area.DefaultOverlapRemovalAlgorithmParams = gg_Area.AlgorithmFactory.CreateOverlapRemovalParameters(OverlapRemovalAlgorithmTypeEnum.FSA);
            gg_Area.DefaultLayoutAlgorithmParams = gg_Area.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.CompoundFDP);

            //((OverlapRemovalParameters)gg_Area.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 200;
            //((OverlapRemovalParameters)gg_Area.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 200;
            */
            /////////////////

            gg_Area.GenerateGraph(graph);
            EnableDrag();

            if (gg_Area.AsyncAlgorithmCompute)
                gg_loader.Visibility = System.Windows.Visibility.Visible;
        }

        private void EnableDrag()
        {
            foreach (var item in gg_Area.VertexList)
            {
                DragBehaviour.SetIsDragEnabled(item.Value, true);
                item.Value.EventOptions.PositionChangeNotification = true;
                item.Value.PositionChanged += Value_PositionChanged;
            }
        }

        void Value_PositionChanged(object sender, GraphX.Models.VertexPositionEventArgs args)
        {
            if (gg_arearesize.IsChecked == true)
            {
                var zoomtop = gg_zoomctrl.TranslatePoint(new Point(0, 0), gg_Area);
                //dg_Area.UpdateLayout();
                var zoombottom = new Point(gg_Area.ActualWidth, gg_Area.ActualHeight);
                var pos = args.Position;

                if (pos.X < zoomtop.X) { GraphAreaBase.SetX(args.VertexControl, zoomtop.X, true); }
                if (pos.Y < zoomtop.Y) { GraphAreaBase.SetY(args.VertexControl, zoomtop.Y, true); }

                if (pos.X > zoombottom.X) { GraphAreaBase.SetX(args.VertexControl, zoombottom.X, true); }
                if (pos.Y > zoombottom.Y) { GraphAreaBase.SetY(args.VertexControl, zoombottom.Y, true); }
            }
        }

        private void gg_but_randomzoom_Click(object sender, RoutedEventArgs e)
        {
           /* var item = gg_Area.VertexList.Values.FirstOrDefault(a => (a.Vertex as DataVertex).Text == gg_txt_number.Text);
                if (item == null) return;
            var x = GraphAreaBase.GetX(item);
            var y = GraphAreaBase.GetY(item);*/
        }


    }
}
