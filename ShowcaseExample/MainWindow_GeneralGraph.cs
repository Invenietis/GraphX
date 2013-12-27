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
using GraphX.GraphSharp.Algorithms.OverlapRemoval;

namespace ShowcaseExample
{
    public partial class MainWindow
    {
        private void GeneralGraph_Constructor()
        {
            //gg_Area.DefaulOverlapRemovalAlgorithm
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

        void GG_RegisterCommands()
        {
            gg_but_relayout.Command = new SimpleCommand(GGRelayoutCommandCanExecute, GGRelayoutCommandExecute);
        }


        #endregion

        void gg_async_Checked(object sender, RoutedEventArgs e)
        {
            gg_Area.AsyncAlgorithmCompute = (bool)gg_async.IsChecked;
        }

        void gg_zoomctrl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
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
