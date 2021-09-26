# LLin
从 [mfosu](https://github.com/MATRIX-feather/osu) 中分离出来的osu!lazer播放器

## 构建
构建需要的工具：
* Git : https://git-scm.com/
* .NET 5 SDK : https://docs.microsoft.com/zh-cn/dotnet/core/install/

脚本：
```Bash
#!/bin/bash

# 新建目录
mkdir llin && cd llin

# 克隆依赖
git clone https://github.com/MATRIX-feather/osu
git clone https://github.com/MATRIX-feather/osu-framework

# 克隆repo
git clone https://github.com/MATRIX-feather/LLin

# 检出
cd osu
git checkout custom
cd ..

# 构建
cd LLin
dotnet build LLin.Desktop -c Release
```

构建完成后前往`LLin.Desktop/bin/Release/net5.0`中启动`LLin`即可

## 使用
目前LLin尚不能自动初始化数据库，所以您需要手动为其提供。

### 新建数据库
1. 前往lazer的默认文件夹(Linux: `~/.local/share/osu`, Windows: `%APPDATA%\osu`)，找到`storage.ini`(如果没有则创建一个)
2. 关闭lazer
3. 将`storage.ini`中的`FullPath`设置为LLin默认文件夹的绝对路径(Linux在主目录的`.local/share/LLin`下, Windows则在`%APPDATA%`的LLin下)
4. 启动lazer，此时他会提示你osu!存储错误
![2021-09-27 03-23-43 的屏幕截图](https://user-images.githubusercontent.com/55654482/134821392-c2c4c375-652e-498e-bdf4-18cd672a93b4.png)
5. 选择第一项 `Start fresh at specified location`
6. 正常进行设置、下图
7. 关闭lazer，启动LLin
8. 完成

### 使用现有的数据库
**注：本项目当前还处于早期阶段，使用现有的数据库前请备份好当前数据库以避免意外发生**

将LLin默认文件夹链接到你的lazer文件夹即可

## 协议
[MIT](LICENSE)

部分存在外部代码引用的组件中，代码引用的部分可能存在其自己的协议，引用部分的协议请参照其根目录下的 <项目名>-<LICENSE 或 LICENCE> 文件
