using System.Collections.Generic;

namespace Fluxx.Interpreter
{
    public class SequenceLiteralEval : ObjectEval
    {
        private readonly ObjectEval[] items;

        public SequenceLiteralEval(ObjectEval[] items)
        {
            this.items = items;
        }

        public override object Eval()
        {
            // TODO: Make this more efficient; maybe just go back to GenerateSequence
            List<object> list = new List<object>();
            int listItemsLength = this.items.Length;
            for (int i = 0; i < listItemsLength; i++)
            {
                list.Add(this.items[i].Eval() );
            }

            return list;
        }

        public IEnumerable<object> GenerateSequence()
        {
            int listItemsLength = this.items.Length;
            for (int i = 0; i < listItemsLength; i++)
            {
                yield return this.items[i].Eval();
            }
        }
    }
}
