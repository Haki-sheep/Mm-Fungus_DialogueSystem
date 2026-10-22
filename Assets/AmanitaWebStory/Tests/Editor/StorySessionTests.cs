using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AmanitaWebStory.Tests
{
    public sealed class StorySessionTests
    {
        private readonly List<StoryAsset> assetList = new List<StoryAsset>();

        [TearDown]
        public void TearDown()
        {
            foreach (var Asset in assetList)
                UnityEngine.Object.DestroyImmediate(Asset);
            assetList.Clear();
        }

        #region 分支与状态

        [UnityTest]
        public IEnumerator BothBranchesAndReplayKeepAssetUnchanged() => UniTask.ToCoroutine(async () =>
        {
            var Asset = BranchStory();
            var First = new StorySession(Asset);
            var FirstView = new RecordingView { Selection = 0 };
            int Signals = 0;
            await First.RunAsync(FirstView, Event => { Assert.AreEqual("accepted", Event.EventId); Signals++; }, CancellationToken.None);
            Assert.AreEqual(1, First.Variables["trust"].Integer);
            CollectionAssert.AreEqual(new[] { "accepted_line" }, FirstView.LineList);
            CollectionAssert.AreEqual(new[] { "请进 1" }, FirstView.TextList);
            Assert.AreEqual(1, Signals);
            var Second = new StorySession(Asset);
            var SecondView = new RecordingView { Selection = 1 };
            await Second.RunAsync(SecondView, Event => Signals++, CancellationToken.None);
            Assert.AreEqual(0, Second.Variables["trust"].Integer);
            CollectionAssert.AreEqual(new[] { "declined_line" }, SecondView.LineList);
            Assert.AreEqual(1, Signals);
            Assert.AreEqual(0, Asset.Variables[0].Value.Integer);
            Assert.AreEqual(1, First.Variables["trust"].Integer);
        });

        [UnityTest]
        public IEnumerator ConcurrentSessionsHaveIndependentVariables() => UniTask.ToCoroutine(async () =>
        {
            var Asset = BranchStory();
            var First = new StorySession(Asset);
            var Second = new StorySession(Asset);
            await UniTask.WhenAll(
                First.RunAsync(new RecordingView { Selection = 0 }, Event => { }, CancellationToken.None),
                Second.RunAsync(new RecordingView { Selection = 1 }, Event => { }, CancellationToken.None));
            Assert.AreEqual(1, First.Variables["trust"].Integer);
            Assert.AreEqual(0, Second.Variables["trust"].Integer);
        });

        [Test]
        public void ValuesKeepTheirTypes()
        {
            Assert.AreEqual(3, StoryValue.Parse(eStoryDataType.Integer, "5").Apply(eStoryAssignment.Subtract, StoryValue.Parse(eStoryDataType.Integer, "2")).Integer);
            Assert.IsTrue(StoryValue.Parse(eStoryDataType.Float, "1.5").Compare(eStoryComparison.Greater, StoryValue.Parse(eStoryDataType.Float, "1")));
            Assert.IsFalse(StoryValue.Parse(eStoryDataType.Boolean, "true").Compare(eStoryComparison.Equal, StoryValue.Parse(eStoryDataType.Boolean, "false")));
            Assert.AreEqual("第二句", StoryValue.Parse(eStoryDataType.String, "第一句").Apply(eStoryAssignment.Assign, StoryValue.Parse(eStoryDataType.String, "第二句")).String);
            Assert.Throws<InvalidOperationException>(() => StoryValue.Parse(eStoryDataType.Integer, "1").Apply(eStoryAssignment.Assign, StoryValue.Parse(eStoryDataType.String, "1")));
        }

        #endregion

        #region 取消与失败

        [UnityTest]
        public IEnumerator CancellationInterruptsPendingChoice() => UniTask.ToCoroutine(async () =>
        {
            using (var Cancellation = new CancellationTokenSource())
            {
                var Session = new StorySession(BranchStory());
                var View = new RecordingView { WaitForChoice = true };
                var Pending = Session.RunAsync(View, Event => { }, Cancellation.Token);
                await UniTask.Yield();
                Cancellation.Cancel();
                Assert.IsTrue(await Pending.SuppressCancellationThrow());
                Assert.AreEqual(0, Session.Variables["trust"].Integer);
            }
        });

        [UnityTest]
        public IEnumerator DataOnlyLoopYieldsAndCanBeCancelled() => UniTask.ToCoroutine(async () =>
        {
            var Asset = AssetWith(new StoryAssignment("loop", "loop", "trust", eStoryAssignment.Add, StoryValue.Parse(eStoryDataType.Integer, "1")));
            using (var Cancellation = new CancellationTokenSource())
            {
                var Session = new StorySession(Asset);
                var Pending = Session.RunAsync(new RecordingView(), null, Cancellation.Token);
                await UniTask.Yield();
                Cancellation.Cancel();
                Assert.IsTrue(await Pending.SuppressCancellationThrow());
                Assert.Greater(Session.Variables["trust"].Integer, 0);
            }
        });

        [UnityTest]
        public IEnumerator InvalidChoiceIncludesStoryAndNode() => UniTask.ToCoroutine(async () =>
        {
            try
            {
                await new StorySession(BranchStory()).RunAsync(new RecordingView { Selection = 9 }, null, CancellationToken.None);
                Assert.Fail("无效选项应失败");
            }
            catch (InvalidOperationException Error)
            {
                StringAssert.Contains("test_story", Error.Message);
                StringAssert.Contains("choice", Error.Message);
                StringAssert.Contains("无效选项", Error.InnerException.Message);
            }
        });

        [UnityTest]
        public IEnumerator UnboundEventFailsWithContext() => UniTask.ToCoroutine(async () =>
        {
            try
            {
                await new StorySession(BranchStory()).RunAsync(new RecordingView(), null, CancellationToken.None);
                Assert.Fail("未接入业务事件应失败");
            }
            catch (InvalidOperationException Error)
            {
                StringAssert.Contains("signal", Error.Message);
                StringAssert.Contains("accepted", Error.InnerException.Message);
            }
        });

        [UnityTest]
        public IEnumerator SoundAndWaitReachEnd() => UniTask.ToCoroutine(async () =>
        {
            var Asset = AssetWith(new StorySound("sound", "wait", null, 0.5f, false), new StoryWait("wait", "end", 0), new StoryEnd("end"));
            var View = new RecordingView();
            var Session = new StorySession(Asset);
            await Session.RunAsync(View, null, CancellationToken.None);
            Assert.AreEqual(1, View.SoundCount);
            Assert.AreEqual("end", Session.CurrentNodeId);
        });

        #endregion

        #region 测试数据

        private StoryAsset BranchStory()
        {
            return AssetWith(
                new StoryChoice("choice", new List<StoryOptionData> { new StoryOptionData("同意", "add"), new StoryOptionData("拒绝", "check") }),
                new StoryAssignment("add", "check", "trust", eStoryAssignment.Add, StoryValue.Parse(eStoryDataType.Integer, "1")),
                new StoryCondition("check", "trust", eStoryComparison.Equal, StoryValue.Parse(eStoryDataType.Integer, "1"), "signal", "declined_line"),
                new StoryEvent("signal", "accepted_line", "accepted", StoryValue.Parse(eStoryDataType.None, "")),
                new StoryLine("accepted_line", "end", null, "请进 {$trust}"), new StoryLine("declined_line", "end", null, "再见"), new StoryEnd("end"));
        }

        private StoryAsset AssetWith(params StoryStep[] StepList)
        {
            var Asset = ScriptableObject.CreateInstance<StoryAsset>();
            Asset.SetContent("test_story", "测试", StepList[0].Id, "",
                new List<StoryInitialVariable> { new StoryInitialVariable("trust", StoryValue.Parse(eStoryDataType.Integer, "0")) }, new List<StoryStep>(StepList));
            assetList.Add(Asset);
            return Asset;
        }

        private sealed class RecordingView : IStoryView
        {
            public int Selection;
            public int SoundCount;
            public bool WaitForChoice;
            public readonly List<string> LineList = new List<string>();
            public readonly List<string> TextList = new List<string>();

            public UniTask ShowLineAsync(StoryLine Line, CancellationToken Token) { LineList.Add(Line.Id); TextList.Add(Line.Text); return UniTask.CompletedTask; }
            public UniTask<int> ChooseAsync(IReadOnlyList<StoryOptionData> Options, CancellationToken Token)
            {
                return WaitForChoice ? new UniTaskCompletionSource<int>().Task.AttachExternalCancellation(Token) : UniTask.FromResult(Selection);
            }
            public UniTask PlaySoundAsync(StorySound Sound, CancellationToken Token) { SoundCount++; return UniTask.CompletedTask; }
        }

        #endregion
    }
}
