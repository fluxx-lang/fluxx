using System;

namespace Fluxx.VisualStudio {
    public class CompilationChangedArgs : EventArgs {
        public bool IsEntireProjectAffected { get; }

        public CompilationChangedArgs(bool isEntireProjectAffected) {
            IsEntireProjectAffected = isEntireProjectAffected;
        }
    }
}
