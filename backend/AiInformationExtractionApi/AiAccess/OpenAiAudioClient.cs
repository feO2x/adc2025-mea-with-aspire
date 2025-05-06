using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using OpenAI.Audio;

namespace AiInformationExtractionApi.AiAccess;

[Experimental("MEAI001")]
public sealed class MeaAudioClient : IAiAudioClient
{
    private readonly ISpeechToTextClient _speechToTextClient;

    public MeaAudioClient(ISpeechToTextClient speechToTextClient) => _speechToTextClient = speechToTextClient;

    public async Task<string> TranscribeAudioAsync(string audioFilePath)
    {
        await using var fileStream = File.OpenRead(audioFilePath);
        var response = await _speechToTextClient.GetTextAsync(fileStream);
        return response.Text;
    }
}

public sealed class OpenAiAudioClient : IAiAudioClient
{
    private readonly AudioClient _audioClient;
    private readonly AudioTranscriptionOptions _options;

    public OpenAiAudioClient(AudioClient audioClient, AudioTranscriptionOptions options)
    {
        _audioClient = audioClient;
        _options = options;
    }

    public async Task<string> TranscribeAudioAsync(string filePath)
    {
        var result = await _audioClient.TranscribeAudioAsync(filePath, _options);
        return result.Value.Text;
    }
}
