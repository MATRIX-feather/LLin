# Hikariii
向osu!添加播放器以及Sayobot加速下载功能

> [!CAUTION]
>
> 注意！
> 根据 [Cai1Hsu/osu-plugins](https://github.com/Cai1Hsu/osu-plugins/issues/93) 以及 [ppy/osu](https://github.com/ppy/osu/issues/37540)，连接到 osu! 官方服务器时使用诸如 LLin 这类 ruleset 插件可能会导致你的账号被**封禁**。

## 安装方法
先将你在[Release](https://github.com/MATRIX-feather/LLin/releases)或[Actions](https://github.com/MATRIX-feather/LLin/actions/new)下载到的文件解压，然后：
1. 在游戏里点击`设置 ~> 打开 osu! 文件夹`
2. [将压缩包中的文件和文件夹安装到你的osu!游戏模式目录下](https://bbs.hiosu.com/thread-5-1-1.html)
4. 重启osu!
5. 完成！

## 有关构建

### 通用版本（无 Windows 集成）
若要构建通用版本：
```
dotnet publish BuildHikariii -c Release
```

需要从 publish 中提取的文件在 [package.sh](./package.sh) 的 `function main()` 中有提到。

### Windows
若要构建带有 Windows 支持的 Hikariii：
```
dotnet publish BuildHikariii.Windows -c Release
```

需要从 publish 中提取的文件在 [package.sh](./package.sh) 的 `function main()` 中有提到。

### Android
如果你遇到了类似以下内容的 Android 版本的构建问题，请试着使用 .NET 8.0.423 SDK：
```
1.
---
/LLin/BuildHikariii.Android/BuildHikariii.Android.csproj : error NU1101: 找不到包 Microsoft.NETCore.App.Runtime.linux-bionic-x86。源 /usr/lib64/dotnet/library-packs, nuget.org 中不存在具有此 ID 的包
/LLin/BuildHikariii.Android/BuildHikariii.Android.csproj : error NU1101: 找不到包 Microsoft.NETCore.App.Runtime.linux-bionic-arm。源 /usr/lib64/dotnet/library-packs, nuget.org 中不存在具有此 ID 的包
---

2.
---
/LLin/LLin.OSIntegrations/Linux/DBus/Services/FreedesktopSettingsAccessor.cs(10,14): error CS0246: 未能找到类型或命名空间名“OrgFreedesktopPortalSettingsProxy”(是否缺少 using 指令或程序集引用?) [/home/neko/repo/LLin/LLin.OSIntegrations/LLin.OSIntegrations.csproj::TargetFramework=net8.0]
---

3.
---
/LLin/LLin.OSIntegrations/Linux/DBus/Services/Mpris/MprisService.cs(81,13): error CS0246: 未能找到类型或命名空间名“PathHandler”(是否缺少 using 指令或程序集引用?) [/home/neko/repo/LLin/LLin.OSIntegrations/LLin.OSIntegrations.csproj::TargetFramework=net8.0]
---
```

你可以将 global-android.json 重命名为 global.json 来强制使用 8.0.423。

需要从 publish 中提取的文件在 [package.sh](./package.sh) 的 `function main()` 中有提到。

## 食用指南
### 下载加速
点击任意未下载谱面的预览按钮（"`▶`"），待预览加载完毕后将会在左上角自动显示橙色的下载加速的选项。
下载按钮将在预览结束或下载完成后自动隐藏，你也可以通过点击左上角的按钮开手动关闭此弹窗。

### 播放器
播放器可以通过主界面和单人游戏选歌进入，按键可以在`输入 ~> 快捷键和键位绑定`中设置。

## 第三方开源许可
- SandboxToPanel插件：[EVAST9919/lazer-sandbox](https://github.com/EVAST9919/lazer-sandbox) --> [LICENSE](./osu.Game.Rulesets.Hikariii/Features/Player/Plugins/Bundle/SandboxToPanel/LICENSE)
- `osu.Game.Rulesets.Hikariii/ppyStuffs`下的组件：[ppy/osu](https://github.com/ppy/osu) --> [LICENCE](./osu.Game.Rulesets.Hikariii/ppyStuffs/LICENCE)