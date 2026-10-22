using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace AmanitaWebStory
{
    public interface IStoryView
    {
        UniTask ShowLineAsync(StoryLine Line, CancellationToken Token);
        UniTask<int> ChooseAsync(IReadOnlyList<StoryOptionData> Options, CancellationToken Token);
        UniTask PlaySoundAsync(StorySound Sound, CancellationToken Token);
    }
}
