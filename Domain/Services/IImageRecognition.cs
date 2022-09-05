using System.Threading.Tasks;

namespace DiscordBot.Domain.Services;

public interface IImageRecognition
{
    public Task<string> ConvertImageToCsv(byte[] image);
}