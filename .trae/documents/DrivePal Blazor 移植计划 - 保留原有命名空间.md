# DrivePal Blazor 移植计划

## 目标
将DrivePal的WPF+Blazor Hybrid实现移植到Photino+Blazor Hybrid，并保留原有命名空间，将项目重命名为DrivePalApp。

## 实施步骤

### 1. 项目准备
- [x] 分析DrivePal项目结构和Blazor组件依赖
- [ ] 将XfceBlazorApp重命名为DrivePalApp
- [ ] 创建新的Photino+Blazor Hybrid项目

### 2. 组件移植
- [ ] 复制DrivePal.UI的Blazor组件到新项目，保留原有命名空间
- [ ] 复制wwwroot资源文件
- [ ] 复制必要的服务和工具类

### 3. 项目配置
- [ ] 添加Photino.Blazor NuGet包
- [ ] 添加DrivePal.Core和DrivePal.Infrastructure项目引用
- [ ] 配置项目为Photino Blazor应用
- [ ] 确保原有命名空间和引用正确

### 4. 代码修改
- [ ] 更新Program.cs，使用PhotinoBlazorAppBuilder
- [ ] 确保所有组件使用正确的命名空间
- [ ] 处理任何平台特定的代码差异

### 5. 测试和验证
- [ ] 构建项目，确保无编译错误
- [ ] 运行应用，验证基本功能
- [ ] 测试各个Blazor页面和组件

## 命名空间策略
- 保留原有`DrivePal.UI`命名空间
- 新的Photino宿主应用使用`DrivePalApp`命名空间
- 确保组件引用和依赖关系正确

## 预期结果
- 成功将DrivePal的Blazor实现移植到Photino+Blazor Hybrid
- 保留所有原有功能和命名空间
- 项目重命名为DrivePalApp
- 可以在Ubuntu XFCE上运行

## 技术栈
- .NET 8
- Photino.Blazor 4.0.13
- 原有DrivePal.Core和DrivePal.Infrastructure组件
- Blazor WebAssembly

这个计划将确保移植过程中保留原有命名空间，同时成功将应用迁移到Photino平台。