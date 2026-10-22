using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AmanitaWebStory.Editor
{
    public static class StoryAssetExporter
    {
        public const string Root = "Assets/AmanitaWebStory";
        public const string ExportRoot = Root + "/Stories";
        private static readonly Regex VariablePattern = new Regex(@"\{\$([^}]*)\}");

        public static StoryAsset Export(string Json, string Path)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("请退出 Play 后导出剧情");
            var Document = StoryValidator.Parse(Json, out var Warnings);
            var Variables = Document.variables.ToDictionary(Item => Item.key, Item => Enum.Parse<eStoryDataType>(Item.type));
            foreach (var Node in Document.nodes)
            {
                if (Node.type == "say")
                    ValidateText(Node.text, Node.id, Variables);
                if (Node.type == "choice")
                    foreach (var Option in Node.options)
                        ValidateText(Option.text, Node.id, Variables);
            }
            var ActorDict = StoryCatalog.Actors();
            var VariableList = Document.variables.Select(Item => new StoryInitialVariable(Item.key, StoryValue.Parse(Variables[Item.key], Item.value))).ToList();
            var StepList = Document.nodes.Select(Node => Convert(Node, Variables, ActorDict)).ToList();
            var Existing = AssetDatabase.LoadMainAssetAtPath(Path);
            if (Existing != null && (!(Existing is StoryAsset Previous) || Previous.Id != Document.id))
                throw new InvalidOperationException("目标路径已被其他资产占用 " + Path);
            var Asset = Existing as StoryAsset;
            if (Asset == null)
                Asset = ScriptableObject.CreateInstance<StoryAsset>();
            else
                Undo.RecordObject(Asset, "更新剧情");
            Asset.SetContent(Document.id, Document.title, Document.entry, Json, VariableList, StepList);
            if (Existing == null)
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
                AssetDatabase.Refresh();
                AssetDatabase.CreateAsset(Asset, Path);
            }
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();
            foreach (string Warning in Warnings)
                Debug.LogWarning(Warning, Asset);
            return Asset;
        }

        private static StoryStep Convert(StoryNode Node, Dictionary<string, eStoryDataType> VariableDict, Dictionary<string, StoryActor> ActorDict)
        {
            switch (Node.type)
            {
                case "say":
                    var Actor = string.IsNullOrEmpty(Node.actor) ? null : ActorDict[Node.actor];
                    var Portrait = string.IsNullOrEmpty(Node.portrait) ? null : Actor.Portraits.Single(Sprite => Sprite.name == Node.portrait);
                    return new StoryLine(Node.id, Node.next, Actor, Node.text, Portrait, Load<AudioClip>(Node.voice));
                case "choice": return new StoryChoice(Node.id, Node.options.Select(Option => new StoryOptionData(Option.text, Option.target)).ToList());
                case "set":
                    var Operation = Node.operation == "assign" ? eStoryAssignment.Assign : Node.operation == "add" ? eStoryAssignment.Add : eStoryAssignment.Subtract;
                    return new StoryAssignment(Node.id, Node.next, Node.variable, Operation, StoryValue.Parse(VariableDict[Node.variable], Node.value));
                case "condition":
                    return new StoryCondition(Node.id, Node.variable, Comparison(Node.comparison), StoryValue.Parse(VariableDict[Node.variable], Node.value), Node.onTrue, Node.onFalse);
                case "wait": return new StoryWait(Node.id, Node.next, Node.seconds);
                case "sound": return new StorySound(Node.id, Node.next, Load<AudioClip>(Node.audio), Node.volume, Node.wait);
                case "event":
                    return new StoryEvent(Node.id, Node.next, Node.eventId, StoryValue.Parse(Enum.Parse<eStoryDataType>(Node.eventValueKind), Node.value));
                case "end": return new StoryEnd(Node.id);
                default: throw new NotSupportedException(Node.type);
            }
        }

        private static eStoryComparison Comparison(string Value)
        {
            switch (Value)
            {
                case "eq": return eStoryComparison.Equal;
                case "neq": return eStoryComparison.NotEqual;
                case "gt": return eStoryComparison.Greater;
                case "gte": return eStoryComparison.GreaterOrEqual;
                case "lt": return eStoryComparison.Less;
                case "lte": return eStoryComparison.LessOrEqual;
                default: throw new ArgumentException(Value);
            }
        }

        private static T Load<T>(string Guid) where T : UnityEngine.Object
        {
            return string.IsNullOrEmpty(Guid) ? null : AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(Guid));
        }

        // 变量占位由 StorySession 替换 其余 {} 标签交给 Fungus Writer 解析
        private static void ValidateText(string Text, string NodeId, Dictionary<string, eStoryDataType> VariableDict)
        {
            foreach (Match Match in VariablePattern.Matches(Text))
            {
                string Key = Match.Groups[1].Value;
                if (!VariableDict.ContainsKey(Key))
                    throw new ArgumentException($"节点 {NodeId} 引用了不存在的变量 {Key}");
            }
        }
    }
}
