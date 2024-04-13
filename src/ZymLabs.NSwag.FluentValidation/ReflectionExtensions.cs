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
        /// <summary>
        /// Is sub class of generic type
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static bool IsSubClassOfGenericOld(this Type child, Type parent)
        {
            if (child == parent)
            {
                return false;
            }

            if (child.IsSubclassOf(parent))
            {
                return true;
            }

            var parameters = parent.GetGenericArguments();

            var isParameterLessGeneric = !(parameters.Length > 0 &&
                                           ((parameters[0].Attributes & TypeAttributes.BeforeFieldInit) ==
                                            TypeAttributes.BeforeFieldInit));

            while (child != typeof(object))
            {
                var cur = GetFullTypeDefinition(child);

                if (parent == cur || (isParameterLessGeneric && cur.GetInterfaces()
                        .Select(GetFullTypeDefinition)
                        .Contains(GetFullTypeDefinition(parent))))
                {
                    return true;
                }

                if (!isParameterLessGeneric)
                {
                    if (GetFullTypeDefinition(parent) == cur && !cur.IsInterface)
                    {
                        if (VerifyGenericArguments(GetFullTypeDefinition(parent), cur) &&
                            VerifyGenericArguments(parent, child))
                        {
                            return true;
                        }
                    }
                    else if (child.GetInterfaces()
                             .Where(i => GetFullTypeDefinition(parent) == GetFullTypeDefinition(i))
                             .Any(item => VerifyGenericArguments(parent, item)))
                    {
                        return true;
                    }
                }

                child = child.BaseType!;
            }

            return false;
        }

        public static bool IsSubClassOfGeneric(this Type child, Type parent)
        {
            if (child == null || child == typeof(object))
                return false;

            // If checking against itself and not an open generic interface, return false.
            if (child == parent && !parent.IsGenericTypeDefinition)
                return false;

            // Handle open generic definitions and closed generics differently.
            if (parent.IsGenericTypeDefinition)
            {
                foreach (var current in child.GetBaseTypesAndInterfaces())
                {
                    if (current.IsGenericType && current.GetGenericTypeDefinition() == parent)
                        return true;
                }
            }
            else
            {
                // Check if it matches a specific closed generic type (including interface implementations).
                if (parent.IsGenericType && !parent.IsGenericTypeDefinition)
                {
                    foreach (var current in child.GetBaseTypesAndInterfaces())
                    {
                        if (current == parent || (current.IsGenericType && current.GetGenericTypeDefinition() == parent.GetGenericTypeDefinition() && GenericArgumentsMatch(current.GetGenericArguments(), parent.GetGenericArguments())))
                            return true;
                    }
                }

                // Direct inheritance or interface implementation check for non-generic types.
                if (!parent.IsGenericType && (child.IsSubclassOf(parent) || parent.IsAssignableFrom(child)))
                    return true;
            }

            return false;
        }

        private static bool GenericArgumentsMatch(Type[] childArgs, Type[] parentArgs)
        {
            if (childArgs.Length != parentArgs.Length)
                return false;

            for (int i = 0; i < childArgs.Length; i++)
            {
                if (childArgs[i] != parentArgs[i] && !childArgs[i].IsSubclassOf(parentArgs[i]))
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
        public static bool IsSubClassOfGenericMostlyWorks(this Type child, Type parent)
        {
            if (child == parent)
            {
                return false;  // Explicitly handle the case where both types are the same
            }

            if (child == null)
            {
                return false;
            }

            // If the parent is a generic type definition, check all possibilities for subclassing or implementation
            if (parent.IsGenericTypeDefinition)
            {
                // Check through all base types
                for (Type? baseType = child; baseType != null && baseType != typeof(object); baseType = baseType.BaseType)
                {
                    if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == parent)
                    {
                        return true;
                    }
                }

                // Check through all implemented interfaces
                foreach (var implementedInterface in child.GetInterfaces())
                {
                    if (implementedInterface.IsGenericType && implementedInterface.GetGenericTypeDefinition() == parent)
                    {
                        return true;
                    }
                }
            }
            else
            {
                // If the parent is not a generic type definition, use IsSubclassOf and IsAssignableFrom
                if (parent.IsClass && child.IsSubclassOf(parent))
                {
                    return true;
                }

                if (parent.IsInterface && parent.IsAssignableFrom(child))
                {
                    return true;
                }
            }

            return false;
        }


        private static Type GetFullTypeDefinition(Type type)
        {
            return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
        }

        private static bool VerifyGenericArguments(Type parent, Type child)
        {
            Type[] childArguments = child.GetGenericArguments();
            Type[] parentArguments = parent.GetGenericArguments();

            if (childArguments.Length != parentArguments.Length)
            {
                return true;
            }

            return !childArguments.Where((t, i) =>
                    (t.Assembly != parentArguments[i].Assembly || t.Name != parentArguments[i].Name ||
                     t.Namespace != parentArguments[i].Namespace) && !t.IsSubclassOf(parentArguments[i]))
                .Any();
        }
    }
}