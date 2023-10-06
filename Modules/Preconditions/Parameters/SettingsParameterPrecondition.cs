using Discord;
using Discord.Interactions;
using InnoTasker.Data.ToDo;
using InnoTasker.Services.Interfaces.ToDo;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Modules.Preconditions.Parameters
{
    public abstract class SettingsParameterPrecondition : InnoParamterPrecondition
    {
        protected IToDoSettingsService _settingsServices;
        protected ToDoSettingsInstance _instance;

        public async override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            _settingsServices = services.GetRequiredService<IToDoSettingsService>();
            if (!await _settingsServices.InstanceExists(context.Channel.Id)) return FromError("Sorry, this must be done with a settings instance open via /admin opensettingsmenu");
            _instance = await _settingsServices.GetSettingsInstance(context.Channel.Id);
            return await CheckRequirements(context, parameterInfo, value, services);
        }
    }
}
