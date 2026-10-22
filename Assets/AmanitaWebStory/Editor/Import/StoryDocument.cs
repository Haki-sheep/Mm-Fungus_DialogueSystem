using System;
using System.Collections.Generic;

namespace AmanitaWebStory.Editor
{
    [Serializable]
    public sealed class StoryDocument
    {
        public int version = 1;
        public string id;
        public string title;
        public string entry;
        public List<StoryVariable> variables = new List<StoryVariable>();
        public List<StoryNode> nodes = new List<StoryNode>();
    }

    [Serializable]
    public sealed class StoryVariable
    {
        public string key;
        public string title;
        public string type = "Integer";
        public string value = "0";
    }

    [Serializable]
    public sealed class StoryNode
    {
        public string id;
        public string title;
        public string type = "say";
        public float x;
        public float y;
        public string next;
        public string actor;
        public string portrait;
        public string text;
        public string voice;
        public List<StoryOption> options = new List<StoryOption>();
        public string variable;
        public string operation = "assign";
        public string value;
        public string comparison = "eq";
        public string onTrue;
        public string onFalse;
        public float seconds = 1;
        public string eventId;
        public string eventMode = "signal";
        public string eventTitle;
        public string eventValueKind = "String";
        public string audio;
        public float volume = 1;
        public bool wait = true;
    }

    [Serializable]
    public sealed class StoryOption
    {
        public string text;
        public string target;
    }
}
