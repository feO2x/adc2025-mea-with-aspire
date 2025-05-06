using System.Threading.Tasks;

namespace AiInformationExtractionApi.AiAccess;

public interface IAiAudioClient
{
    Task<string> TranscribeAudioAsync(string audioFilePath);
}
