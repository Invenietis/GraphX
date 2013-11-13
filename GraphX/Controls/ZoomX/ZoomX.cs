using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphX.Controls
{
    [TemplatePart(Name = PART_Presenter, Type = typeof(ContentPresenter))]
    internal sealed class ZoomX: ContentControl
    {
        private const string PART_Presenter = "PART_Presenter";
        private ContentPresenter _presenter;

        ZoomViewModifierMode ModifierMode;
        private Point _mouseDownPos;
        private Vector _startTranslate;

        #region Dummies

        //public event Xceed.Wpf.Toolkit.Zoombox.AreaSelectedEventHandler AreaSelected;

        public double AnimationAccelerationRatio = 0;
        public double AnimationDecelerationRatio = 0;
        public Duration AnimationDuration = new System.Windows.Duration(TimeSpan.FromSeconds(0));
        //private TransformGroup _rt;
        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;


        public void FitToBounds()
        {
        }

        public void ZoomToFill()
        {
        }

        public void ZoomTo(Rect rec)
        {
        }

        public bool IsAnimated { get; set; }

        #endregion

        static ZoomX()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomX),
                                                     new FrameworkPropertyMetadata(typeof(ZoomX)));
        }

        public ZoomX()
        {
            PreviewMouseDown += ZoomX_MouseDown;
            PreviewMouseUp += ZoomX_MouseUp;
            PreviewMouseMove += ZoomX_PreviewMouseMove;

        }

        private void ZoomX_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            switch (ModifierMode)
            {
                case ZoomViewModifierMode.None:
                    return;
                case ZoomViewModifierMode.Pan:
                    var translate = _startTranslate + (e.GetPosition(this) - _mouseDownPos);
                    TranslateX = translate.X;
                    TranslateY = translate.Y;
                    break;
                case ZoomViewModifierMode.ZoomIn:
                    break;
                case ZoomViewModifierMode.ZoomOut:
                    break;
                case ZoomViewModifierMode.ZoomBox:
                    /*var pos = e.GetPosition(this);
                    var x = Math.Min(_mouseDownPos.X, pos.X);
                    var y = Math.Min(_mouseDownPos.Y, pos.Y);
                    var sizeX = Math.Abs(_mouseDownPos.X - pos.X);
                    var sizeY = Math.Abs(_mouseDownPos.Y - pos.Y);
                    ZoomBox = new Rect(x, y, sizeX, sizeY);*/
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void ZoomX_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (ModifierMode)
            {
                case ZoomViewModifierMode.None:
                    return;
                case ZoomViewModifierMode.Pan:
                    break;
                case ZoomViewModifierMode.ZoomIn:
                    break;
                case ZoomViewModifierMode.ZoomOut:
                    break;
                case ZoomViewModifierMode.ZoomBox:
                    //ZoomTo(ZoomBox);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ModifierMode = ZoomViewModifierMode.None;
            ReleaseMouseCapture();
        }

        void ZoomX_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (Keyboard.Modifiers)
            {
                case ModifierKeys.Shift:
//                case ModifierKeys.None:
                    ModifierMode = ZoomViewModifierMode.Pan;
                    break;
                case ModifierKeys.Alt:
                    ModifierMode = ZoomViewModifierMode.ZoomBox;
                    break;
                case ModifierKeys.Control:
                    break;
                case ModifierKeys.Windows:
                    break;
                default:
                    return;
            }
            if (ModifierMode == ZoomViewModifierMode.None)
                return;

            _mouseDownPos = e.GetPosition(this);
            _startTranslate = new Vector(TranslateX, TranslateY);
            Mouse.Capture(this);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //get the presenter, and initialize
            _presenter = GetTemplateChild(PART_Presenter) as ContentPresenter;
            if (_presenter != null)
            {
                _presenter.SizeChanged += (s, a) =>
                {
                    //zoom?
                };
               /* _presenter.ContentSizeChanged += (s, a) =>
                {
                    //zoom?
                };*/

                var _rt = new TransformGroup();
                _scaleTransform = new ScaleTransform();
                _translateTransform = new TranslateTransform();
                _rt.Children.Add(_scaleTransform);
                _rt.Children.Add(_translateTransform);
                _presenter.RenderTransform = _rt;
                _presenter.RenderTransformOrigin = new Point(0.5, 0.5);
            }
            
            ZoomToFill();
        }

        private double tX;
        public double TranslateX { get { return tX; } set { tX = value; _translateTransform.X = tX; } }
        private double tY;
        public double TranslateY { get { return tY; } set { tY = value; _translateTransform.Y = tY; } }
    }
}
