using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace AmanitaWebStory
{
    public sealed class StorySession
    {
        private static readonly Regex VariablePattern = new Regex(@"\{\$([a-zA-Z][a-zA-Z0-9_]*)\}");
        private readonly StoryAsset story;
        private readonly Dictionary<string, StoryStep> stepDict = new Dictionary<string, StoryStep>();
        private readonly Dictionary<string, StoryValue> variableDict = new Dictionary<string, StoryValue>();
        private bool started;

        public string CurrentNodeId { get; private set; }
        public IReadOnlyDictionary<string, StoryValue> Variables { get; private set; }

        public StorySession(StoryAsset Story)
        {
            story = Story;
            foreach (var Step in Story.Steps)
                stepDict.Add(Step.Id, Step);
            foreach (var Variable in Story.Variables)
                variableDict.Add(Variable.Key, Variable.Value);
            Variables = new ReadOnlyDictionary<string, StoryValue>(variableDict);
        }

        public async UniTask RunAsync(IStoryView View, Action<StoryEvent> RaiseEvent, CancellationToken Token)
        {
            if (started)
                throw new InvalidOperationException("StorySession 只能执行一次 重播请创建新会话");
            started = true;
            CurrentNodeId = story.Entry;
            try
            {
                while (true)
                {
                    Token.ThrowIfCancellationRequested();
                    var Step = stepDict[CurrentNodeId];
                    switch (Step)
                    {
                        case StoryLine Line:
                            await View.ShowLineAsync(new StoryLine(Line.Id, Line.Next, Line.Speaker, Format(Line.Text), Line.Portrait, Line.Voice), Token);
                            break;
                        case StoryChoice Choice:
                            var Options = Choice.Options;
                            var DisplayList = new List<StoryOptionData>(Options.Count);
                            foreach (var Option in Options)
                                DisplayList.Add(new StoryOptionData(Format(Option.Text), Option.Target));
                            int Index = await View.ChooseAsync(DisplayList, Token);
                            if (Index < 0 || Index >= Options.Count)
                                throw new InvalidOperationException($"界面返回无效选项 {Index}");
                            CurrentNodeId = Options[Index].Target;
                            break;
                        case StoryAssignment Assignment:
                            variableDict[Assignment.Variable] = variableDict[Assignment.Variable].Apply(Assignment.Operation, Assignment.Value);
                            break;
                        case StoryCondition Condition:
                            CurrentNodeId = variableDict[Condition.Variable].Compare(Condition.Comparison, Condition.Value)
                                ? Condition.OnTrue : Condition.OnFalse;
                            break;
                        case StoryWait Wait:
                            await UniTask.Delay(TimeSpan.FromSeconds(Wait.Seconds), cancellationToken: Token);
                            break;
                        case StoryEvent Event:
                            if (RaiseEvent == null)
                                throw new InvalidOperationException($"事件 {Event.EventId} 未绑定接收逻辑");
                            RaiseEvent(Event);
                            break;
                        case StorySound Sound: await View.PlaySoundAsync(Sound, Token); break;
                        case StoryEnd End: return;
                        default: throw new NotSupportedException(Step.GetType().FullName);
                    }
                    if (!(Step is StoryChoice) && !(Step is StoryCondition))
                        CurrentNodeId = Step.Next;
                    // 无输入的回路也必须让出主线程并响应取消
                    await UniTask.Yield(PlayerLoopTiming.Update, Token);
                }
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception Error)
            {
                throw new InvalidOperationException($"剧情 {story.Id} 节点 {CurrentNodeId} 执行失败", Error);
            }
        }

        private string Format(string Text)
        {
            return VariablePattern.Replace(Text, Match => variableDict[Match.Groups[1].Value].ToString());
        }
    }
}
