using System;

namespace GraphX.DesignerExampleData
{
    internal sealed class EdgeDataExample<Vertex> : EdgeBase<Vertex>
    {
        public EdgeDataExample(Vertex source, Vertex target)
            : base(source, target)
        {
            
        }

        public string Text { get; set; }
    }
}
