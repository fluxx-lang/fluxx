using System.Collections.Generic;

namespace Fluxx.Lexer
{
    public sealed class DelimiterStack
    {
        private Stack<char> stack = new Stack<char>();

        public void Push(char delimiter)
        {
            this.stack.Push(delimiter);
        }

        public void PopUntil(char delimiter)
        {
            while (this.stack.Count > 0)
            {
                char popped = this.stack.Pop();
                if (popped == delimiter)
                {
                    return;
                }
            }
        }

        public bool IsEmpty => this.stack.Count == 0;
    }
}
