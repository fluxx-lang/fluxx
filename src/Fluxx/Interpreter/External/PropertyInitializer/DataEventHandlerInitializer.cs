namespace Faml.Interpreter.External.PropertyInitializer
{
#if false
    public class DataEventHandlerInitializer : PropertyInitializer {
        private readonly ListEval _listEval;
        private readonly EventInfo _eventInfo;
        private readonly DotNetDataEventHandler.DataEventHandlerDelegate _dataEventHandlerDelegate;

        public DataEventHandlerInitializer(ListEval listEval, EventInfo eventInfo, DotNetDataEventHandler dotNetDataEventHandler) {
            _listEval = listEval;
            _eventInfo = eventInfo;
            _dataEventHandlerDelegate = dotNetDataEventHandler.EventHandlerDelegate;
        }

        public void eventHandler(object sender, EventArgs e) {
            var data = new  List<object>();
            _listEval.EvalAndAdd(data);

            _dataEventHandlerDelegate(sender, e, data);
        }

        public override void Initialize(object obj) {
            _eventInfo.AddEventHandler(obj, new EventHandler(eventHandler));
        }
    }
#endif
}
