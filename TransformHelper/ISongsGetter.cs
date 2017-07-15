using System.Net.Http;
using System.Threading.Tasks;
using Model;

namespace TransformHelper
{
    public interface ISongsGetter
    {
        HttpClient HttpClient { get; set; }

        TransformResult TransformResult { get; set; }

        Task<TransformResult> GetSongsBasedOnUrl(string url);
    }
}
