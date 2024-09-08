using System;
using System.Linq;
using System.Reflection;

namespace osu.Game.Rulesets.IGPlayer.Helper.Handler;

public static class HandlerExtension
{
    private const BindingFlags instance_flag = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.GetField;

    private static FieldInfo? findFieldInstanceInBaseType(Type baseType, Type type)
    {
        var field = baseType.GetFields(instance_flag)
                            .FirstOrDefault(f => f.FieldType == type);

        if (field == null && baseType.BaseType != null)
            field = findFieldInstanceInBaseType(baseType.BaseType, type);

        return field;
    }

    public static FieldInfo? FindFieldInstance(this object obj, Type type)
    {
        var field = obj.GetType().GetFields(instance_flag)
                       .FirstOrDefault(f => f.FieldType == type);

        var baseType = obj.GetType().BaseType;
        if (baseType != null && field == null)
            field = findFieldInstanceInBaseType(baseType, type);

        return field;
    }

    public static object? FindInstance(this object obj, Type type)
    {
        var field = obj.FindFieldInstance(type);
        return field == null ? null : field.GetValue(obj);
    }
}
