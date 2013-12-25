using System.ComponentModel;

namespace GraphX.GraphSharp.Algorithms.Layout
{
    public abstract class LayoutParametersBase : ILayoutParameters
    {
        public object Clone()
        {
            LayoutParametersBase p = (LayoutParametersBase)this.MemberwiseClone();
            p.PropertyChanged = null;
            return p;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged( string propertyName )
        {
            var h = PropertyChanged;
            if( h != null ) h( this, new PropertyChangedEventArgs( propertyName ) );
        }
    }
}