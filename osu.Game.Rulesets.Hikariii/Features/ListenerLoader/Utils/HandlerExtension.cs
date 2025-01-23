using System;
using System.Linq;
using System.Reflection;

namespace osu.Game.Rulesets.Hikariii.Features.ListenerLoader.Utils;

public static class HandlerExtension
{
    public const BindingFlags INSTANCE_FLAG = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.GetField;

    private static FieldInfo? findFieldInstanceInBaseType(Type baseType, Type type)
    {
        var field = baseType.GetFields(INSTANCE_FLAG)
                            .FirstOrDefault(f => f.FieldType == type);

        if (field == null && baseType.BaseType != null)
            field = findFieldInstanceInBaseType(baseType.BaseType, type);

        return field;
    }

    public static FieldInfo? FindFieldInstance(this object obj, Type type)
    {
        var field = obj.GetType().GetFields(INSTANCE_FLAG)
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
