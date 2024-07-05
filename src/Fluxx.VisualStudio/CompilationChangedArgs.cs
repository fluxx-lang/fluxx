using System;

namespace Faml.VisualStudio {
    public class CompilationChangedArgs : EventArgs {
        public bool IsEntireProjectAffected { get; }

        public CompilationChangedArgs(bool isEntireProjectAffected) {
            IsEntireProjectAffected = isEntireProjectAffected;
        }
    }
}
