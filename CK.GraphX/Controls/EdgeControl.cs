﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;

namespace GraphX
{
    /// <summary>
    /// Visual edge control
    /// </summary>
    [Serializable]
    [TemplatePart( Name = "PART_edgePath", Type = typeof( Path ) )]
    [TemplatePart( Name = "PART_edgeArrowPath", Type = typeof( Path ) )]
    [TemplatePart( Name = "PART_edgeLabel", Type = typeof( EdgeLabelControl ) )]
    public class EdgeControl : Control, IGraphControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register( "Source",
                                                                                               typeof( VertexControl ),
                                                                                               typeof( EdgeControl ),
                                                                                               new UIPropertyMetadata( null ) );


        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register( "Target",
                                                                                               typeof( VertexControl ),
                                                                                               typeof( EdgeControl ),
                                                                                               new UIPropertyMetadata( null ) );

        /*public static readonly DependencyProperty RoutePointsProperty = DependencyProperty.Register( "RoutePoints",
                                                                                                    typeof( Point[] ),
                                                                                                    typeof(EdgeControl),
                                                                                                    new UIPropertyMetadata(
                                                                                                        null ) );*/

        public static readonly DependencyProperty EdgeProperty = DependencyProperty.Register( "Edge", typeof( object ),
                                                                                             typeof( EdgeControl ),
                                                                                             new PropertyMetadata( null ) );

        public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner( typeof( EdgeControl ),
                                                                                                                    new UIPropertyMetadata( 5.0 ) );
        public GraphAreaBase RootArea
        {
            get { return (GraphAreaBase)GetValue( RootCanvasProperty ); }
            set { SetValue( RootCanvasProperty, value ); }
        }

        public static readonly DependencyProperty RootCanvasProperty =
            DependencyProperty.Register( "RootArea", typeof( GraphAreaBase ), typeof( EdgeControl ), new UIPropertyMetadata( null ) );

        #endregion

        #region Properties

        #region DashStyle

        public static readonly DependencyProperty DashStyleProperty = DependencyProperty.Register( "DashStyle",
                                                                                       typeof( EdgeDashStyle ),
                                                                                       typeof( EdgeControl ),
                                                                                       new UIPropertyMetadata( EdgeDashStyle.Solid, dashstyle_changed ) );

        private static void dashstyle_changed( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            var ec = d as EdgeControl;
            switch( (EdgeDashStyle)e.NewValue )
            {
                case EdgeDashStyle.Solid:
                    ec.strokeDashArray = null;
                    break;

                case EdgeDashStyle.Dash:
                    ec.strokeDashArray = new DoubleCollection( new Double[] { 4.0, 2.0 } );
                    break;
                case EdgeDashStyle.Dot:
                    ec.strokeDashArray = new DoubleCollection( new Double[] { 1.0, 2.0 } );
                    break;

                case EdgeDashStyle.DashDot:
                    ec.strokeDashArray = new DoubleCollection( new Double[] { 4.0, 2.0, 1.0, 2.0 } );
                    break;

                case EdgeDashStyle.DashDotDot:
                    ec.strokeDashArray = new DoubleCollection( new Double[] { 4.0, 2.0, 1.0, 2.0, 1.0, 2.0 } );
                    break;

                default:
                    ec.strokeDashArray = null;
                    break;
            }
            ec.UpdateEdge();
        }

        private DoubleCollection strokeDashArray { get; set; }

        /// <summary>
        /// Gets or sets edge dash style
        /// </summary>
        public EdgeDashStyle DashStyle
        {
            get { return (EdgeDashStyle)GetValue( DashStyleProperty ); }
            set { SetValue( DashStyleProperty, value ); }
        }
        #endregion

        private bool canbeparallel = true;
        /// <summary>
        /// Gets or sets if this edge can be paralellized if GraphArea.EnableParallelEdges is true.
        /// If not it will be drawn by default.
        /// </summary>
        public bool CanBeParallel { get { return canbeparallel; } set { canbeparallel = value; } }

        /// <summary>
        /// Gets if this edge is self looped (have same Source and Target)
        /// </summary>
        public bool IsSelfLooped
        {
            get { return Source != null && Target != null && Source.Vertex == Target.Vertex; }
        }

        /// <summary>
        /// Show arrows on the edge ends. Default value is true.
        /// </summary>
        public bool ShowArrows { get { return showarrows; } set { showarrows = value; UpdateEdge(); } }
        private bool showarrows;

        /// <summary>
        /// Show edge label.Default value is False.
        /// </summary>
        public bool ShowLabel { get { return showlabel; } set { showlabel = value; UpdateEdge(); } }
        private bool showlabel;

        /// <summary>
        ///  Gets or Sets that user controls the path geometry object or it is generated automatically
        /// </summary>
        public bool ManualDrawing { get; set; }

        /// <summary>
        /// Geometry object that represents visual edge path. Applied in OnApplyTemplate and OnRender.
        /// </summary>
        private Geometry _linegeometry;

        /// <summary>
        /// Geometry object that represents visual edge arrow. Applied in OnApplyTemplate and OnRender.
        /// </summary>
        private PathGeometry _arrowgeometry;

        /// <summary>
        /// Templated Path object to operate with routed path
        /// </summary>
        private Path LinePathObject;
        /// <summary>
        /// Templated Path object to operate with routed path arrow head
        /// </summary>
        private Path ArrowPathObject;
        /// <summary>
        /// Templated label control to display labels
        /// </summary>
        private EdgeLabelControl EdgeLabelControl;

        public EdgeEventOptions EventOptions { get; private set; }

        /// <summary>
        /// Source visual vertex object
        /// </summary>
        public VertexControl Source
        {
            get { return (VertexControl)GetValue( SourceProperty ); }
            set { SetValue( SourceProperty, value ); }
        }
        /// <summary>
        /// Target visual vertex object
        /// </summary>
        public VertexControl Target
        {
            get { return (VertexControl)GetValue( TargetProperty ); }
            set { SetValue( TargetProperty, value ); }
        }

        /*public Point[] RoutePoints
        {
            get { return (Point[])GetValue( RoutePointsProperty ); }
            set { SetValue( RoutePointsProperty, value ); }
        }*/

        /// <summary>
        /// Data edge object
        /// </summary>
        public object Edge
        {
            get { return GetValue( EdgeProperty ); }
            set { SetValue( EdgeProperty, value ); }
        }

        /// <summary>
        /// Custom edge thickness
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue( StrokeThicknessProperty ); }
            set { SetValue( StrokeThicknessProperty, value ); }
        }
        #endregion

        #region public Clean()
        public void Clean()
        {
            //cleanVertexTracer(true);
            //cleanVertexTracer(false);
            if( Source != null )
                Source.PositionChanged -= source_PositionChanged;
            if( Target != null )
                Target.PositionChanged -= source_PositionChanged;
            _oldSource = _oldTarget = null;
            Source = null;
            Target = null;
            Edge = null;
            RootArea = null;
            HighlightBehaviour.SetIsHighlightEnabled( this, false );
            DragBehaviour.SetIsDragEnabled( this, false );
            _linegeometry = null;
            _arrowgeometry = null;
            LinePathObject = null;
            ArrowPathObject = null;
            if( EventOptions != null )
                EventOptions.Clean();
        }
        #endregion

        public EdgeControl( VertexControl source, VertexControl target, object edge, bool showLabels = false, bool showArrows = true )
        {
            DataContext = edge;
            Source = source; Target = target;
            Edge = edge; DataContext = edge;
            ShowArrows = showArrows;
            ShowLabel = showLabels;

            EventOptions = new EdgeEventOptions( this );
            foreach( var item in Enum.GetValues( typeof( EventType ) ).Cast<EventType>() )
                updateEventhandling( item );

            if( source != null )
            {
                _sourceTrace = source.EventOptions.PositionChangeNotification;
                source.EventOptions.PositionChangeNotification = true;
                source.PositionChanged += source_PositionChanged;
            }
            if( target != null )
            {
                _targetTrace = target.EventOptions.PositionChangeNotification;
                target.EventOptions.PositionChangeNotification = true;
                target.PositionChanged += source_PositionChanged;
            }

            var dpd = DependencyPropertyDescriptor.FromProperty( EdgeControl.SourceProperty, typeof( EdgeControl ) );
            if( dpd != null ) dpd.AddValueChanged( this, sChanged );
            dpd = DependencyPropertyDescriptor.FromProperty( EdgeControl.TargetProperty, typeof( EdgeControl ) );
            if( dpd != null ) dpd.AddValueChanged( this, tChanged );
        }

        static EdgeControl()
        {
            //override the StyleKey
            DefaultStyleKeyProperty.OverrideMetadata( typeof( EdgeControl ), new FrameworkPropertyMetadata( typeof( EdgeControl ) ) );
        }

        public EdgeControl()
            : this( null, null, null )
        {
        }

        #region Vertex position tracing
        private void sChanged( object sender, EventArgs e )
        {
            if( _oldSource != null )
            {
                _oldSource.PositionChanged -= source_PositionChanged;
                _oldSource.EventOptions.PositionChangeNotification = _sourceTrace;
            }
            _oldSource = Source;
            if( Source != null )
            {
                _sourceTrace = Source.EventOptions.PositionChangeNotification;
                Source.EventOptions.PositionChangeNotification = true;
                Source.PositionChanged += source_PositionChanged;
            }
        }
        private void tChanged( object sender, EventArgs e )
        {
            if( _oldTarget != null )
            {
                _oldTarget.PositionChanged -= source_PositionChanged;
                _oldTarget.EventOptions.PositionChangeNotification = _targetTrace;
            }
            _oldTarget = Target;
            if( Target != null )
            {
                _targetTrace = Target.EventOptions.PositionChangeNotification;
                Target.EventOptions.PositionChangeNotification = true;
                Target.PositionChanged += source_PositionChanged;
            }
        }

        private void source_PositionChanged( object sender, EventArgs e )
        {
            UpdateEdge();
        }

        private bool _sourceTrace;
        private bool _targetTrace;
        private VertexControl _oldSource;
        private VertexControl _oldTarget;
        #endregion

        #region Position methods
        /// <summary>
        /// Set attached coordinates X and Y
        /// </summary>
        /// <param name="pt"></param>
        public void SetPosition( Point pt, bool alsoFinal = true )
        {
            GraphAreaBase.SetX( this, pt.X, alsoFinal );
            GraphAreaBase.SetY( this, pt.Y, alsoFinal );
        }

        /// <summary>
        /// Get control position on the GraphArea panel in attached coords X and Y
        /// </summary>
        /// <param name="offset">Return visual offset relative to parent host instead of coordinates</param>
        public Point GetPosition( bool offset = false, bool final = false, bool round = false )
        {
            if( offset )
            {
                var of = VisualTreeHelper.GetOffset( this );
                return new Point( of.X, of.Y );
            }
            else return new Point( final ? GraphAreaBase.GetFinalX( this ) : GraphAreaBase.GetX( this ), final ? GraphAreaBase.GetFinalY( this ) : GraphAreaBase.GetY( this ) );
        }
        #endregion

        #region Event handlers

        internal void updateEventhandling( EventType typ )
        {
            switch( typ )
            {
                case EventType.MouseClick:
                    if( EventOptions.MouseClickEnabled ) MouseDown += GraphEdge_MouseDown;
                    else MouseDown -= GraphEdge_MouseDown;
                    break;
                case EventType.MouseDoubleClick:
                    if( EventOptions.MouseDoubleClickEnabled ) MouseDoubleClick += EdgeControl_MouseDoubleClick;
                    else MouseDoubleClick -= EdgeControl_MouseDoubleClick;
                    break;
                case EventType.MouseEnter:
                    if( EventOptions.MouseEnterEnabled ) MouseEnter += EdgeControl_MouseEnter;
                    else MouseEnter -= EdgeControl_MouseEnter;
                    break;
                case EventType.MouseLeave:
                    if( EventOptions.MouseLeaveEnabled ) MouseLeave += EdgeControl_MouseLeave;
                    else MouseLeave -= EdgeControl_MouseLeave;
                    break;

                case EventType.MouseMove:
                    if( EventOptions.MouseMoveEnabled ) MouseMove += EdgeControl_MouseMove;
                    else MouseMove -= EdgeControl_MouseMove;
                    break;
            }
        }

        void EdgeControl_MouseLeave( object sender, System.Windows.Input.MouseEventArgs e )
        {
            if( RootArea != null && Visibility == System.Windows.Visibility.Visible )
                RootArea.OnEdgeMouseLeave( this );
            // e.Handled = true;
        }

        void EdgeControl_MouseEnter( object sender, System.Windows.Input.MouseEventArgs e )
        {
            if( RootArea != null && Visibility == System.Windows.Visibility.Visible )
                RootArea.OnEdgeMouseEnter( this );
            // e.Handled = true;
        }

        void EdgeControl_MouseMove( object sender, System.Windows.Input.MouseEventArgs e )
        {
            if( RootArea != null && Visibility == System.Windows.Visibility.Visible )
                RootArea.OnEdgeMouseMove( this );
            e.Handled = true;
        }

        void EdgeControl_MouseDoubleClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            if( RootArea != null && Visibility == System.Windows.Visibility.Visible )
                RootArea.OnEdgeDoubleClick( this );
            e.Handled = true;
        }

        void GraphEdge_MouseDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
        {
            if( RootArea != null && Visibility == System.Windows.Visibility.Visible )
                RootArea.OnEdgeSelected( this );
            e.Handled = true;
        }

        #endregion


        #region Manual path controls
        /// <summary>
        /// Gets current edge path geometry object
        /// </summary>
        public PathGeometry GetEdgePathManually()
        {
            if( !ManualDrawing ) return null;
            return _linegeometry as PathGeometry;
        }

        /// <summary>
        /// Sets current edge path geometry object
        /// </summary>
        public void SetEdgePathManually( PathGeometry geo )
        {
            if( !ManualDrawing ) return;
            _linegeometry = geo;
            UpdateEdge();
        }
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if( this.Template != null )
            {
                LinePathObject = Template.FindName( "PART_edgePath", this ) as Path;
                if( LinePathObject == null ) throw new GX_ObjectNotFoundException( "EdgeControl Template -> Edge template must contain 'PART_edgePath' Path object to draw route points!" );
                LinePathObject.Data = _linegeometry;
                ArrowPathObject = Template.FindName( "PART_edgeArrowPath", this ) as Path;
                if( ArrowPathObject == null ) Debug.WriteLine( "EdgeControl Template -> Edge template have no 'PART_edgeArrowPath' Path object to draw!" );
                else ArrowPathObject.Data = _arrowgeometry;
                EdgeLabelControl = Template.FindName( "PART_edgeLabel", this ) as EdgeLabelControl;
                //if (EdgeLabelControl == null) Debug.WriteLine("EdgeControl Template -> Edge template have no 'PART_edgeLabel' object to draw!");
                UpdateEdge();
            }

        }

        protected override void OnRender( DrawingContext drawingContext )
        {
            base.OnRender( drawingContext );
            // if(_geometry!=null)
            //    drawingContext.DrawGeometry(new SolidColorBrush(Colors.Black), new Pen(new SolidColorBrush(Colors.Black), 2), _geometry);
        }



        #region public PrepareEdgePath()

        internal void UpdateEdge()
        {
            if( Visibility == System.Windows.Visibility.Visible && LinePathObject != null )
            {
                PrepareEdgePath( true );
                LinePathObject.Data = _linegeometry;
                LinePathObject.StrokeDashArray = strokeDashArray;

                if( ArrowPathObject != null )
                {
                    if( ShowArrows ) ArrowPathObject.Data = _arrowgeometry;
                    else ArrowPathObject.Data = null;
                }
                if( EdgeLabelControl != null )
                    EdgeLabelControl.Visibility = ShowLabel ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }

        internal int SourceOffset;
        internal int TargetOffset;

        private Point GetOffset( VertexControl source, VertexControl target, int side_distance )
        {
            var sourcepos = source.GetPosition();
            var targetpos = target.GetPosition();

            var main_vector = new Vector( targetpos.X - sourcepos.X, targetpos.Y - sourcepos.Y );
            //get new point coordinate
            var joint = new Point(
                 sourcepos.X + side_distance * (main_vector.Y / main_vector.Length),
                 sourcepos.Y - side_distance * (main_vector.X / main_vector.Length) );
            return joint;
        }

        /// <summary>
        /// Create and apply edge path using calculated ER parameters stored in edge
        /// </summary>
        /// <param name="useCurrentCoords">Use current vertices coordinates or final coordinates (for.ex if move animation is active final coords will be its destination)</param>
        /// <param name="externalRoutingPoints">Provided custom routing points will be used instead of stored ones.
        public void PrepareEdgePath( bool useCurrentCoords = false, Point[] externalRoutingPoints = null )
        {
            //do not calculate invisible edges
            if( Visibility != System.Windows.Visibility.Visible || Source == null || Target == null || ManualDrawing ) return;

            var template = Template;
            if( template != null )
            {
                #region Get the inputs
                //get the position of the source
                var sourcePos = new Point()
                {
                    X = useCurrentCoords ? GraphAreaBase.GetX( Source ) : GraphAreaBase.GetFinalX( Source ),
                    Y = useCurrentCoords ? GraphAreaBase.GetY( Source ) : GraphAreaBase.GetFinalY( Source )
                };
                //get the size of the source
                var sourceSize = new Size()
                {
                    Width = Source.ActualWidth,
                    Height = Source.ActualHeight
                };
                //get the position of the target
                var targetPos = new Point()
                {
                    X = useCurrentCoords ? GraphAreaBase.GetX( Target ) : GraphAreaBase.GetFinalX( Target ),
                    Y = useCurrentCoords ? GraphAreaBase.GetY( Target ) : GraphAreaBase.GetFinalY( Target )
                };
                //get the size of the target
                var targetSize = new Size()
                {
                    Width = Target.ActualWidth,
                    Height = Target.ActualHeight
                };

                //get the route informations
                var routeInformation = externalRoutingPoints == null ? (Edge as IRoutingInfo).RoutingPoints : externalRoutingPoints;
                #endregion

                //if self looped edge
                if( IsSelfLooped )
                {
                    if( !RootArea.EdgeShowSelfLooped ) return;
                    var pt = new Point( sourcePos.X - sourceSize.Width / 2 + RootArea.EdgeSelfLoopCircleOffset.X - RootArea.EdgeSelfLoopCircleRadius, sourcePos.Y - sourceSize.Height / 2 + RootArea.EdgeSelfLoopCircleOffset.X - RootArea.EdgeSelfLoopCircleRadius );
                    var geo = new EllipseGeometry( pt,
                        RootArea.EdgeSelfLoopCircleRadius, RootArea.EdgeSelfLoopCircleRadius );
                    var dArrowAngle = Math.PI / 2.0;
                    _arrowgeometry = new PathGeometry(); _arrowgeometry.Figures.Add( GeometryHelper.GenerateArrow( new Point( sourcePos.X - sourceSize.Width / 2, sourcePos.Y - sourceSize.Height / 2 ), new Point(), new Point(), dArrowAngle ) );

                    _linegeometry = geo;
                    return;
                }


                bool hasRouteInfo = routeInformation != null && routeInformation.Length > 1;

                //calculate source and target edge attach points
                if( !hasRouteInfo && RootArea.EnableParallelEdges )
                {
                    if( SourceOffset != 0 ) sourcePos = GetOffset( Source, Target, SourceOffset );
                    if( TargetOffset != 0 ) targetPos = GetOffset( Target, Source, TargetOffset );
                }
                Point p1 = GeometryHelper.GetEdgeEndpoint( sourcePos, new Rect( sourceSize ), (hasRouteInfo ? routeInformation[1] : (targetPos)), Source.MathShape );
                Point p2 = GeometryHelper.GetEdgeEndpoint( targetPos, new Rect( targetSize ), hasRouteInfo ? routeInformation[routeInformation.Length - 2] : (sourcePos), Target.MathShape );

                _linegeometry = new PathGeometry(); PathFigure lineFigure = null;
                _arrowgeometry = new PathGeometry(); PathFigure arrowFigure = null;

                //if we have route and route consist of 2 or more points
                if( hasRouteInfo )
                {
                    //replace start and end points with accurate ones
                    var routePoints = routeInformation.ToList();
                    routePoints.Remove( routePoints.First() );
                    routePoints.Remove( routePoints.Last() );
                    routePoints.Insert( 0, p1 );
                    routePoints.Add( p2 );

                    if( RootArea.EdgeCurvingEnabled )
                    {
                        var oPolyLineSegment = GeometryHelper.GetCurveThroughPoints( routePoints.ToArray(), 0.5, RootArea.EdgeCurvingTolerance );
                        lineFigure = GeometryHelper.GetPathFigureFromPathSegments( routePoints[0], true, true, oPolyLineSegment );
                        //get two last points of curved path to generate correct arrow
                        var c_last = oPolyLineSegment.Points.Last();
                        var c_prev = oPolyLineSegment.Points[oPolyLineSegment.Points.Count - 2];
                        arrowFigure = GeometryHelper.GenerateOldArrow( c_prev, c_last );
                        //freeze and create resulting geometry
                        GeometryHelper.TryFreeze( oPolyLineSegment );
                    }
                    else
                    {
                        lineFigure = new PathFigure( p1, new PathSegment[1] { new PolyLineSegment( routePoints, true ) }, false );
                        arrowFigure = GeometryHelper.GenerateOldArrow( routePoints[routePoints.Count - 2], p2 );
                    }

                }
                else // no route defined
                {
                    //!!! Here is the line calculation to not overlap an arrowhead
                    //Vector v = p1 - p2; v = v / v.Length * 5;
                    // Vector n = new Vector(-v.Y, v.X) * 0.7;
                    //segments[0] = new LineSegment(p2 + v, true);
                    lineFigure = new PathFigure( p1, new PathSegment[1] { new LineSegment( p2, true ) }, false );
                    arrowFigure = GeometryHelper.GenerateOldArrow( p1, p2 );
                }
                GeometryHelper.TryFreeze( lineFigure );
                (_linegeometry as PathGeometry).Figures.Add( lineFigure );
                if( arrowFigure != null )
                {
                    GeometryHelper.TryFreeze( arrowFigure );
                    _arrowgeometry.Figures.Add( arrowFigure );
                }
                GeometryHelper.TryFreeze( _linegeometry );
                GeometryHelper.TryFreeze( _arrowgeometry );

            }
            else
            {
                Debug.WriteLine( "PrepareEdgePath() -> Edge template not found! Can't apply path to display edge!" );
            }

        }
        #endregion
    }
}