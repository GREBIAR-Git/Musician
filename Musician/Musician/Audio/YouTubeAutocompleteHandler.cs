using Discord;
using Discord.Interactions;
using YoutubeExplode;
using YoutubeExplode.Search;

namespace Musician.Audio;

public class YouTubeAutocompleteHandler : AutocompleteHandler
{
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        string? input = autocompleteInteraction.Data.Current.Value as string;
        YoutubeClient youtube = new();
        IAsyncEnumerable<VideoSearchResult> videos = youtube.Search.GetVideosAsync(input);

        IAsyncEnumerable<AutocompleteResult> autocompleteResults =
            videos.Take(5).Select(video => new AutocompleteResult(video.Title, video.Url));

        return Task.FromResult(AutocompletionResult.FromSuccess(autocompleteResults.ToEnumerable()));
    }
}