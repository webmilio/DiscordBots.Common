using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Common;

public static class ConfigurationExtensions
{
    public record ConfigurationOptions(Action<Type>? PreRegister = null, Action<Type, Action<object>>? PostRegister = null);

    public static IHostApplicationBuilder SetupConfiguration(this IHostApplicationBuilder builder, ConfigurationOptions options, params Assembly[] assemblies)
    {
        // There is no way this is the way to do it
        var methods = typeof(OptionsServiceCollectionExtensions).GetMethods();
        var method = methods.Where(m =>
            m.Name.Equals(nameof(OptionsServiceCollectionExtensions.Configure)) &&
            m.GetParameters().Length == 2 && m.ContainsGenericParameters).First();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    continue;
                }

                var attr = type.GetCustomAttribute<ConfigurationSectionAttribute>();

                if (attr == null)
                {
                    continue;
                }

                options.PreRegister?.Invoke(type);

                var section = builder.Configuration.GetSection(attr.SectionName);

                var typedMethod = method.MakeGenericMethod([type]);
                var action = (object instance) => builder.Configuration.GetSection(attr.SectionName).Bind(instance);

                typedMethod.Invoke(null, [builder.Services, action]);

                options.PostRegister?.Invoke(type, action);
            }
        }

        return builder;
    }
}
