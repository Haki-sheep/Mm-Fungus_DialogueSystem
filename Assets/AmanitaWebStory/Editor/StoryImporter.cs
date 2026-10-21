using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Fungus;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace AmanitaWebStory.Editor
{
    public static class StoryImporter
    {
        #region 读写资产

        public static object Import(string Json, string ExpectedRevision)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("请退出 Play 后回填剧情");
            var Document = StoryValidator.Parse(Json, out var Warnings);
            string Path = AssetPath(Document.id);
            var Existing = AssetDatabase.LoadAssetAtPath<GameObject>(Path);
            if (Existing != null)
            {
                var Previous = Existing.GetComponent<WebStoryHost>();
                if (Previous == null || Previous.StoryId != Document.id || string.IsNullOrEmpty(Previous.SourceJson))
                    throw new InvalidOperationException("目标路径已被非网页剧情资产占用 " + Path);
                if (Revision(Previous.SourceJson) != ExpectedRevision)
                    throw new InvalidOperationException("Unity 里已有更新版本 请先读取 Unity 版本后再编辑回填 或更换剧情标识另存");
            }
            else if (!string.IsNullOrEmpty(ExpectedRevision))
                throw new InvalidOperationException("原剧情资产已被移除 请重新读取 Unity 目录");
            string Canonical = JsonConvert.SerializeObject(Document, Formatting.Indented);
            Directory.CreateDirectory(StoryTemplateBuilder.GeneratedRoot);
            byte[] Backup = File.Exists(Path) ? File.ReadAllBytes(Path) : null;
            var SignalUpdate = new StorySignalAssetUpdate(Document);
            var RootObject = PrefabUtility.LoadPrefabContents(StoryTemplateBuilder.TemplatePath);
            bool SaveAttempted = false;
            string EventAssetPath = "";
            try
            {
                Build(RootObject.GetComponent<WebStoryHost>(), Document);
                var Host = RootObject.GetComponent<WebStoryHost>();
                Host.SignalCatalog = SignalUpdate.Apply();
                EventAssetPath = AssetDatabase.GetAssetPath(Host.SignalCatalog);
                Host.SourceJson = Canonical;
                Host.StoryId = Document.id;
                RootObject.name = Document.title;
                SaveAttempted = true;
                var Saved = PrefabUtility.SaveAsPrefabAsset(RootObject, Path, out bool Success);
                if (!Success || Saved == null)
                    throw new IOException("生成剧情预制体失败 " + Path);
            }
            catch (Exception Error)
            {
                try
                {
                    SignalUpdate.Rollback();
                    if (SaveAttempted && Backup != null)
                    {
                        File.WriteAllBytes(Path, Backup);
                        AssetDatabase.ImportAsset(Path, ImportAssetOptions.ForceUpdate);
                    }
                    else if (SaveAttempted && File.Exists(Path))
                        AssetDatabase.DeleteAsset(Path);
                }
                catch (Exception RollbackError)
                {
                    throw new AggregateException("剧情回填失败 且回滚未完成", Error, RollbackError);
                }
                throw;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(RootObject);
            }
            return new { assetPath = Path, eventAssetPath = EventAssetPath, revision = Revision(Canonical), nodes = Document.nodes.Count, warnings = Warnings };
        }

        public static object Read(string Id)
        {
            string Path = AssetPath(Id);
            var Asset = AssetDatabase.LoadAssetAtPath<GameObject>(Path);
            if (Asset == null || Asset.GetComponent<WebStoryHost>() == null)
                throw new FileNotFoundException("没有找到网页剧情 " + Id);
            var Host = Asset.GetComponent<WebStoryHost>();
            return new { document = JsonConvert.DeserializeObject<StoryDocument>(Host.SourceJson), revision = Revision(Host.SourceJson), assetPath = Path };
        }

        public static string AssetPath(string Id)
        {
            if (Id == null || !System.Text.RegularExpressions.Regex.IsMatch(Id, "^[a-zA-Z][a-zA-Z0-9_]{0,63}$"))
                throw new ArgumentException("剧情标识格式错误");
            return StoryTemplateBuilder.GeneratedRoot + "/" + Id + ".prefab";
        }

        public static string Revision(string Json)
        {
            using (var Hash = SHA256.Create())
                return BitConverter.ToString(Hash.ComputeHash(Encoding.UTF8.GetBytes(Json))).Replace("-", "").ToLowerInvariant();
        }

        #endregion

        #region 编译原生流程

        private static void Build(WebStoryHost Host, StoryDocument Document)
        {
            var Graph = Host.Flowchart;
            if (Graph.GetComponents<Block>().Length > 0 || Graph.Variables.Count > 0)
                throw new InvalidOperationException("剧情模板只能包含界面角色和事件绑定 不应包含剧情 Block 或变量");
            var VariableDict = new Dictionary<string, Variable>();
            foreach (var Definition in Document.variables)
            {
                Variable Variable;
                switch (Definition.type)
                {
                    case "Integer":
                        var Integer = Graph.gameObject.AddComponent<IntegerVariable>();
                        Integer.Value = int.Parse(Definition.value, CultureInfo.InvariantCulture);
                        Variable = Integer;
                        break;
                    case "Float":
                        var Float = Graph.gameObject.AddComponent<FloatVariable>();
                        Float.Value = float.Parse(Definition.value, CultureInfo.InvariantCulture);
                        Variable = Float;
                        break;
                    case "Boolean":
                        var Boolean = Graph.gameObject.AddComponent<BooleanVariable>();
                        Boolean.Value = bool.Parse(Definition.value);
                        Variable = Boolean;
                        break;
                    case "String":
                        var String = Graph.gameObject.AddComponent<StringVariable>();
                        String.Value = Definition.value;
                        Variable = String;
                        break;
                    default: throw new ArgumentException(Definition.type);
                }
                Variable.Key = Definition.key;
                Graph.Variables.Add(Variable);
                VariableDict.Add(Definition.key, Variable);
            }
            var BlockDict = new Dictionary<string, Block>();
            foreach (var Node in Document.nodes)
            {
                var Block = Graph.CreateBlock(new Vector2(Node.x, Node.y));
                Block.BlockName = Node.title + " [" + Node.id + "]";
                BlockDict.Add(Node.id, Block);
            }
            foreach (var Node in Document.nodes)
            {
                var Block = BlockDict[Node.id];
                Add<WaitFrames>(Block);
                CompileNode(Host, Node, Block, BlockDict, VariableDict);
                if (Node.type != "end" && Node.type != "choice" && Node.type != "condition")
                    Jump(Block, BlockDict[Node.next]);
                Block.UpdateIndentLevels();
            }
            Host.Entry = BlockDict[Document.entry];
            Host.AutoPlay = true;
        }

        private static void CompileNode(WebStoryHost Host, StoryNode Node, Block Block, Dictionary<string, Block> BlockDict, Dictionary<string, Variable> VariableDict)
        {
            switch (Node.type)
            {
                case "say":
                    var Say = Add<Say>(Block);
                    Set(Say, "storyText", Node.text);
                    Set(Say, "character", Host.GetComponentsInChildren<Character>(true).SingleOrDefault(Actor => Actor.name == Node.actor));
                    Set(Say, "portrait", Load<Sprite>(Node.portrait));
                    Set(Say, "voiceOverClip", Load<AudioClip>(Node.voice));
                    Set(Say, "setSayDialog", Host.SayDialog);
                    Set(Say, "fadeWhenDone", true);
                    break;
                case "choice":
                    foreach (var Option in Node.options)
                    {
                        var Menu = Add<Fungus.Menu>(Block);
                        Menu.SetMenuDialog = Host.MenuDialog;
                        Set(Menu, "text", Option.text);
                        Set(Menu, "targetBlock", BlockDict[Option.target]);
                    }
                    break;
                case "set":
                    var Assign = Add<SetVariable>(Block);
                    Set(Assign, "anyVar", Pair(VariableDict[Node.variable], Node.value));
                    Set(Assign, "setOperator", Node.operation == "add" ? SetOperator.Add : Node.operation == "subtract" ? SetOperator.Subtract : SetOperator.Assign);
                    break;
                case "condition":
                    var Condition = Add<If>(Block);
                    var ComparisonDict = new Dictionary<string, CompareOperator>
                    {
                        { "eq", CompareOperator.Equals }, { "neq", CompareOperator.NotEquals }, { "gt", CompareOperator.GreaterThan },
                        { "gte", CompareOperator.GreaterThanOrEquals }, { "lt", CompareOperator.LessThan }, { "lte", CompareOperator.LessThanOrEquals }
                    };
                    Set(Condition, "conditions", new List<ConditionExpression> { new ConditionExpression(ComparisonDict[Node.comparison], Pair(VariableDict[Node.variable], Node.value)) });
                    Jump(Block, BlockDict[Node.onTrue]);
                    Add<Else>(Block);
                    Jump(Block, BlockDict[Node.onFalse]);
                    Add<End>(Block);
                    break;
                case "wait": Set(Add<Wait>(Block), "_duration", new FloatData(Node.seconds)); break;
                case "event":
                    var Event = Add<WebStoryEvent>(Block);
                    Set(Event, "host", Host);
                    Set(Event, "eventId", Node.eventId);
                    Set(Event, "value", Node.value ?? "");
                    Set(Event, "broadcast", Node.eventMode == "signal");
                    break;
                case "sound":
                    var Sound = Add<PlaySound>(Block);
                    Set(Sound, "soundClip", Load<AudioClip>(Node.audio));
                    Set(Sound, "volume", Node.volume);
                    Set(Sound, "waitUntilFinished", Node.wait);
                    break;
            }
        }

        private static AnyVariableAndDataPair Pair(Variable Variable, string Value)
        {
            var Data = new AnyVariableData();
            if (Variable is IntegerVariable) Data.integerData = new IntegerData(int.Parse(Value, CultureInfo.InvariantCulture));
            else if (Variable is FloatVariable) Data.floatData = new FloatData(float.Parse(Value, CultureInfo.InvariantCulture));
            else if (Variable is BooleanVariable) Data.booleanData = new BooleanData(bool.Parse(Value));
            else if (Variable is StringVariable) Data.stringData = new StringData(Value);
            return new AnyVariableAndDataPair { variable = Variable, data = Data };
        }

        private static T Load<T>(string Guid) where T : UnityEngine.Object => string.IsNullOrEmpty(Guid) ? null : AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(Guid));

        private static void Jump(Block Block, Block Target)
        {
            var Call = Add<Call>(Block);
            Set(Call, "targetBlock", Target);
            Set(Call, "callMode", CallMode.StopThenCall);
        }

        private static T Add<T>(Block Block) where T : Command
        {
            var Command = Block.gameObject.AddComponent<T>();
            Command.ParentBlock = Block;
            Command.ItemId = Block.GetFlowchart().NextItemId();
            Block.CommandList.Add(Command);
            return Command;
        }

        private static void Set(object Target, string Field, object Value) => StoryTemplateBuilder.Set(Target, Field, Value);

        #endregion
    }
}
