using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace AmanitaWebStory.Editor
{
    public static class StoryValidator
    {
        public const int MaxNodes = 1000;
        private static readonly Regex IdPattern = new Regex("^[a-zA-Z][a-zA-Z0-9_]{0,63}$");
        private static readonly HashSet<string> NodeTypeHashList = new HashSet<string> { "say", "choice", "condition", "set", "wait", "event", "sound", "end" };

        #region 文档验证

        public static StoryDocument Parse(string Json, out List<string> Warnings)
        {
            var Object = JObject.Parse(Json, new JsonLoadSettings { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
            ValidateShape(Object);
            var Document = Object.ToObject<StoryDocument>(JsonSerializer.Create(new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error,
                TypeNameHandling = TypeNameHandling.None,
                MaxDepth = 32
            }));
            Require(Document.version == 1, "仅支持 version = 1 的剧情数据");
            Require(Document.id != null && IdPattern.IsMatch(Document.id), "剧情标识只能使用英文字母开头的字母 数字 下划线 最多64字");
            Require(!string.IsNullOrWhiteSpace(Document.title), "请填写剧情标题");
            Require(Document.nodes != null && Document.nodes.Count > 0 && Document.nodes.Count <= MaxNodes, $"节点数量必须为 1 到 {MaxNodes}");
            Require(Document.variables != null, "variables 不能是 null");
            var NodeDict = new Dictionary<string, StoryNode>();
            foreach (var Node in Document.nodes)
            {
                Require(Node != null && Node.id != null && IdPattern.IsMatch(Node.id), "节点标识格式错误");
                Require(!NodeDict.ContainsKey(Node.id), "节点标识重复 " + Node.id);
                Require(!string.IsNullOrWhiteSpace(Node.title), Node.id + " 缺少节点标题");
                Require(NodeTypeHashList.Contains(Node.type ?? ""), Node.id + " 不支持的节点类型 " + Node.type);
                Require(IsFinite(Node.x) && IsFinite(Node.y), Node.id + " 坐标必须是有限数字");
                NodeDict.Add(Node.id, Node);
            }
            Require(Document.entry != null && NodeDict.ContainsKey(Document.entry), "开始节点不存在");
            var VariableDict = new Dictionary<string, StoryVariable>();
            foreach (var Variable in Document.variables)
            {
                Require(Variable != null && Variable.key != null && IdPattern.IsMatch(Variable.key), "变量标识格式错误");
                Require(!VariableDict.ContainsKey(Variable.key), "变量重复 " + Variable.key);
                Require(!string.IsNullOrWhiteSpace(Variable.title), Variable.key + " 缺少中文变量名称");
                Require(new[] { "Integer", "Float", "Boolean", "String" }.Contains(Variable.type), Variable.key + " 变量类型只支持整数 小数 开关或文字");
                ValidateValue(Variable.type, Variable.value, "变量 " + Variable.title);
                VariableDict.Add(Variable.key, Variable);
            }
            var ActorDict = StoryCatalog.Actors();
            var SignalDict = new Dictionary<string, StoryNode>();
            foreach (var Node in Document.nodes)
            {
                string Context = $"节点「{Node.title}」({Node.id})";
                foreach (string Target in Targets(Node))
                    Require(!string.IsNullOrEmpty(Target) && NodeDict.ContainsKey(Target), Context + " 后续节点不存在 " + Target);
                ValidateNode(Node, Context, VariableDict, ActorDict);
                if (Node.type == "event")
                {
                    if (SignalDict.TryGetValue(Node.eventId, out var Previous))
                        Require(Previous.eventTitle == Node.eventTitle && Previous.eventValueKind == Node.eventValueKind,
                            Context + " 同一事件标识的名称和参数类型必须一致");
                    else
                        SignalDict.Add(Node.eventId, Node);
                }
            }
            Warnings = new List<string>();
            var ReachableHashList = new HashSet<string>();
            var Pending = new Stack<string>();
            Pending.Push(Document.entry);
            while (Pending.Count > 0)
            {
                string Id = Pending.Pop();
                if (!ReachableHashList.Add(Id))
                    continue;
                foreach (string Target in Targets(NodeDict[Id]))
                    Pending.Push(Target);
            }
            foreach (var Node in Document.nodes.Where(Node => !ReachableHashList.Contains(Node.id)))
                Warnings.Add("从开始节点无法到达「" + Node.title + "」");
            return Document;
        }

        private static void ValidateNode(StoryNode Node, string Context, Dictionary<string, StoryVariable> VariableDict, Dictionary<string, StoryActor> ActorDict)
        {
            switch (Node.type)
            {
                case "say":
                    Require(!string.IsNullOrWhiteSpace(Node.text), Context + " 请填写对白");
                    ActorDict.TryGetValue(Node.actor ?? "", out var Actor);
                    Require(string.IsNullOrEmpty(Node.actor) || Actor != null, Context + " 角色不存在 请在 Unity 创建 StoryActor 后刷新能力目录");
                    Require(string.IsNullOrEmpty(Node.portrait) || Actor != null && Actor.Portraits.Any(Sprite => Sprite.name == Node.portrait), Context + " 表情不属于此角色");
                    ValidateAudio(Node.voice, Context);
                    break;
                case "choice":
                    Require(Node.options != null && Node.options.Count >= 2 && Node.options.Count <= StoryCatalog.MaxOptions, Context + $" 需要 2 到 {StoryCatalog.MaxOptions} 个选项");
                    Require(Node.options.All(Option => Option != null && !string.IsNullOrWhiteSpace(Option.text)), Context + " 选项文字不能为空");
                    break;
                case "condition":
                case "set":
                    Require(Node.variable != null && VariableDict.ContainsKey(Node.variable), Context + " 变量不存在");
                    var Variable = VariableDict[Node.variable];
                    ValidateValue(Variable.type, Node.value, Context);
                    bool Numeric = Variable.type == "Integer" || Variable.type == "Float";
                    if (Node.type == "set")
                        Require(Node.operation == "assign" || Numeric && (Node.operation == "add" || Node.operation == "subtract"), Context + " 此变量不支持这种修改方式");
                    else
                        Require(Node.comparison == "eq" || Node.comparison == "neq" || Numeric && new[] { "gt", "gte", "lt", "lte" }.Contains(Node.comparison), Context + " 此变量不支持这种比较方式");
                    break;
                case "wait":
                    Require(IsFinite(Node.seconds) && Node.seconds >= 0 && Node.seconds <= 3600, Context + " 等待时间须为0到3600秒");
                    break;
                case "event":
                    Require(Node.eventMode == "signal", Context + " 事件只支持 signal 模式 由 StoryPlayer.EventRaised 接收");
                    Require(Node.eventId != null && IdPattern.IsMatch(Node.eventId), Context + " 事件标识格式错误");
                    Require(!string.IsNullOrWhiteSpace(Node.eventTitle), Context + " 请填写事件名称");
                    Require(new[] { "None", "String", "Integer", "Float", "Boolean" }.Contains(Node.eventValueKind), Context + " 未知事件参数类型");
                    ValidateValue(Node.eventValueKind, Node.value, Context);
                    break;
                case "sound":
                    Require(!string.IsNullOrEmpty(Node.audio), Context + " 请选择音效");
                    ValidateAudio(Node.audio, Context);
                    Require(IsFinite(Node.volume) && Node.volume >= 0 && Node.volume <= 1, Context + " 音量须为0到1");
                    break;
            }
        }

        #endregion

        #region 值与连接

        private static void ValidateShape(JObject Object)
        {
            Require(Object["version"]?.Type == JTokenType.Integer, "version 必须是整数");
            foreach (string Key in new[] { "id", "title", "entry" })
                Require(Object[Key]?.Type == JTokenType.String, Key + " 必须是文字");
            Require(Object["nodes"] is JArray && Object["variables"] is JArray, "nodes 与 variables 必须是数组");
            foreach (var Token in (JArray)Object["variables"])
            {
                Require(Token is JObject, "变量必须是对象");
                foreach (string Key in new[] { "key", "title", "type", "value" })
                    Require(Token[Key]?.Type == JTokenType.String, "变量 " + Key + " 必须是文字");
            }
            foreach (var Token in (JArray)Object["nodes"])
            {
                Require(Token is JObject, "节点必须是对象");
                foreach (string Key in new[] { "id", "title", "type" })
                    Require(Token[Key]?.Type == JTokenType.String, "节点 " + Key + " 必须是文字");
                foreach (string Key in new[] { "x", "y" })
                    Require(Token[Key]?.Type == JTokenType.Integer || Token[Key]?.Type == JTokenType.Float, "节点 " + Key + " 必须是数字");
                foreach (string Key in new[] { "value", "next", "actor", "portrait", "text", "voice", "variable", "operation", "comparison", "onTrue", "onFalse", "eventId", "eventMode", "eventTitle", "eventValueKind", "audio" })
                    Require(Token[Key] == null || Token[Key].Type == JTokenType.Null || Token[Key].Type == JTokenType.String, "节点 " + Key + " 必须是文字");
                foreach (string Key in new[] { "seconds", "volume" })
                    Require(Token[Key] == null || Token[Key].Type == JTokenType.Integer || Token[Key].Type == JTokenType.Float, "节点 " + Key + " 必须是数字");
                Require(Token["wait"] == null || Token["wait"].Type == JTokenType.Boolean, "节点 wait 必须是布尔值");
            }
        }

        public static IEnumerable<string> Targets(StoryNode Node)
        {
            if (Node.type == "end") yield break;
            if (Node.type == "choice")
            {
                Require(Node.options != null, Node.id + " 缺少选项列表");
                foreach (var Option in Node.options)
                {
                    Require(Option != null, Node.id + " 选项不能是 null");
                    yield return Option.target;
                }
            }
            else if (Node.type == "condition")
            {
                yield return Node.onTrue;
                yield return Node.onFalse;
            }
            else yield return Node.next;
        }

        public static void ValidateValue(string Kind, string Value, string Context)
        {
            bool Valid;
            switch (Kind)
            {
                case "None": Valid = string.IsNullOrEmpty(Value); break;
                case "String": Valid = Value != null; break;
                case "Integer": Valid = int.TryParse(Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int Integer); break;
                case "Float": Valid = float.TryParse(Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float Number) && IsFinite(Number); break;
                case "Boolean": Valid = Value == "true" || Value == "false"; break;
                default: throw new ArgumentException(Context + " 未知的参数类型 " + Kind);
            }
            Require(Valid, Context + " 参数不符合 " + Kind + " 类型 " + Value);
        }

        private static void ValidateAudio(string Guid, string Context)
        {
            Require(string.IsNullOrEmpty(Guid) || AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(Guid)) != null, Context + " 音频资源不存在 请刷新能力目录后重选");
        }

        private static bool IsFinite(float Value) => !float.IsNaN(Value) && !float.IsInfinity(Value);

        private static void Require(bool Condition, string Message)
        {
            if (!Condition)
                throw new ArgumentException(Message);
        }

        #endregion
    }
}
