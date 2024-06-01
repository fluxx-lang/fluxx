using System.Collections;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;

namespace TypeTooling.DotNet.Types {
    public sealed class CollectionPropertyInitializer : PropertyInitializer {
        private readonly DotNetRawProperty _rawProperty;
        private readonly DotNetRawMethod _addMethod;

        public CollectionPropertyInitializer(DotNetRawProperty rawProperty, DotNetRawType elementRawType) {
            _rawProperty = rawProperty;

            DotNetRawType propertyRawType = rawProperty.PropertyType;

            // TODO: Remove the need for this hack
            _addMethod = propertyRawType.GetRequiredInstanceMethod("Add", new[] { elementRawType });
            if (_addMethod == null) {
                // {System.Collections.Generic.ICollection`1[Windows.UI.Xaml.UIElement]}



            }
        }

        public override void Initialize(object obj, object propertyValue) {
            object collection = ((ReflectionDotNetRawProperty) _rawProperty).PropertyInfo.GetValue(obj);

            var enumerable = (IEnumerable) propertyValue;

            var args = new object[1];
            foreach (object value in enumerable) {
                if (value is IEnumerable valueEnumerable) {
                    foreach (var valueItem in valueEnumerable) {
                        args[0] = valueItem;
                        ((ReflectionDotNetRawMethod)_addMethod).MethodInfo.Invoke(collection, args);
                    }
                }
                else {
                    args[0] = value;
                    ((ReflectionDotNetRawMethod) _addMethod).MethodInfo.Invoke(collection, args);
                }
            }
        }
    }
}
