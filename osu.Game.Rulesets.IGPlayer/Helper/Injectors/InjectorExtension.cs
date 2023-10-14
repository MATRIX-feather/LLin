using System;
using System.Linq;
using System.Reflection;

namespace osu.Game.Rulesets.IGPlayer.Helper.Injectors;

public static class InjectorExtension
{
    private static readonly BindingFlags instanceFlag = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.GetField;

    private static FieldInfo? FindFieldInstanceInBaseType(Type baseType, Type type)
    {
        var field = baseType.GetFields(instanceFlag)
                            .FirstOrDefault(f => f.FieldType == type);

        if (field == null && baseType.BaseType != null)
            field = FindFieldInstanceInBaseType(baseType.BaseType, type);

        return field;
    }

    public static FieldInfo? FindFieldInstance(this object obj, Type type)
    {
        var field = obj.GetType().GetFields(instanceFlag)
                       .FirstOrDefault(f => f.FieldType == type);

        var baseType = obj.GetType().BaseType;
        if (baseType != null && field == null)
            field = FindFieldInstanceInBaseType(baseType, type);

        return field;
    }

    public static object? FindInstance(this object obj, Type type)
    {
        var field = obj.FindFieldInstance(type);
        return field == null ? null : field.GetValue(obj);
    }
}
