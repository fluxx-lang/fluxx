/**
 * @author Bret Johnson
 * @since 4/15/2015
 */

using System;

namespace Faml.Binding
{
    [Flags]
    public enum TypeFlags
    {
        None = 0x0,
        IsReactive = 0x1
    }
}
