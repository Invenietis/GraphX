using GraphX;
using System;
using YAXLib;

namespace SimpleGraph
{
    /* DataVertex is the data class for the vertices. It contains all custom vertex data specified by the user.
     * This class also must be derived from VertexBase that provides properties and methods mandatory for
     * correct GraphX operations.
     * Some of the useful VertexBase members are:
     *  - ID property that stores unique positive identification number. Property must be filled by user.
     *  
     */

    public class DataVertex: VertexBase
    {
        /// <summary>
        /// Some string property for example purposes
        /// </summary>
        public string Text { get; set; }
 
        #region Calculated or static props
        [YAXDontSerialize]
        public DataVertex Self
        {
            get { return this; }
        }

        public override string ToString()
        {
            return Text;
        }

        private string[] imgArray = new string[4]
        {
            @"pack://application:,,,/CK.GraphX;component/Images/help_black.png",
            @"pack://application:,,,/ShowcaseExample;component/Images/skull_bw.png",
            @"pack://application:,,,/ShowcaseExample;component/Images/wrld.png",
            @"pack://application:,,,/ShowcaseExample;component/Images/birdy.png",
        };
        private string[] textArray = new string[4]
        {
            @"",
            @"Skully",
            @"Worldy",
            @"Birdy",
        };

        #endregion

        /// <summary>
        /// Default parameterless constructor for this class
        /// (required for YAXLib serialization)
        /// </summary>
        public DataVertex():this("")
        {
        }

        public DataVertex(string text = "")
        {
            Text = text;
        }
    }
}
