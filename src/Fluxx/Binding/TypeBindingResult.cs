using System;

namespace Faml.Binding {
    public abstract class TypeBindingResult {
        public string GetNotFoundOrOtherErrorMessage(string notFoundErrorMessage) {
            switch (this)
            {
                case NotFound _:
                    return notFoundErrorMessage;
                case Error error:
                    return error.Message;
                default:
                    throw new Exception($"Unexpected TypeBindingResult type: {this.GetType().FullName}");
            }
        }

        public sealed class Success : TypeBindingResult {
            private readonly TypeBinding _typeBinding;

            public Success(TypeBinding typeBinding) {
                _typeBinding = typeBinding;
            }

            public TypeBinding TypeBinding => _typeBinding;
        }

        public sealed class NotFound : TypeBindingResult {
        }

        private static readonly NotFound _notFoundResult = new NotFound();
        public static NotFound NotFoundResult => _notFoundResult;

        public sealed class Error : TypeBindingResult {
            private readonly string _message;

            public Error(string message) {
                _message = message;
            }

            public string Message => _message;
        }
    }
}
