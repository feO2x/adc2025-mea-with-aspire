using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Audio;
using OpenAI.Chat;

namespace AiInformationExtractionApi.AiAccess;

public static class AiAccessModule
{
    [Experimental("MEAI001")]
    public static IServiceCollection AddAiAccess(this IServiceCollection services)
    {
        services.AddSingleton(sp => AiOptions.FromConfiguration(sp.GetRequiredService<IConfiguration>()));

        services
           .AddChatClient(sp =>
                {
                    var options = sp.GetRequiredService<AiOptions>();
                    return string.Equals(
                        options.TextVisionConnectionString,
                        "OpenAI",
                        StringComparison.OrdinalIgnoreCase
                    ) ?
                        new ChatClient(options.TextVisionModel, options.ApiKey).AsIChatClient() :
                        new OllamaChatClient(options.TextVisionConnectionString, options.TextVisionModel);
                }
            )
           .UseLogging()
           .UseOpenTelemetry();
        services.AddScoped<IAiChatClient, MeaChatClient>();

        services
           .AddSpeechToTextClient(sp =>
                {
                    var options = sp.GetRequiredService<AiOptions>();
                    return new AudioClient(options.AudioTranscriptionModel, options.ApiKey).AsISpeechToTextClient();
                }
            )
           .UseLogging();
        services.AddSingleton<IAiAudioClient, MeaAudioClient>();

        return services;
    }
}
