﻿using GraphX.GraphSharp.Algorithms.OverlapRemoval;
using GraphX.Xceed.Wpf.Toolkit.Zoombox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

/* Some notes about the main objects and types in this example:
 * 
 * GraphAreaExample - our custom layout panel derived from the base GraphArea class with custom data types. Presented as object named: Area.
 * GraphExample - our custom data graph derived from BidirectionalGraph class using custom data types.
 * DataVertex - our custom vertex data type.
 * DataEdge - our custom edge data type.
 * Zoombox - zoom control object that handles all zooming and interaction stuff. Presented as object named: zoomctrl.
 * 
 * VertexControl - visual vertex object that is responsible for vertex drawing. Can be found in Area.VertexList collection.
 * EdgeControl - visual edge object that is responsible for edge drawing. Can be found in Area.EdgesList collection.
 */

namespace SimpleGraph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private GraphExample _dataGraph;

        public MainWindow()
        {
            InitializeComponent();

            //Customize Zoombox a bit
            //Set minimap (overview) window to be visible by default
            Zoombox.SetViewFinderVisibility(zoomctrl, System.Windows.Visibility.Visible);
            //Set Fill zooming strategy so whole graph will be always visible
            zoomctrl.FillToBounds();

            //Lets create filled data graph with edges and vertices
            _dataGraph= GraphExample_Setup();

            //Lets setup GraphArea settings
            GraphAreaExample_Setup();

            gg_but_randomgraph.Click += gg_but_randomgraph_Click;
            gg_but_relayout.Click += gg_but_relayout_Click;

            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            gg_but_randomgraph_Click(null, null);
            zoomctrl.FillToBounds(); 
        }

        void gg_but_relayout_Click(object sender, RoutedEventArgs e)
        {
            //This method initiates graph relayout process which involves consequnet call to all selected algorithms.
            //It behaves like GenerateGraph() method except that it doesn't create any visual object. Only update existing ones
            //using current Area.Graph data graph.
            Area.RelayoutGraph();
        }

        void gg_but_randomgraph_Click(object sender, RoutedEventArgs e)
        {
            //Lets generate configured graph using pre-created data graph.
            //Optionaly we set second method param to True (False by default) so this method will automatically generate edges
            //  By default edges are not generated. That allows to increase performance in cases where edges don't need to be drawn at first.
            //  You can also handle edge generation by calling manually Area.GenerateAllEdges() method.
            //Optionaly we set third param to True (True by default) so this method will automaticaly checks and assigns missing unique data ids
            //for edges and vertices in _dataGraph.
            //Note! Area.Graph property will be filled with supplied _dataGraph object.
            Area.GenerateGraph(_dataGraph, true, true);

            /* 
             * After graph generation is finished you can apply some additional settings for newly created visual vertex and edge controls
             * (VertexControl and EdgeControl classes).
             * 
             */

            //This method sets the dash style for edges. It is applied to all edges in Area.EdgesList. You can also set dash property for
            //each edge individually using EdgeControl.DashStyle property.
            //For ex.: Area.EdgesList[0].DashStyle = GraphX.EdgeDashStyle.Dash;
            Area.SetEdgesDashStyle(GraphX.EdgeDashStyle.Dash);

            //This method sets edges arrows visibility. It is also applied to all edges in Area.EdgesList. You can also set property for
            //each edge individually using property, for ex: Area.EdgesList[0].ShowArrows = true;
            Area.ShowAllEdgesArrows(true);

            //This method sets edges labels visibility. It is also applied to all edges in Area.EdgesList. You can also set property for
            //each edge individually using property, for ex: Area.EdgesList[0].ShowLabel = true;
            Area.ShowAllEdgesLabels(true);


        }

        private GraphExample GraphExample_Setup()
        {
            //Lets make new data graph instance
            var dataGraph = new GraphExample();
            //Now we need to create edges and vertices to fill data graph
            //This edges and vertices will represent graph structure and connections
            //Lets make some vertices
            for (int i = 1; i < 10; i++)
            {
                //Create new vertex with specified Text. Also we will assign custom unique ID.
                //This ID is needed for several features such as serialization and edge routing algorithms.
                //If you don't need any custom IDs and you are using automatic Area.GenerateGraph() method then you can skip ID assignment
                //because specified method automaticaly assigns missing data ids (this behavior controlled by method param).
                var dataVertex = new DataVertex("MyVertex " + i) { ID = i };
                //Add vertex to data graph
                dataGraph.AddVertex(dataVertex);
            }

            //Now lets make some edges that will connect our vertices
            //get the indexed list of graph vertices we have already added
            var vlist = dataGraph.Vertices.ToList();
            //Then create two edges optionally defining Text property to show who are connected
            var dataEdge = new DataEdge(vlist[0], vlist[1]) { Text = string.Format("{0} -> {1}", vlist[0], vlist[1]) };
            dataGraph.AddEdge(dataEdge);
                dataEdge = new DataEdge(vlist[2], vlist[3]) { Text = string.Format("{0} -> {1}", vlist[2], vlist[3]) };
            dataGraph.AddEdge(dataEdge);

            return dataGraph;
        }

        private void GraphAreaExample_Setup()
        {
            //This property sets vertex overlap removal algorithm.
            //Such algorithms help to arrange vertices in the layout so no one overlaps each other.
            Area.DefaultOverlapRemovalAlgorithm = GraphX.OverlapRemovalAlgorithmTypeEnum.FSA;
            Area.DefaultOverlapRemovalAlgorithmParams = Area.AlgorithmFactory.CreateOverlapRemovalParameters(GraphX.OverlapRemovalAlgorithmTypeEnum.FSA);
            ((OverlapRemovalParameters)Area.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            ((OverlapRemovalParameters)Area.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;

            //This property sets edge routing algorithm that is used to build route paths according to algorithm logic.
            //For ex., SimpleER algorithm will try to set edge paths around vertices so no edge will intersect any vertex.
            //Bundling algorithm will try to tie different edges that follows same direction to a single channel making complex graphs more appealing.
            Area.DefaultEdgeRoutingAlgorithm = GraphX.EdgeRoutingAlgorithmTypeEnum.SimpleER;

            //This property sets async algorithms computation so methods like: Area.RelayoutGraph() and Area.GenerateGraph()
            //will run async with the UI thread. Completion of the specified methods can be catched by corresponding events:
            //Area.RelayoutFinished and Area.GenerateGraphFinished.
            Area.AsyncAlgorithmCompute = false;
        }

        public void Dispose()
        {
            //If you plan dynamicaly create and destroy GraphArea it is wise to use Dispose() method
            //that ensures that all potential memory-holding objects will be released.
            Area.Dispose();
        }
    }
}
