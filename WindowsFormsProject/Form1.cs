using GraphX;
using GraphX.GraphSharp.Algorithms.Layout.Simple.FDP;
using GraphX.GraphSharp.Algorithms.OverlapRemoval;
using GraphX.Xceed.Wpf.Toolkit.Zoombox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace WindowsFormsProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        void Form1_Load(object sender, EventArgs e)
        {
            wpfHost.Child = GenerateWpfVisuals();
            zoomctrl.FillToBounds();
        }

        private Zoombox zoomctrl;
        private GraphAreaExample gArea;

        private UIElement GenerateWpfVisuals()
        {
            zoomctrl = new Zoombox();
            Zoombox.SetViewFinderVisibility(zoomctrl, System.Windows.Visibility.Visible);
            /* ENABLES WINFORMS HOSTING MODE --- >*/
            gArea = new GraphAreaExample() { Graph = GenerateGraph(), EnableWinFormsHostingMode = true };
            gArea.DefaultLayoutAlgorithm = GraphX.LayoutAlgorithmTypeEnum.KK;
            gArea.DefaultLayoutAlgorithmParams = gArea.AlgorithmFactory.CreateLayoutParameters(GraphX.LayoutAlgorithmTypeEnum.KK);
            ((KKLayoutParameters)gArea.DefaultLayoutAlgorithmParams).MaxIterations = 100;
            gArea.DefaultOverlapRemovalAlgorithm = GraphX.OverlapRemovalAlgorithmTypeEnum.FSA;
            gArea.DefaultOverlapRemovalAlgorithmParams = gArea.AlgorithmFactory.CreateOverlapRemovalParameters(GraphX.OverlapRemovalAlgorithmTypeEnum.FSA);
            ((OverlapRemovalParameters)gArea.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            ((OverlapRemovalParameters)gArea.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;
            gArea.DefaultEdgeRoutingAlgorithm = GraphX.EdgeRoutingAlgorithmTypeEnum.SimpleER;
            gArea.AsyncAlgorithmCompute = false;
            zoomctrl.Content = gArea;

            return zoomctrl;
        }

        private GraphExample GenerateGraph()
        {
            //FOR DETAILED EXPLANATION please see SimpleGraph example project
            var dataGraph = new GraphExample();
            for (int i = 1; i < 10; i++)
            {
                var dataVertex = new DataVertex("MyVertex " + i) { ID = i };
                dataGraph.AddVertex(dataVertex);
            }
            var vlist = dataGraph.Vertices.ToList();
            //Then create two edges optionaly defining Text property to show who are connected
            var dataEdge = new DataEdge(vlist[0], vlist[1]) { Text = string.Format("{0} -> {1}", vlist[0], vlist[1]) };
            dataGraph.AddEdge(dataEdge);
            dataEdge = new DataEdge(vlist[2], vlist[3]) { Text = string.Format("{0} -> {1}", vlist[2], vlist[3]) };
            dataGraph.AddEdge(dataEdge);
            return dataGraph;
        }

        private void but_generate_Click(object sender, EventArgs e)
        {
            gArea.GenerateGraph(true);
            gArea.SetVerticesDrag(true, true);
        }

        private void but_reload_Click(object sender, EventArgs e)
        {
            gArea.RelayoutGraph();
        }
    }
}
