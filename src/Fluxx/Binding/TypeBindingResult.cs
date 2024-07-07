using System;

namespace Fluxx.Binding
{
    public abstract class TypeBindingResult
    {
        public string GetNotFoundOrOtherErrorMessage(string notFoundErrorMessage)
        {
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

        public sealed class Success : TypeBindingResult
        {
            private readonly TypeBinding typeBinding;

            public Success(TypeBinding typeBinding)
            {
                this.typeBinding = typeBinding;
            }

            public TypeBinding TypeBinding => this.typeBinding;
        }

        public sealed class NotFound : TypeBindingResult
        {
        }

        private static readonly NotFound notFoundResult = new NotFound();
        public static NotFound NotFoundResult => notFoundResult;

        public sealed class Error : TypeBindingResult
        {
            private readonly string message;

            public Error(string message)
            {
                this.message = message;
            }

            public string Message => this.message;
        }
    }
}
