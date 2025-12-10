﻿using DrivePal.Core.Services;
using DrivePal.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;

namespace DrivePalApp
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        { 


            // 创建Photino Blazor应用构建器
            var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

            // 添加必要的Blazor服务
            appBuilder.Services.AddBlazorWebView();
            // 配置服务 
            var configuration = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appcontext.json", optional: false, reloadOnChange: true)
                .Build();
            appBuilder.Services.AddInfrastructureServices(configuration);





            // 配置根组件 - 这是关键！
            appBuilder.RootComponents.Add<App>("#app"); 

            // 构建应用
            var app = appBuilder.Build();

            // 配置窗口
            app.MainWindow
                .SetTitle("DrivePal - 变频器选型计算器")
                .SetWidth(1024)
                .SetHeight(768)
                .Center();

            // 启动应用
            app.Run();
        }

         
    }
}
