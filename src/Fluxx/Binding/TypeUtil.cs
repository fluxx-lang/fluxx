/**
 * @author Bret Johnson
 * @since 4/15/2015
 */

using System.Collections.Generic;

namespace Faml.Binding
{
    public class TypeUtil
    {
        // TODO: Fix this up, to look for common subtypes, but for now just use first type as the common one, as a demo hack
        public static TypeBinding FindCommonType(IEnumerable<TypeBinding> types)
        {
            TypeBinding commonType = null;

            foreach (TypeBinding type in types)
            {
                if (commonType == null)
                    commonType = type;
                else
                {
                    return type;
                    /*
                    if (commonType.IsAssignableFrom(type))
                        continue;
                    else if (type.IsAssignableFrom(commonType))
                        commonType = type;
                    else return null;
                    */
                }
            }

            return commonType;
        }
    }
}
