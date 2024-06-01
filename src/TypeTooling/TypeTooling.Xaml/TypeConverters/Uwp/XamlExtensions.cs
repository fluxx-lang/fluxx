// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
#if false
namespace Microsoft.VisualStudio.DesignTools.UwpDesigner
{
    extern alias WindowsRuntime;

    using Xaml = WindowsRuntime::Windows.UI.Xaml;
    using Foundation = WindowsRuntime::Windows.Foundation;
    using XamlMatrix = WindowsRuntime::Windows.UI.Xaml.Media.Matrix;
    using XamlGridLength = WindowsRuntime::Windows.UI.Xaml.GridLength;
    using XamlGridUnitType = WindowsRuntime::Windows.UI.Xaml.GridUnitType;
    using XamlThickness = WindowsRuntime::Windows.UI.Xaml.Thickness;
    using XamlCornerRadius = WindowsRuntime::Windows.UI.Xaml.CornerRadius;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.DesignTools.Designer.Utility;

    internal static class XamlExtensions
    {
        /// <summary>
        /// Extension method that is compatible with both List.Count and IVector.Size
        /// </summary>
        public static int Count<T>(this IList<T> list)
        {
            return list.Count;
        }

        /// <summary>
        /// Extension method that is compatible with both List[int] and IVector.GetAt(uint)
        /// </summary>
        public static T GetItem<T>(this IList<T> list, int index)
        {
            return list[index];
        }

        public static void Add<T>(this IList<T> list, T item)
        {
            list.Add(item);
        }

        public static int IndexOf<T>(this IList<T> list, T item)
        {
            return list.IndexOf(item);
        }

        public static void InsertAt<T>(this IList<T> list, int i, T item)
        {
            list.Insert(i, item);
        }

        public static void RemoveAt<T>(this IList<T> list, int i)
        {
            list.RemoveAt(i);
        }

        public static void Clear<T>(this IList<T> list)
        {
            list.Clear();
        }

        /// <summary>
        /// Extension method that can adapt IVector to ICollection.
        /// </summary>
        public static ICollection AsCollection(this object instance, Type instanceType)
        {
            Type itemType = instanceType.GetVectorItemType();
            if (itemType != null)
            {
                Type adaptorType = typeof(GenericListToListAdapter<>).MakeGenericType(itemType);
                if (adaptorType != null)
                {
                    return Activator.CreateInstance(adaptorType, instance) as ICollection;
                }
            }
            return null;	
        }

        public static Type GetVectorItemType(this Type type)
        {
            if (type != null)
            {
                foreach (Type interfaceType in type.GetInterfaces())
                {
                    if (interfaceType.IsIListType())
                    {
                        Type itemType = interfaceType.GetGenericArguments()[0];
                        if (interfaceType == typeof(System.Collections.Generic.IList<>).MakeGenericType(itemType))
                        {
                            return itemType;
                        }
                    }
                }
            }
            return null;
        }

        internal static XamlMatrix CreateMatrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            return new XamlMatrix()
            {
                M11 = m11,
                M12 = m12,
                M21 = m21,
                M22 = m22,
                OffsetX = offsetX,
                OffsetY = offsetY,
            };
        }

        internal static XamlThickness CreateThickness(double left, double top, double right, double bottom)
        {
            return new XamlThickness()
            {
                Left = left,
                Top = top,
                Right = right,
                Bottom = bottom,
            };
        }

        internal static XamlGridLength CreateGridLength(double value, XamlGridUnitType unitType)
        {
            return new XamlGridLength(value, unitType);
        }

        internal static object CreateCornerRadius(double topLeft, double topRight, double bottomRight, double bottomLeft)
        {
            return new XamlCornerRadius()
            {
                TopLeft = topLeft,
                TopRight = topRight,
                BottomRight = bottomRight,
                BottomLeft = bottomLeft,
            };
        }

        internal static Foundation.Point Transform(this Xaml.Media.GeneralTransform transform, Foundation.Point point)
        {
            return transform.TransformPoint(point);
        }
    }
}
#endif