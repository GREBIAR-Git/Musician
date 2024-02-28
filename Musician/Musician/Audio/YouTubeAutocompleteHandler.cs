using Discord;
using Discord.Interactions;
using YoutubeExplode;

namespace Musician.Audio
{
    public class YouTubeAutocompleteHandler : AutocompleteHandler
    {
        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            var input = autocompleteInteraction.Data.Current.Value as string;
            var youtube = new YoutubeClient();
            var videos = youtube.Search.GetVideosAsync(input);

            var autocompleteResults = videos.Take(5).Select(video => new AutocompleteResult(video.Title, video.Url));

            return AutocompletionResult.FromSuccess(autocompleteResults.ToEnumerable());
        }
    }
}
