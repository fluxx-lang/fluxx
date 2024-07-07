using System;
using System.Collections.Generic;

namespace Faml
{
    public class DotNetDataEventHandler : DataEventHandler
    {
        private readonly DataEventHandlerDelegate eventHandlerDelegate;

        public delegate void DataEventHandlerDelegate(object sender, EventArgs e, IList<object> data);

        public DotNetDataEventHandler(DataEventHandlerDelegate eventHandlerDelegate)
        {
            this.eventHandlerDelegate = eventHandlerDelegate;
        }

        public DataEventHandlerDelegate EventHandlerDelegate => this.eventHandlerDelegate;
    }
}