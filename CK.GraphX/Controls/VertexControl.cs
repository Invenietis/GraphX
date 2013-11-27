using System.Windows;
using System.Windows.Controls;
using System;
using System.Linq;
using System.ComponentModel;
using System.Windows.Media;
using GraphX.Models;

namespace GraphX
{
	/// <summary>
	/// Visual vertex control
	/// </summary>
    [Serializable]
    public class VertexControl: Control, IGraphControl
    {
        #region Properties

        /// <summary>
        /// Provides settings for event calls within single vertex control
        /// </summary>
        public VertexEventOptions EventOptions { get; private set; }

        /// <summary>
        /// Gets or sets actual shape form of vertex control (affects mostly math calculations such edges connectors)
        /// </summary>
        public VertexShape MathShape
        {
            get { return (VertexShape)GetValue(VertexShapeProperty); }
            set { SetValue(VertexShapeProperty, value); }
        }

        public static readonly DependencyProperty VertexShapeProperty =
            DependencyProperty.Register("VertexShape", typeof(VertexShape), typeof(VertexControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets vertex data object
        /// </summary>
		public object Vertex
		{
			get { return GetValue( VertexProperty ); }
			set { SetValue( VertexProperty, value ); }
		}

		public static readonly DependencyProperty VertexProperty =
            DependencyProperty.Register("Vertex", typeof(object), typeof(VertexControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets vertex control parent GraphArea object (don't need to be set manualy)
        /// </summary>
        public GraphAreaBase RootArea
        {
            get { return (GraphAreaBase)GetValue(RootCanvasProperty); }
            set { SetValue(RootCanvasProperty, value); }
        }

        public static readonly DependencyProperty RootCanvasProperty =
            DependencyProperty.Register("RootArea", typeof(GraphAreaBase), typeof(VertexControl), new UIPropertyMetadata(null));

		static VertexControl()
		{
			//override the StyleKey Property
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VertexControl), new FrameworkPropertyMetadata(typeof(VertexControl)));
		}
        #endregion

        #region Position trace feature

        /// <summary>
        /// Fires when IsPositionTraceEnabled property set and object changes its coordinates.
        /// </summary>
        public event VertexPositionChangedEH PositionChanged;

        protected void OnPositionChanged(Point offset, Point pos)
        {
            if (PositionChanged != null)
                PositionChanged.Invoke(this, new VertexPositionEventArgs(offset, pos, this));
        }

        private DependencyPropertyDescriptor sxDescriptor;
        private DependencyPropertyDescriptor syDescriptor;
        internal void updatePositionTraceState()
        {
            if (EventOptions.PositionChangeNotification)
            {
                sxDescriptor = DependencyPropertyDescriptor.FromProperty(GraphAreaBase.XProperty, typeof(VertexControl));
                sxDescriptor.AddValueChanged(this, source_PositionChanged);
                syDescriptor = DependencyPropertyDescriptor.FromProperty(GraphAreaBase.YProperty, typeof(VertexControl));
                syDescriptor.AddValueChanged(this, source_PositionChanged);
            }
            else
            {
                if (sxDescriptor != null)
                    sxDescriptor.RemoveValueChanged(this, source_PositionChanged);
                if (syDescriptor != null)
                    syDescriptor.RemoveValueChanged(this, source_PositionChanged);
            }
        }

        private void source_PositionChanged(object sender, EventArgs e)
        {
            OnPositionChanged(GetPosition(true), GetPosition());
        }
        #endregion

        #region Position methods
        /// <summary>
        /// Set attached coordinates X and Y
        /// </summary>
        /// <param name="pt"></param>
        public void SetPosition(Point pt, bool alsoFinal = true)
        {
            GraphAreaBase.SetX(this, pt.X, alsoFinal);
            GraphAreaBase.SetY(this, pt.Y, alsoFinal);
        }

        /// <summary>
        /// Get control position on the GraphArea panel in attached coords X and Y
        /// </summary>
        /// <param name="offset">Return visual offset relative to parent host instead of coordinates</param>
        public Point GetPosition(bool offset = false, bool final = false, bool round = false)
        {
            if (offset)
            {
                var of = VisualTreeHelper.GetOffset(this);
                return round ? new Point((int)of.X, (int)of.Y) : new Point(of.X, of.Y);
            }
            else
            {
                if (round) return new Point(final ? (int)GraphAreaBase.GetFinalX(this) : (int)GraphAreaBase.GetX(this), final ? (int)GraphAreaBase.GetFinalY(this) : (int)GraphAreaBase.GetY(this));
                else return new Point(final ? GraphAreaBase.GetFinalX(this) : GraphAreaBase.GetX(this), final ? GraphAreaBase.GetFinalY(this) : GraphAreaBase.GetY(this));
            }
        }        
        #endregion

        /// <summary>
        /// Create vertex visual control
        /// </summary>
        /// <param name="vertexData">Vertex data object</param>
        /// <param name="tracePositionChange">Listen for the vertex position changed events and fire corresponding event</param>
        /// <param name="bindToDataObject">Bind DataContext to the Vertex data. True by default. </param>
        public VertexControl(object vertexData, bool tracePositionChange = true, bool bindToDataObject = true)
        {
            if (bindToDataObject) DataContext = vertexData;
            Vertex = vertexData;

            EventOptions = new VertexEventOptions(this) { PositionChangeNotification = tracePositionChange };
            foreach(var item in Enum.GetValues(typeof(EventType)).Cast<EventType>())
                updateEventhandling(item);
        }

        #region Events handling

        internal void updateEventhandling(EventType typ)
        {
            switch (typ)
            {
                case EventType.MouseClick:
                    if (EventOptions.MouseClickEnabled) MouseDown += VertexControl_Down;
                    else MouseDown -= VertexControl_Down;
                    break;
                case EventType.MouseDoubleClick:
                    if (EventOptions.MouseDoubleClickEnabled) MouseDoubleClick += VertexControl_MouseDoubleClick;
                    else MouseDoubleClick -= VertexControl_MouseDoubleClick;
                    break;
                case EventType.MouseMove:
                    if (EventOptions.MouseMoveEnabled) MouseMove += VertexControl_MouseMove;
                    else MouseMove -= VertexControl_MouseMove;
                    break;
                case EventType.MouseEnter:
                    if (EventOptions.MouseEnterEnabled) MouseEnter += VertexControl_MouseEnter;
                    else MouseEnter -= VertexControl_MouseEnter;
                    break;
                case EventType.MouseLeave:
                    if (EventOptions.MouseLeaveEnabled) MouseLeave += VertexControl_MouseLeave;
                    else MouseLeave -= VertexControl_MouseLeave;
                    break;
                case EventType.PositionChangeNotify:
                    updatePositionTraceState();
                    break;
            }
        }

        void VertexControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (RootArea != null && Visibility == System.Windows.Visibility.Visible)
                RootArea.OnVertexMouseLeave(this);
            //e.Handled = true;
        }

        void VertexControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (RootArea != null && Visibility == System.Windows.Visibility.Visible)
                RootArea.OnVertexMouseEnter(this);
           // e.Handled = true;
        }

        void VertexControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (RootArea != null)
                RootArea.OnVertexMouseMove(this);
        }

        void VertexControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (RootArea != null && Visibility == System.Windows.Visibility.Visible)
                RootArea.OnVertexDoubleClick(this);
            e.Handled = true;
        }

        void VertexControl_Down(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (RootArea != null && Visibility == System.Windows.Visibility.Visible)
                RootArea.OnVertexSelected(this, e);
            e.Handled = true;
        }
        #endregion

        /// <summary>
        /// Cleans all potential memory-holding code
        /// </summary>
        public void Clean()
        {
            Vertex = null;
            RootArea = null;
            HighlightBehaviour.SetIsHighlightEnabled(this, false);
            DragBehaviour.SetIsDragEnabled(this, false);
            if (EventOptions != null)
            {
                EventOptions.PositionChangeNotification = false;
                EventOptions.Clean();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }
	}
}