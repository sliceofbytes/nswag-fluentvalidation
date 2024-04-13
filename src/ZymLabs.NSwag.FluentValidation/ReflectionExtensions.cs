namespace ZymLabs.NSwag.FluentValidation;

/// <summary>
/// Reflection extensions.
/// </summary>
public static class ReflectionExtension
{
    /// <summary>
    /// Checks if a type is a subclass of a generic type or direct implementation.
    /// </summary>
    /// <param name="child">Child object Type.</param>
    /// <param name="parent">Parent object Type.</param>
    /// <returns>Returns true if it's a subclass of a generic.</returns>
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
        return child.BaseType?.IsSubClassOfGeneric(parent) == true || parent.IsAssignableFrom(child);
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

    /// <summary>
    /// Retrieves base types and interfaces given a type.
    /// </summary>
    /// <param name="type">Object Type.</param>
    /// <returns>IEnumerable of base types and interfaces.</returns>
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