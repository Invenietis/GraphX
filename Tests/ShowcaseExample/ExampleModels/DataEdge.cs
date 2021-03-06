﻿using GraphX.GraphSharp;
using GraphX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using YAXLib;

namespace ShowcaseExample
{
    [Serializable]
    public class DataEdge : EdgeBase<DataVertex>
    {
        public DataEdge(DataVertex source, DataVertex target)
			: base(source, target)
		{
		}

        public DataEdge()
            : base(null, null )
        {
        }

        /// <summary>
        /// Node main description (header)
        /// </summary>
        public string Text { get; set; }
        public string ToolTipText {get; set; }

        public override string ToString()
        {
            return Text;
        }

        [YAXDontSerialize]
        public DataEdge Self
        {
            get { return this; }
        }

    }
}
