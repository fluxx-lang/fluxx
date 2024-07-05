using System.Collections.Generic;

namespace Faml.Lexer {
    public sealed class DelimiterStack {
        private Stack<char> _stack = new Stack<char>();

        public void Push(char delimiter) {
            _stack.Push(delimiter);
        }

        public void PopUntil(char delimiter) {
            while (_stack.Count > 0) {
                char popped = _stack.Pop();
                if (popped == delimiter)
                    return;
            }
        }

        public bool IsEmpty => _stack.Count == 0;
    }
}
