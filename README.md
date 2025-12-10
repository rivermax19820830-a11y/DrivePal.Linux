# Photino + Blazor Hybrid 桌面应用

这是一个使用 Photino + Blazor Hybrid 开发的桌面应用程序，专为 Ubuntu XFCE 桌面环境设计。

## 技术栈

- **框架**: .NET 8
- **UI技术**: Blazor WebAssembly
- **桌面容器**: Photino
- **目标平台**: Linux (Ubuntu XFCE)

## 项目结构

```
xfceblazor/
├── XfceBlazorApp/
│   ├── Pages/              # Blazor 页面组件
│   │   └── Index.razor     # 首页
│   ├── Shared/             # 共享组件
│   │   └── MainLayout.razor # 主布局
│   ├── wwwroot/            # Web 资源
│   │   ├── css/            # CSS 样式
│   │   │   └── app.css     # 主样式文件
│   │   └── index.html      # HTML 入口
│   ├── App.razor           # Blazor 应用入口
│   ├── Program.cs          # 应用主程序
│   └── XfceBlazorApp.csproj # 项目配置
└── README.md              # 项目说明
```

## 构建和运行

### 在 Windows 上开发

1. 确保已安装 .NET 8 SDK
2. 进入项目目录：
   ```bash
   cd XfceBlazorApp
   ```
3. 构建项目：
   ```bash
   dotnet build
   ```
4. 运行项目（Windows）：
   ```bash
   dotnet run
   ```

### 在 Ubuntu XFCE 上运行

#### 方法一：使用 dotnet 命令运行（推荐）

1. 在 Ubuntu 上安装 .NET 8 SDK
2. 复制以下文件到 Ubuntu 系统：
   ```
   XfceBlazorApp.dll
   XfceBlazorApp.deps.json
   XfceBlazorApp.runtimeconfig.json
   wwwroot/（整个目录）
   runtimes/（整个目录，包含 Photino.Native.so）
   ```
3. 运行应用：
   ```bash
   dotnet XfceBlazorApp.dll
   ```

#### 方法二：发布为自包含应用

1. 在 Windows 上发布自包含应用：
   ```bash
   dotnet publish -c Release -r linux-x64 --self-contained true
   ```
2. 复制发布目录（通常在 `bin/Release/net8.0/linux-x64/publish/`）到 Ubuntu 系统
3. 赋予执行权限：
   ```bash
   chmod +x XfceBlazorApp
   ```
4. 运行应用：
   ```bash
   ./XfceBlazorApp
   ```

## 功能特性

- 响应式设计，适配不同屏幕尺寸
- 使用 Blazor 开发交互式 UI
- 计数器示例功能
- 简洁的现代化 UI 设计

## 在 Ubuntu XFCE 上的优化

1. **主题集成**: 应用样式设计参考了 XFCE 桌面的配色方案
2. **性能优化**: 使用 Photino 轻量级浏览器引擎，资源占用低
3. **跨平台支持**: 同一代码库可在 Windows 和 Linux 上运行

## 故障排除

### 错误：无法执行二进制文件: 可执行文件格式错误

这通常是由于二进制文件架构与系统不匹配导致的。解决方案：

1. **检查系统架构**：
   ```bash
   uname -m
   ```
   - `x86_64` 表示 64 位 Intel/AMD 架构
   - `aarch64` 表示 64 位 ARM 架构

2. **针对正确架构重新发布**：
   ```bash
   # 对于 x86_64（Intel/AMD）
   dotnet publish -c Release -r linux-x64 --self-contained true
   
   # 对于 aarch64（ARM）
   dotnet publish -c Release -r linux-arm64 --self-contained true
   ```

3. **使用 dotnet 命令运行（推荐）**：
   ```bash
   # 安装 .NET 8 SDK（如果未安装）
   sudo apt update
   sudo apt install -y dotnet-sdk-8.0
   
   # 运行应用
   dotnet XfceBlazorApp.dll
   ```

### 错误：缺少 libglib 或其他依赖

如果遇到缺少系统库的错误，安装所需依赖：
```bash
sudo apt install -y libglib2.0-0 libgtk-3-0 libwebkit2gtk-4.0-37
```

## 开发说明

### 添加新页面

1. 在 `Pages` 目录下创建新的 `.razor` 文件
2. 添加 `@page "/route"` 指令设置路由
3. 编写页面内容和逻辑

### 修改样式

- 全局样式：修改 `wwwroot/css/app.css`
- 组件内样式：使用 Razor 组件的 `<style>` 标签

## 依赖项

- `Photino.Blazor`: 4.0.13
- `Microsoft.AspNetCore.Components.WebView`: 内置依赖

## 许可证

MIT License