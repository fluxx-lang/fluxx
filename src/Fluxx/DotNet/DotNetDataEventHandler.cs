using System;
using System.Collections.Generic;

namespace Faml {
    public class DotNetDataEventHandler : DataEventHandler {
        private readonly DataEventHandlerDelegate _eventHandlerDelegate;

        public delegate void DataEventHandlerDelegate(object sender, EventArgs e, IList<object> data);

        public DotNetDataEventHandler(DataEventHandlerDelegate eventHandlerDelegate) {
            _eventHandlerDelegate = eventHandlerDelegate;
        }

        public DataEventHandlerDelegate EventHandlerDelegate => _eventHandlerDelegate;
    }
}