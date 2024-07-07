namespace Faml
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ThisPropertyAttribute(string name) : Attribute
    {
        /// <summary>Gets the name of the this property</summary>
        /// <value>A string representing the name of the this property.</value>
        /// <remarks>
        /// For example, if function Foo has this property Bar and
        /// regular property Baz, then Foo can be invoked with the normal function syntax, Foo{Bar:value1 Baz:value2}, or with 
        /// object syntax value1.Foo{Baz:value2}.  Object syntax is useful for chaining function calls together, in a pipleline.
        /// Normally the "main" input to a function should be flagged as the object property, especialy if chaing is useful for
        /// the function.
        /// </remarks>
        public string Name { get; private set; } = name;
    }
}
