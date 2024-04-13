using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZymLabs.NSwag.FluentValidation
{
    /// <summary>
    /// Reflection extensions
    /// </summary>
    public static class ReflectionExtension
    {
        public static bool IsSubClassOfGeneric(this Type child, Type parent)
        {
            if (child == null)
                return false;

            // Check for exact type match, return false for non-generic or if the parent is not a generic definition
            if (child == parent)
                return false;

            // Handling generic definitions
            if (parent.IsGenericTypeDefinition)
            {
                if (child.IsGenericType && child.GetGenericTypeDefinition() == parent)
                    return true;

                // Check all base types and interfaces for matching generic definition
                foreach (var baseType in child.GetBaseTypesAndInterfaces())
                {
                    if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == parent)
                        return true;
                }
            }
            else
            {
                // Handling specific closed generics or direct implementations
                if (parent.IsGenericType && child.IsGenericType)
                {
                    foreach (var baseType in child.GetBaseTypesAndInterfaces())
                    {
                        if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == parent.GetGenericTypeDefinition())
                        {
                            return GenericArgumentsMatch(baseType.GetGenericArguments(), parent.GetGenericArguments());
                        }
                    }
                }
            }

            // Fallback to handle non-generic subclassing or interface implementations
            return child.BaseType != null && child.BaseType.IsSubClassOfGeneric(parent) || parent.IsAssignableFrom(child);
        }

        private static bool GenericArgumentsMatch(Type[] childArgs, Type[] parentArgs)
        {
            if (childArgs.Length != parentArgs.Length)
                return false;

            for (int i = 0; i < childArgs.Length; i++)
            {
                if (childArgs[i] != parentArgs[i] && !childArgs[i].IsSubclassOf(parentArgs[i]) && !childArgs[i].IsAssignableFrom(parentArgs[i]))
                    return false;
            }
            return true;
        }

        public static IEnumerable<Type> GetBaseTypesAndInterfaces(this Type type)
        {
            if (type.BaseType != null)
            {
                yield return type.BaseType;
                foreach (var bt in type.BaseType.GetBaseTypesAndInterfaces())
                    yield return bt;
            }
            foreach (var iface in type.GetInterfaces())
                yield return iface;
        }


    }
    /// <summary>
    /// Is sub class of generic type
    /// </summary>
    /// <param name="child"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    //    public static bool IsSubClassOfGeneric(this Type child, Type parent)
    //    {
    //        if (child == null || child == typeof(object))
    //            return false;

    //        // If checking against itself and not an open generic interface, return false.
    //        if (child == parent && !parent.IsGenericTypeDefinition)
    //            return false;

    //        // Handle open generic definitions and closed generics differently.
    //        if (parent.IsGenericTypeDefinition)
    //        {
    //            foreach (var current in child.GetBaseTypesAndInterfaces())
    //            {
    //                if (current.IsGenericType && current.GetGenericTypeDefinition() == parent)
    //                    return true;
    //            }
    //        }
    //        else
    //        {
    //            // Check if it matches a specific closed generic type (including interface implementations).
    //            if (parent.IsGenericType && !parent.IsGenericTypeDefinition)
    //            {
    //                foreach (var current in child.GetBaseTypesAndInterfaces())
    //                {
    //                    if (current == parent || (current.IsGenericType && current.GetGenericTypeDefinition() == parent.GetGenericTypeDefinition() && GenericArgumentsMatch(current.GetGenericArguments(), parent.GetGenericArguments())))
    //                        return true;
    //                }
    //            }

    //            // Direct inheritance or interface implementation check for non-generic types.
    //            if (!parent.IsGenericType && (child.IsSubclassOf(parent) || parent.IsAssignableFrom(child)))
    //                return true;
    //        }

    //        return false;
    //    }

    //    private static bool GenericArgumentsMatch(Type[] childArgs, Type[] parentArgs)
    //    {
    //        if (childArgs.Length != parentArgs.Length)
    //            return false;

    //        for (int i = 0; i < childArgs.Length; i++)
    //        {
    //            if (childArgs[i] != parentArgs[i] && !childArgs[i].IsSubclassOf(parentArgs[i]))
    //                return false;
    //        }

    //        return true;
    //    }

    //    public static IEnumerable<Type> GetBaseTypesAndInterfaces(this Type type)
    //    {
    //        if (type.BaseType != null)
    //        {
    //            yield return type.BaseType;
    //            foreach (var bt in type.BaseType.GetBaseTypesAndInterfaces())
    //                yield return bt;
    //        }
    //        foreach (var iface in type.GetInterfaces())
    //            yield return iface;
    //    }


    //    private static Type GetFullTypeDefinition(Type type)
    //    {
    //        return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
    //    }

    //    private static bool VerifyGenericArguments(Type parent, Type child)
    //    {
    //        Type[] childArguments = child.GetGenericArguments();
    //        Type[] parentArguments = parent.GetGenericArguments();

    //        if (childArguments.Length != parentArguments.Length)
    //        {
    //            return true;
    //        }

    //        return !childArguments.Where((t, i) =>
    //                (t.Assembly != parentArguments[i].Assembly || t.Name != parentArguments[i].Name ||
    //                 t.Namespace != parentArguments[i].Namespace) && !t.IsSubclassOf(parentArguments[i]))
    //            .Any();
    //    }
    //}
}