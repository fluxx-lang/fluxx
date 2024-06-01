// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace TypeTooling.DotNet.RawTypes.Roslyn.DocumentationCommentParser {
    /// <summary>
    ///  NOTE: This was copied from Roslyn https://github.com/dotnet/roslyn/tree/5b54220e40b51d5faf4f620984940f83c793f3be
    /// 
    /// Shared object pool for roslyn
    /// 
    /// Use this shared pool if only concern is reducing object allocations.
    /// if perf of an object pool itself is also a concern, use ObjectPool directly.
    /// 
    /// For example, if you want to create a million of small objects within a second, 
    /// use the ObjectPool directly. it should have much less overhead than using this.
    /// </summary>
    internal static class SharedPools {
        /// <summary>
        /// pool that uses default constructor with 20 elements pooled
        /// </summary>
        public static ObjectPool<T> Default<T>() where T : class, new() {
            return DefaultNormalPool<T>.Instance;
        }

        private static class DefaultNormalPool<T> where T : class, new() {
            public static readonly ObjectPool<T> Instance = new ObjectPool<T>(() => new T(), 20);
        }
    }
}
