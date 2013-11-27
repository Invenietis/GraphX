﻿using System;

namespace GraphX
{
    public sealed class VertexEventOptions
    {
        /// <summary>
        /// Gets or sets if MouseMove event should be enabled
        /// </summary>
        public bool MouseMoveEnabled { get { return mousemove; } set { if (mousemove != value) { mousemove = value; _vc.updateEventhandling(EventType.MouseMove); } } }
        private bool mousemove = true;
        /// <summary>
        /// Gets or sets if MouseEnter event should be enabled
        /// </summary>
        public bool MouseEnterEnabled { get { return mouseenter; } set { if (mouseenter != value) { mouseenter = value; _vc.updateEventhandling(EventType.MouseEnter); } } }
        private bool mouseenter = true;
        /// <summary>
        /// Gets or sets if MouseLeave event should be enabled
        /// </summary>
        public bool MouseLeaveEnabled { get { return mouseleave; } set { if (mouseleave != value) { mouseleave = value; _vc.updateEventhandling(EventType.MouseLeave); } } }
        private bool mouseleave = true;
        /// <summary>
        /// Gets or sets if MouseDown event should be enabled
        /// </summary>
        public bool MouseClickEnabled { get { return mouseclick; } set { if (mouseclick != value) { mouseclick = value; _vc.updateEventhandling(EventType.MouseClick); } } }
        private bool mouseclick = true;
        /// <summary>
        /// Gets or sets if MouseDoubleClick event should be enabled
        /// </summary>
        public bool MouseDoubleClickEnabled { get { return mousedblclick; } set { if (mousedblclick != value) { mousedblclick = value; _vc.updateEventhandling(EventType.MouseDoubleClick); } } }
        private bool mousedblclick = true;

        /// <summary>
        /// Gets or sets if position trace enabled. If enabled then PositionChanged event will be rised on each X or Y property change.
        /// True by default. 
        /// </summary>
        public bool PositionChangeNotification { get { return poschange; } set { if (poschange != value) { poschange = value; _vc.updatePositionTraceState(); } } }
        private bool poschange = true;

        private VertexControl _vc;

        public VertexEventOptions(VertexControl vc)
        {
            _vc = vc;
        }

        public void Clean()
        {
            _vc = null;
        }

    }
}
