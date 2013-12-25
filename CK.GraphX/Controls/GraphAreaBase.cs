using GraphX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GraphX.Animations;
using System.Windows.Input;
using GraphX.Models.Interfaces;

namespace GraphX
{
    public abstract class GraphAreaBase: Panel
    {
        #region Attached Dependency Property registrations
        public static readonly DependencyProperty XProperty =
            DependencyProperty.RegisterAttached("X", typeof(double), typeof(GraphAreaBase),
                                                 new FrameworkPropertyMetadata(double.NaN,
                                                                                FrameworkPropertyMetadataOptions.AffectsMeasure |
                                                                                FrameworkPropertyMetadataOptions.AffectsArrange |
                                                                                FrameworkPropertyMetadataOptions.AffectsRender |
                                                                                FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                                                                                FrameworkPropertyMetadataOptions.AffectsParentArrange |
                                                                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty FinalXProperty =
            DependencyProperty.RegisterAttached("FinalX", typeof(double), typeof(GraphAreaBase),
                                                 new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty FinalYProperty =
            DependencyProperty.RegisterAttached("FinalY", typeof(double), typeof(GraphAreaBase),
                                                 new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public static readonly DependencyProperty YProperty =
            DependencyProperty.RegisterAttached("Y", typeof(double), typeof(GraphAreaBase),
                                                 new FrameworkPropertyMetadata(double.NaN,
                                                                                FrameworkPropertyMetadataOptions.AffectsMeasure |
                                                                                FrameworkPropertyMetadataOptions.AffectsArrange |
                                                                                FrameworkPropertyMetadataOptions.AffectsRender |
                                                                                FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                                                                                FrameworkPropertyMetadataOptions.AffectsParentArrange |
                                                                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                                                                                ));



        
        #endregion

        #region Child EVENTS

        /// <summary>
        /// Fires when edge is selected
        /// </summary>
        public virtual event EdgeSelectedEventHandler EdgeSelected;

        internal virtual void OnEdgeSelected(EdgeControl ec)
        {
            if (EdgeSelected != null)
                EdgeSelected(this, new EdgeSelectedEventArgs(ec));
        }

        /// <summary>
        /// Fires when vertex is double clicked
        /// </summary>
        public virtual event VertexSelectedEventHandler VertexDoubleClick;

        internal virtual void OnVertexDoubleClick(VertexControl vc)
        {
            if (VertexDoubleClick != null)
                VertexDoubleClick(this, new VertexSelectedEventArgs(vc, null));
        }

        /// <summary>
        /// Fires when vertex is selected
        /// </summary>
        public virtual event VertexSelectedEventHandler VertexSelected;

        internal virtual void OnVertexSelected(VertexControl vc, MouseButtonEventArgs e)
        {
            if (VertexSelected != null)
                VertexSelected(this, new VertexSelectedEventArgs(vc, e));
        }
        /// <summary>
        /// Fires when mouse is over the vertex control
        /// </summary>
        public virtual event VertexSelectedEventHandler VertexMouseEnter;

        internal virtual void OnVertexMouseEnter(VertexControl vc)
        {
            if (VertexMouseEnter != null)
                VertexMouseEnter(this, new VertexSelectedEventArgs(vc, null));
            if (MouseOverAnimation != null)
                MouseOverAnimation.AnimateVertexForward(vc);
        }

        /// <summary>
        /// Fires when mouse is moved over the vertex control
        /// </summary>
        public virtual event VertexMovedEventHandler VertexMouseMove;

        internal virtual void OnVertexMouseMove(VertexControl vc)
        {
            if (VertexMouseMove != null)
                VertexMouseMove(this, new VertexMovedEventArgs(vc, new Point()));
        }

        /// <summary>
        /// Fires when mouse leaves vertex control
        /// </summary>
        public virtual event VertexSelectedEventHandler VertexMouseLeave;

        internal virtual void OnVertexMouseLeave(VertexControl vc)
        {
            if (VertexMouseLeave != null)
                VertexMouseLeave(this, new VertexSelectedEventArgs(vc, null));
            if (MouseOverAnimation != null)
                MouseOverAnimation.AnimateVertexBackward(vc);
        }

        /// <summary>
        /// Fires when layout algorithm calculation is finished
        /// </summary>
        public virtual event EventHandler LayoutCalculationFinished;

        protected virtual void OnLayoutCalculationFinished()
        {
            if (LayoutCalculationFinished != null)
                LayoutCalculationFinished(this, null);
        }

        /// <summary>
        /// Fires when overlap removal algorithm calculation is finished
        /// </summary>
        public virtual event EventHandler OverlapRemovalCalculationFinished;

        protected virtual void OnOverlapRemovalCalculationFinished()
        {
            if (OverlapRemovalCalculationFinished != null)
                OverlapRemovalCalculationFinished(this, null);
        }

        /// <summary>
        /// Fires when edge routing algorithm calculation is finished
        /// </summary>
        public virtual event EventHandler EdgeRoutingCalculationFinished;

        protected virtual void OnEdgeRoutingCalculationFinished()
        {
            if (EdgeRoutingCalculationFinished != null)
                EdgeRoutingCalculationFinished(this, null);
        }

        /// <summary>
        /// Fires when relayout operation is finished
        /// </summary>
        public virtual event EventHandler RelayoutFinished;

        protected virtual void OnRelayoutFinished()
        {
            if (RelayoutFinished != null)
                RelayoutFinished(this, null);
        }

        /// <summary>
        /// Fires when move animation for all objects is finished
        /// </summary>
       /* public virtual event EventHandler MoveAnimationFinished;

        protected virtual void OnMoveAnimationFinished()
        {
            if (MoveAnimationFinished != null)
                MoveAnimationFinished(this, null);
        }*/

        /// <summary>
        /// Fires when graph generation operation is finished
        /// </summary>
        public virtual event EventHandler GenerateGraphFinished;

        protected virtual void OnGenerateGraphFinished()
        {
            if (GenerateGraphFinished != null)
                GenerateGraphFinished(this, null);
        }

        public virtual event EdgeSelectedEventHandler EdgeDoubleClick;
        internal void OnEdgeDoubleClick(EdgeControl edgeControl)
        {
            if (EdgeDoubleClick != null)
                EdgeDoubleClick(this, new EdgeSelectedEventArgs(edgeControl));
        }

        public virtual event EdgeSelectedEventHandler EdgeMouseMove;
        internal void OnEdgeMouseMove(EdgeControl edgeControl)
        {
            if (EdgeMouseMove != null)
                EdgeMouseMove(this, new EdgeSelectedEventArgs(edgeControl));
        }

        public virtual event EdgeSelectedEventHandler EdgeMouseEnter;
        internal void OnEdgeMouseEnter(EdgeControl edgeControl)
        {
            if (EdgeMouseEnter != null)
                EdgeMouseEnter(this, new EdgeSelectedEventArgs(edgeControl));
            if (MouseOverAnimation != null)
                MouseOverAnimation.AnimateEdgeForward(edgeControl);
        }

        public virtual event EdgeSelectedEventHandler EdgeMouseLeave;
        internal void OnEdgeMouseLeave(EdgeControl edgeControl)
        {
            if (EdgeMouseLeave != null)
                EdgeMouseLeave(this, new EdgeSelectedEventArgs(edgeControl));
            if (MouseOverAnimation != null)
                MouseOverAnimation.AnimateEdgeBackward(edgeControl);
        }

        #endregion

        #region ComputeEdgeRoutesByVertex()
        /// <summary>
        /// Get edge route points for single edge only
        /// </summary>
        /// <param name="edge">Data edge</param>
        //public virtual Point[] GetSingleEdgeRouting(object edge) { return null; }

        /// <summary>
        /// Compute new edge routes for all edges of the vertex
        /// </summary>
        /// <param name="vc">Vertex visual control</param>
        /// <param name="VertexDataNeedUpdate">If vertex data inside edge routing algorthm needs to be updated</param>
        public virtual void ComputeEdgeRoutesByVertex(VertexControl vc, bool VertexDataNeedUpdate = true) { }
        #endregion


        public GraphAreaBase()
        {
            UseNativeObjectArrange = true;
            ParallelEdgeDistance = 5;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
        }

        #region Virtual members
        /// <summary>
        /// Returns all existing VertexControls added into the layout
        /// </summary>
        /// <returns></returns>
        public virtual ICollection<VertexControl> GetAllVertexControls() { return new VertexControl[0]; }

        /// <summary>
        /// Get controls related to specified control 
        /// </summary>
        /// <param name="vc">Original control</param>
        /// <param name="resultType">Type of resulting related controls</param>
        /// <param name="edgesType">Optional edge controls type</param>
        public virtual List<IGraphControl> GetRelatedControls(IGraphControl ctrl, GraphControlType resultType = GraphControlType.VertexAndEdge, EdgesType edgesType = EdgesType.Out) { return null; }

        /// <summary>
        /// Generates and displays edges for specified vertex
        /// </summary>
        /// <param name="vc">Vertex control</param>
        /// <param name="edgeType">Type of edges to display</param>
        /// <param name="defaultVisibility">Default edge visibility on layout</param>
        public virtual void GenerateEdgesForVertex(VertexControl vc, EdgesType edgeType, Visibility defaultVisibility = System.Windows.Visibility.Visible) { }

        /// <summary>
        /// Value overloaded for extensibility purposes. Indicates if ER will be performed on Compute().
        /// </summary>
        public virtual bool IsEdgeRoutingEnabled { get { return false; } }

        /// <summary>
        /// Use edge curving technique for smoother edges. Default value is false.
        /// </summary>
        public virtual bool EdgeCurvingEnabled { get; set; }

        /// <summary>
        /// This is roughly the length of each line segment in the polyline
        /// approximation to a continuous curve in WPF units.  The smaller the
        /// number the smoother the curve, but slower the performance. Default is 8.
        /// </summary>
        public virtual double EdgeCurvingTolerance { get; set; }

        /// <summary>
        /// Radius of a self-loop edge, which is drawn as a circle. Default is 20.
        /// </summary>
        public virtual double EdgeSelfLoopCircleRadius { get; set; }

        /// <summary>
        /// Offset from the corner of the vertex. Useful for custom vertex shapes. Default is 10,10.
        /// </summary>
        public virtual Point EdgeSelfLoopCircleOffset { get; set; }

        /// <summary>
        /// Show self looped edges on vertices. Default value is true.
        /// </summary>
        public virtual bool EdgeShowSelfLooped { get; set; }

        #endregion


        /// <summary>
        /// Use native object measure and arrange algorithm. This results in the odd zoom control operability but differently handles object coordinates.
        /// </summary>
        public bool UseNativeObjectArrange { get; set; }

        #region DP - ExternalSettings
        public static readonly DependencyProperty ExternalSettingsProperty = DependencyProperty.Register("ExternalSettingsOnly", typeof(object),
                                        typeof(GraphAreaBase), new PropertyMetadata(null));
        /// <summary>
        ///User-defined settings storage for using in templates and converters
        /// </summary>
        public object ExternalSettings
        {
            get { return (object)GetValue(ExternalSettingsProperty); }
            set { SetValue(ExternalSettingsProperty, value); }
        }
        #endregion

        #region DP - Origo

        /// <summary>
        /// Gets or sets the virtual origo of the canvas.
        /// </summary>
        public Point Origo
        {
            get { return (Point)GetValue(OrigoProperty); }
            set { SetValue(OrigoProperty, value); }
        }

        public static readonly DependencyProperty OrigoProperty =
            DependencyProperty.Register("Origo", typeof(Point), typeof(GraphAreaBase),
                new FrameworkPropertyMetadata(
                    new Point(),
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

        #endregion

        #region DP - Animations

        /// <summary>
        /// Gets or sets vertex and edge controls animation
        /// </summary>
        public MoveAnimationBase MoveAnimation
        {
            get { return (MoveAnimationBase)GetValue(MoveAnimationProperty); }
            set { SetValue(MoveAnimationProperty, value); }
        }

        public static readonly DependencyProperty MoveAnimationProperty =
            DependencyProperty.Register("MoveAnimation", typeof(MoveAnimationBase), typeof(GraphAreaBase), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets vertex and edge controls delete animation
        /// </summary>
        public IOneWayControlAnimation DeleteAnimation
        {
            get { return (IOneWayControlAnimation)GetValue(DeleteAnimationProperty); }
            set { SetValue(DeleteAnimationProperty, value); }
        }

        public static readonly DependencyProperty DeleteAnimationProperty =
            DependencyProperty.Register("DeleteAnimation", typeof(IOneWayControlAnimation), typeof(GraphAreaBase), new UIPropertyMetadata(DeleteAnimationPropertyChanged));

        private static void DeleteAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is IOneWayControlAnimation)
                (e.OldValue as IOneWayControlAnimation).Completed -= GraphAreaBase_Completed;

            (e.NewValue as IOneWayControlAnimation).Completed += GraphAreaBase_Completed;
            
        }

        static void GraphAreaBase_Completed(object sender, ControlEventArgs e)
        {
            if (e.Control == null || e.Control.RootArea == null) return;
            e.Control.RootArea.Children.Remove(e.Control as UIElement);
            e.Control.Clean();
        }

        /// <summary>
        /// Gets or sets vertex and edge controls mouse over animation
        /// </summary>
        public IBidirectionalControlAnimation MouseOverAnimation
        {
            get { return (IBidirectionalControlAnimation)GetValue(MouseOverAnimationProperty); }
            set { SetValue(MouseOverAnimationProperty, value); }
        }

        public static readonly DependencyProperty MouseOverAnimationProperty =
            DependencyProperty.Register("MouseOverAnimation", typeof(IBidirectionalControlAnimation), typeof(GraphAreaBase), new UIPropertyMetadata(null));

        #endregion

        #region Measure & Arrange

        /// <summary>
        /// The position of the topLeft corner of the most top-left or top left object if UseNativeObjectArrange == false
        /// vertex.
        /// </summary>
        public Point TopLeft;

        /// <summary>
        /// The position of the bottom right corner of the most or bottom right object if UseNativeObjectArrange == false
        /// bottom-right vertex.
        /// </summary>
        public Point BottomRight;

        /// <summary>
        /// Translation of the GraphArea object
        /// </summary>
        public Vector Translation { get; private set; }

        /// <summary>
        /// Gets or sets additional area space for each side of GraphArea expanded by specified amount
        /// Useful for showing scale animation near the area borders (no object clipping thanks to expanded area)
        /// 0 by default.
        /// </summary>
        public Size SideExpansionSize { get; set; }

        public Size StrictSize { get; set; }

        /// <summary>
        /// Gets or sets if edge route paths must be taken into consideration while determining area size
        /// </summary>
        private bool countRoutePaths = true;

        /// <summary>
        /// Arranges the size of the control.
        /// </summary>
        /// <param name="arrangeSize">The arranged size of the control.</param>
        /// <returns>The size of the control.</returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var translate = new Vector(-TopLeft.X, -TopLeft.Y);
            Vector graphSize = (BottomRight - TopLeft);

            if (double.IsNaN(graphSize.X) || double.IsNaN(graphSize.Y) ||
                 double.IsInfinity(graphSize.X) || double.IsInfinity(graphSize.Y))
                translate = new Vector(0, 0);

            Translation = translate;

            graphSize = InternalChildren.Count > 0
                            ? new Vector(double.NegativeInfinity, double.NegativeInfinity)
                            : new Vector(0, 0);

            Point minPoint = new Point(double.PositiveInfinity, double.PositiveInfinity);
            Point maxPoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
            //translate with the topLeft

            foreach (UIElement child in InternalChildren)
            {
                double x = GetX(child);
                double y = GetY(child);
                var ec = child as EdgeControl;

                if (double.IsNaN(x) || double.IsNaN(y))
                {
                    //not a vertex, set the coordinates of the top-left corner
                    if (!UseNativeObjectArrange && ec != null)
                    {
                        x = 0;
                        y = 0;
                    }
                    else
                    {
                        x = double.IsNaN(x) ? translate.X + SideExpansionSize.Width : x;
                        y = double.IsNaN(y) ? translate.Y + SideExpansionSize.Width : y;
                    }

                    if (countRoutePaths && ec != null && (ec.Edge as IRoutingInfo).RoutingPoints != null)
                    {
                        foreach (var item in (ec.Edge as IRoutingInfo).RoutingPoints)
                        {
                            if (UseNativeObjectArrange)
                            {
                                graphSize.X = Math.Max(0, Math.Max(graphSize.X, Math.Abs(item.X)));
                                graphSize.Y = Math.Max(0, Math.Max(graphSize.Y, Math.Abs(item.Y)));
                            }
                            else
                            {
                                minPoint = new Point(Math.Min(minPoint.X, item.X), Math.Min(minPoint.Y, item.Y));
                                maxPoint = new Point(Math.Max(maxPoint.X, item.X), Math.Max(maxPoint.Y, item.Y));
                            }
                        }
                    }

                }
                else
                {
                    //dont't translate if we are using custom arrange
                    if (UseNativeObjectArrange)
                    {
                        x += translate.X + SideExpansionSize.Width;
                        y += translate.Y + SideExpansionSize.Width;
                    }

                    //get the top-left corner
                    x -= child.DesiredSize.Width * 0.5;
                    y -= child.DesiredSize.Height * 0.5;
                    if (!UseNativeObjectArrange)
                    {
                        minPoint = new Point(Math.Min(minPoint.X, x), Math.Min(minPoint.Y, y));
                        maxPoint = new Point(Math.Max(maxPoint.X, x), Math.Max(maxPoint.Y, y));
                    }
                }

                child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
               // if (x > 0 && y > 0)
                if (UseNativeObjectArrange)
                {
                    graphSize.X = Math.Max(0, Math.Max(graphSize.X, Math.Abs(x) + child.DesiredSize.Width));
                    graphSize.Y = Math.Max(0, Math.Max(graphSize.Y, Math.Abs(y) + child.DesiredSize.Height));
                }
            }

            if (!UseNativeObjectArrange)
            {
                graphSize.X = Math.Max(0, maxPoint.X - minPoint.X);
                graphSize.Y = Math.Max(0, maxPoint.Y - minPoint.Y);
                //TODO: probably this is not the best way to calc children width
                if (graphSize.X == 0 && Children.Count > 0) graphSize.X = Children[0].DesiredSize.Width * Children.Count;
                if (graphSize.Y == 0 && Children.Count > 0) graphSize.Y = Children[0].DesiredSize.Height * Children.Count;
            }

            StrictSize = new Size(graphSize.X, graphSize.Y);

            if (!UseNativeObjectArrange) return new Size(graphSize.X + Math.Abs(minPoint.X) + SideExpansionSize.Width*2, graphSize.Y + Math.Abs(minPoint.Y) + SideExpansionSize.Height*2);
            else return new Size(graphSize.X + SideExpansionSize.Width*2, graphSize.Y + SideExpansionSize.Height*2);
        }

        /// <summary>
        /// Overridden measure. It calculates a size where all of 
        /// of the vertices are visible.
        /// </summary>
        /// <param name="constraint">The size constraint.</param>
        /// <returns>The calculated size.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            TopLeft = new Point(double.PositiveInfinity, double.PositiveInfinity);
            BottomRight = new Point(double.NegativeInfinity, double.NegativeInfinity);

            foreach (UIElement child in InternalChildren)
            {
                //measure the child
                child.Measure(constraint);

                //get the position of the vertex
                double left = GetX(child);
                double top = GetY(child);

                double halfWidth = child.DesiredSize.Width * 0.5;
                double halfHeight = child.DesiredSize.Height * 0.5;

                if (double.IsNaN(left) || double.IsNaN(top))
                {
                    left = halfWidth;
                    top = halfHeight;
                    var ec = child as EdgeControl;
                    if (countRoutePaths && ec != null && (ec.Edge as IRoutingInfo).RoutingPoints != null)
                    {
                        foreach (var item in (ec.Edge as IRoutingInfo).RoutingPoints)
                        {
                            //get the top left corner point
                            TopLeft.X = Math.Min(TopLeft.X, item.X);
                            TopLeft.Y = Math.Min(TopLeft.Y, item.Y);

                            //calculate the bottom right corner point
                            BottomRight.X = Math.Max(BottomRight.X, item.X);
                            BottomRight.Y = Math.Max(BottomRight.Y, item.Y);
                        }
                    }
                }
                else
                {
                    //get the top left corner point
                    TopLeft.X = Math.Min(TopLeft.X, left - halfWidth - Origo.X);
                    TopLeft.Y = Math.Min(TopLeft.Y, top - halfHeight - Origo.Y);

                    //calculate the bottom right corner point
                    BottomRight.X = Math.Max(BottomRight.X, left + halfWidth - Origo.X);
                    BottomRight.Y = Math.Max(BottomRight.Y, top + halfHeight - Origo.Y);
                }

            }

            var graphSize = (Size)(BottomRight - TopLeft);
            graphSize.Width = Math.Max(0, graphSize.Width) + (UseNativeObjectArrange ? 0 : Math.Abs(TopLeft.X)) + SideExpansionSize.Width*2;
            graphSize.Height = Math.Max(0, graphSize.Height) + (UseNativeObjectArrange ? 0 : Math.Abs(TopLeft.Y)) + SideExpansionSize.Height*2;

            if (double.IsNaN(graphSize.Width) || double.IsNaN(graphSize.Height) ||
                 double.IsInfinity(graphSize.Width) || double.IsInfinity(graphSize.Height))
                return new Size(0, 0);

            //Debug.WriteLine("GA: " + graphSize.Width + "  /  " + graphSize.Height);

            if(dirtySpeedHack) return new Size(50000, 50000);
            else return graphSize;
        }
        #endregion

        /// <summary>
        /// Tmp drag speed solution switch
        /// </summary>
        public bool dirtySpeedHack;

        #region Attached Properties
        [AttachedPropertyBrowsableForChildren]
        public static double GetX(DependencyObject obj)
        {
            return (double)obj.GetValue(XProperty);
        }

        public static void SetX(DependencyObject obj, double value, bool alsoSetFinal = true)
        {
            obj.SetValue(XProperty, value);
            if(alsoSetFinal)
                obj.SetValue(FinalXProperty, value);
        }

        [AttachedPropertyBrowsableForChildren]
        public static double GetY(DependencyObject obj)
        {
            return (double)obj.GetValue(YProperty);
        }

        public static void SetY(DependencyObject obj, double value, bool alsoSetFinal = false)
        {
            obj.SetValue(YProperty, value);
            if (alsoSetFinal)
                obj.SetValue(FinalYProperty, value);
        }


        [AttachedPropertyBrowsableForChildren]
        public static double GetFinalX(DependencyObject obj)
        {
            return (double)obj.GetValue(FinalXProperty);
        }

        public static void SetFinalX(DependencyObject obj, double value)
        {
            obj.SetValue(FinalXProperty, value);
        }

        [AttachedPropertyBrowsableForChildren]
        public static double GetFinalY(DependencyObject obj)
        {
            return (double)obj.GetValue(FinalYProperty);
        }

        public static void SetFinalY(DependencyObject obj, double value)
        {
            obj.SetValue(FinalYProperty, value);
        }

        #endregion


        /// <summary>
        /// Gets or sets special mode for WinForms interoperability
        /// </summary>
        public bool EnableWinFormsHostingMode { get; set; }

        /// <summary>
        /// Enables parallel edges. All edges between the same nodes will be separated by ParallelEdgeDistance value.
        /// This is post-process procedure and it may be performance-costly.
        /// </summary>
        public bool EnableParallelEdges { get; set; }

        /// <summary>
        /// Distance by which edges are parallelized if EnableParallelEdges is true. Default value is 5.
        /// </summary>
        public int ParallelEdgeDistance { get; set; }

    }
}
