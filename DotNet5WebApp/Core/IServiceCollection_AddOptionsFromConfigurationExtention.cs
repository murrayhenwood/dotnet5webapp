using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotNet5WebApp.Core
{
    public static class IServiceCollection_AddOptionsFromConfigurationExtention
    {
        /// <summary>
        /// Adds options object from IConfiguration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddOptionsFromConfiguration<optionsType>(this IServiceCollection services, IConfiguration configuration)
            where optionsType : class
        {
            try
            {
                services.Configure<optionsType>(options =>
                {
                    IConfigurationSection optionSection = configuration.GetSection(typeof(optionsType).Name);

                    if (!optionSection.Exists()) { throw new Exception($"Section {typeof(optionsType).Name} cannot be found in configuration"); }

                    optionSection.Bind(options, binderOptions =>
                    {
                        binderOptions.BindNonPublicProperties = true;
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying options OptionsFromConfiguration {ex.Message}");
                throw;
            }
            return services;
        }

    }

    public static class IConfiguration_GetOptionsFromConfigurationExtention
    {
        public static optionsType GetOptionsFromConfiguration<optionsType>(this IConfiguration configuration, string sectionNameOverride = null)
           where optionsType : class, new()
        {

            string sectionName = String.IsNullOrWhiteSpace(sectionNameOverride) ? typeof(optionsType).Name : sectionNameOverride;

            var options = new optionsType();
            try
            {
                IConfigurationSection optionSection = configuration.GetSection(sectionName);

                if (!optionSection.Exists()) { throw new Exception($"Section {sectionName} cannot be found in configuration"); }

                optionSection.Bind(options, binderOptions =>
                {
                    binderOptions.BindNonPublicProperties = true;
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting options GetOptionsFromConfiguration {ex.Message}");
                throw;
            }
            return options;
        }

        public static optionsType GetOptionsFromConfiguration<optionsType>(this IConfiguration configuration, optionsType obj, string sectionNameOverride = null)
           where optionsType : class, new()
        {

            string sectionName = String.IsNullOrWhiteSpace(sectionNameOverride) ? typeof(optionsType).Name : sectionNameOverride;

            
            try
            {
                IConfigurationSection optionSection = configuration.GetSection(sectionName);

                if (!optionSection.Exists()) { throw new Exception($"Section {sectionName} cannot be found in configuration"); }

                optionSection.Bind(obj, binderOptions =>
                {
                    binderOptions.BindNonPublicProperties = true;
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting options GetOptionsFromConfiguration {ex.Message}");
                throw;
            }
            return obj;
        }
    }
}
