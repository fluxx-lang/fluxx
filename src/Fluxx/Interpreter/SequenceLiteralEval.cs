/**
 * @author Bret Johnson
 * @since 4/4/2015
 */

using System.Collections.Generic;

namespace Faml.Interpreter
{
    public class SequenceLiteralEval : ObjectEval
    {
        private readonly ObjectEval[] _items;

        public SequenceLiteralEval(ObjectEval[] items)
        {
            this._items = items;
        }

        public override object Eval()
        {
            // TODO: Make this more efficient; maybe just go back to GenerateSequence
            List<object> list = new List<object>();
            int listItemsLength = this._items.Length;
            for (int i = 0; i < listItemsLength; i++)
            {
                list.Add( this._items[i].Eval() );
            }

            return list;
        }

        public IEnumerable<object> GenerateSequence()
        {
            int listItemsLength = this._items.Length;
            for (int i = 0; i < listItemsLength; i++)
            {
                yield return this._items[i].Eval();
            }
        }
    }
}
