using System;
using System.Reflection;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Hikariii.Features.ListenerLoader.Utils;

namespace osu.Game.Rulesets.Hikariii.Features.ListenerLoader;

public abstract partial class AbstractInjector : CompositeDrawable
{
    protected static readonly BindingFlags InstanceOrStaticFlag = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;

    /// <summary>
    /// 在给定的对象中寻找第一个符合<para>type</para>的非公开字段
    /// </summary>
    /// <param name="type">目标类型</param>
    /// <param name="obj">目标对象</param>
    /// <returns></returns>
    protected FieldInfo? FindFieldInstance(object obj, Type type)
    {
        return obj.FindFieldInstance(type);
    }
}
