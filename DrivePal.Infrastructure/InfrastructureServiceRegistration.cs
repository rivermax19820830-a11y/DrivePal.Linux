using DrivePal.Core.Services;
using DrivePal.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivePal.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            // 注册数据库上下文
            services.AddScoped<SqlSugarDbContext>(provider =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                var dbContext = new SqlSugarDbContext(connectionString);
                
                //dbContext.InitTables(); // 初始化表结构, 第一次使用
                return dbContext;
            });

            services.AddScoped<IDeviceService, DeviceService>();
            
            // 注册计算策略
            services.AddKeyedScoped<IMechanismCalculationStrategy, HoistingCalculationStrategy>("hoist");
            services.AddKeyedScoped<IMechanismCalculationStrategy, TrolleyCalculationStrategy>("trolley");
            services.AddKeyedScoped<IMechanismCalculationStrategy, LufferCalculationStrategy>("luffer");
            services.AddKeyedScoped<IMechanismCalculationStrategy, RotatingCalculationStrategy>("rotate");
            services.AddKeyedScoped<IMechanismCalculationStrategy, AFECalculationStrategy>("afe");
            services.AddKeyedScoped<IMechanismCalculationStrategy, GeneralCalculationStrategy>("common");


            return services;
        }
    }
}
