using GraphX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GraphX.GraphSharp.Algorithms.EdgeRouting;

namespace ShowcaseExample
{
    public partial class MainWindow
    {
        private void TestGround_Constructor()
        {
            tst_but_gen.Click += tst_but_gen_Click;
            tst_but_fakeLayout.Click += tst_but_fakeLayout_Click;
            tst_Area.UseNativeObjectArrange = false;
            tst_Area.EnableParallelEdges = true;
            tst_Area.ParallelEdgeDistance = 5;
        }

        void tst_but_fakeLayout_Click( object sender, RoutedEventArgs e )
        {
            var vControls = tst_Area.GetAllVertexControls();
            foreach( var c in vControls ) Console.WriteLine( c.GetPosition() );
            tst_Area.RelayoutGraph();
        }

        void tst_but_gen_Click(object sender, RoutedEventArgs e)
        {
            var _graph = new GraphExample();

            /*var v1 = new DataVertex("Start");
            _graph.AddVertex(v1);
            var v2 = new DataVertex("End");
            _graph.AddVertex(v2);
            var e1 = new DataEdge(v1, v2);
            _graph.AddEdge(e1);

            _graph.AddVertex(new DataVertex("Block 1"));
            _graph.AddVertex(new DataVertex("Block 2"));
            */
            for (int i = 0; i < 10; i++)
            {
                _graph.AddVertex(new DataVertex(i.ToString() + "bldsddd\ndssdsds"));
            }

            _graph.AddEdge(new DataEdge(_graph.Vertices.First(), _graph.Vertices.Last()));
            _graph.AddEdge(new DataEdge(_graph.Vertices.Last(), _graph.Vertices.First()));
            _graph.AddEdge(new DataEdge(_graph.Vertices.First(), _graph.Vertices.Last()));

            // Resets external algorithms to null to start new fresh graph given that starting algo is a default algo
            tst_Area.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.SimpleRandom;
            tst_Area.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            //tst_Area.DefaultEdgeRoutingAlgorithmParams = tst_Area.AlgorithmFactory.CreateEdgeRoutingParameters(EdgeRoutingAlgorithmTypeEnum.SimpleER);
            //((SimpleERParameters)tst_Area.DefaultEdgeRoutingAlgorithmParams).BackStep = 10;

            tst_Area.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;

            tst_Area.GenerateGraph(_graph, true);

            /*var vlist = tst_Area.VertexList.Values.ToList();
            vlist[0].SetPosition(new Point(100, 100)); vlist[0].MathShape = VertexShape.Circle;
            vlist[1].SetPosition(new Point(600, 100));

            vlist[2].SetPosition(new Point(400, 100));
            vlist[3].SetPosition(new Point(440, 150)); 
            */
            foreach (var item in tst_Area.VertexList)
            {
                item.Value.MathShape = VertexShape.Rectangle;
                DragBehaviour.SetIsDragEnabled(item.Value, true);
                DragBehaviour.SetUpdateEdgesOnMove(item.Value, true);
                HighlightBehaviour.SetIsHighlightEnabled(item.Value, true);
                HighlightBehaviour.SetHighlightControl(item.Value, GraphControlType.VertexAndEdge);
                HighlightBehaviour.SetHighlightEdges(item.Value, EdgesType.All);
            }

            foreach (var item in tst_Area.EdgesList.Values)
            {
                HighlightBehaviour.SetHighlightControl(item, GraphControlType.VertexAndEdge);
                HighlightBehaviour.SetHighlightEdges(item, EdgesType.All);
                HighlightBehaviour.SetHighlightControl(item, GraphControlType.VertexAndEdge);
            }
        }
      

    }
}
