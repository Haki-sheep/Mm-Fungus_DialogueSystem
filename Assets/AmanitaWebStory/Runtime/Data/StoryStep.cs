using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmanitaWebStory
{
    [Serializable]
    public abstract class StoryStep
    {
        [field: SerializeField]
        public string Id { get; private set; }
        [field: SerializeField]
        public string Next { get; private set; }

        protected StoryStep(string Id, string Next) { this.Id = Id; this.Next = Next; }
    }

    [Serializable]
    public sealed class StoryLine : StoryStep
    {
        /// <summary> 为空表示旁白 </summary>
        [field: SerializeField]
        public StoryActor Actor { get; private set; }
        [field: SerializeField, TextArea]
        public string Text { get; private set; }
        [field: SerializeField]
        public Sprite Portrait { get; private set; }
        [field: SerializeField]
        public AudioClip Voice { get; private set; }

        public StoryLine(string Id, string Next, StoryActor Actor, string Text, Sprite Portrait = null, AudioClip Voice = null)
            : base(Id, Next)
        {
            this.Actor = Actor;
            this.Text = Text;
            this.Portrait = Portrait;
            this.Voice = Voice;
        }
    }

    [Serializable]
    public sealed class StoryOptionData
    {
        [field: SerializeField]
        public string Text { get; private set; }
        [field: SerializeField]
        public string Target { get; private set; }

        public StoryOptionData(string Text, string Target) { this.Text = Text; this.Target = Target; }
    }

    [Serializable]
    public sealed class StoryChoice : StoryStep
    {
        [SerializeField]
        private List<StoryOptionData> optionList;

        public IReadOnlyList<StoryOptionData> Options => optionList.AsReadOnly();

        public StoryChoice(string Id, List<StoryOptionData> OptionList) : base(Id, null)
        {
            optionList = new List<StoryOptionData>(OptionList);
        }
    }

    [Serializable]
    public sealed class StoryAssignment : StoryStep
    {
        [field: SerializeField]
        public string Variable { get; private set; }
        [field: SerializeField]
        public eStoryAssignment Operation { get; private set; }
        [field: SerializeField]
        public StoryValue Value { get; private set; }

        public StoryAssignment(string Id, string Next, string Variable, eStoryAssignment Operation, StoryValue Value) : base(Id, Next)
        { this.Variable = Variable; this.Operation = Operation; this.Value = Value; }
    }

    [Serializable]
    public sealed class StoryCondition : StoryStep
    {
        [field: SerializeField]
        public string Variable { get; private set; }
        [field: SerializeField]
        public eStoryComparison Comparison { get; private set; }
        [field: SerializeField]
        public StoryValue Value { get; private set; }
        [field: SerializeField]
        public string OnTrue { get; private set; }
        [field: SerializeField]
        public string OnFalse { get; private set; }

        public StoryCondition(string Id, string Variable, eStoryComparison Comparison, StoryValue Value, string OnTrue, string OnFalse) : base(Id, null)
        {
            this.Variable = Variable; this.Comparison = Comparison; this.Value = Value;
            this.OnTrue = OnTrue; this.OnFalse = OnFalse;
        }
    }

    [Serializable]
    public sealed class StoryWait : StoryStep
    {
        [field: SerializeField]
        public float Seconds { get; private set; }

        public StoryWait(string Id, string Next, float Seconds) : base(Id, Next) { this.Seconds = Seconds; }
    }

    [Serializable]
    public sealed class StoryEvent : StoryStep
    {
        [field: SerializeField]
        public string EventId { get; private set; }
        [field: SerializeField]
        public StoryValue Value { get; private set; }

        public StoryEvent(string Id, string Next, string EventId, StoryValue Value) : base(Id, Next)
        { this.EventId = EventId; this.Value = Value; }
    }

    [Serializable]
    public sealed class StorySound : StoryStep
    {
        [field: SerializeField]
        public AudioClip Clip { get; private set; }
        [field: SerializeField]
        public float Volume { get; private set; }
        [field: SerializeField]
        public bool WaitUntilFinished { get; private set; }

        public StorySound(string Id, string Next, AudioClip Clip, float Volume, bool WaitUntilFinished) : base(Id, Next)
        { this.Clip = Clip; this.Volume = Volume; this.WaitUntilFinished = WaitUntilFinished; }
    }

    [Serializable]
    public sealed class StoryEnd : StoryStep
    {
        public StoryEnd(string Id) : base(Id, null) { }
    }
}
