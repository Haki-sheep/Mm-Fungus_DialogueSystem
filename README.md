# Mm-Fungus DialogueSystem — Unity 对话系统工程

基于 [Amanita / Fungus](https://github.com/Atelier-Mycelia/Amanita) 修改的中文对话工具。策划在网页里搭剧情，回填成 Unity 剧情 SO；AI 通过随模块提供的 CLI 读写同一份数据。

工程使用 Unity **6000.3.27f1**。可直接使用源码工程，或导出 `.unitypackage` 导入自己的工程。核心包的本地输出目录是 `Releases/`；菜单 **Tools → MmFungus → 导出核心包** 可重新导出。

## 有什么用

- 八类剧情卡片：对白、玩家选择、条件判断、修改变量、等待、事件、音效、结束
- 网页支持多段剧情管理 拖线连接 复制剪切粘贴 撤销重做 黑白主题和分支试读
- 一键回填生成剧情 SO 读取和更新使用版本号防止覆盖新数据
- 使用 UniTask 动画统一使用 PrimeTween 开发界面保留中文 运行不依赖 Odin 或 Lua
- 保留对话 流程 立绘 镜头与音频核心 去掉官方示例和一批通用集合 数学 属性命令 源码工程另含数据与表现分离的试用示例 默认核心发布包不包含示例与测试

事件卡片负责声明和发送事件 不会自动实现 发任务 给物品 等玩法 程序订阅 `StoryPlayer.EventRaised` 处理 AI 使用 JSON 契约 不会在回填时偷偷生成或执行 C#

## 怎么用

1. 克隆仓库 用 Unity Hub 打开根目录 等待依赖安装和编译完成
2. 点击 **Tools → MmFungus → 打开网页剧情编辑器** 新建剧情 编辑卡片并连线 再点 回填 Unity
3. 把 `Assets/AmanitaWebStory/Stories/` 中生成的剧情 SO 拖到场景中 `StoryPlayer` 的剧情字段 Play 即可播放 `SampleScene` 只是原有空场景
4. 角色立绘在 `Assets/AmanitaWebStory/` 下新建 `StoryActor` SO 配置 右键菜单 **创建 → 剧情 → 角色** 网页试读只模拟文本和变量 事件与音效的实际效果请在 Unity 验证

只导入发布包时，先在 Package Manager 安装依赖，再导入 `.unitypackage`：

- **UniTask 2.5.10**：通过 Git URL 添加 `https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask#2.5.10`。
- **PrimeTween 1.5.0**：按[作者说明](https://github.com/KyryloKuzyk/PrimeTween#install-via-unity-package-manager-upm)添加 Scoped Registry，名称 `npmjs`、地址 `https://registry.npmjs.org`、范围 `com.kyrylokuzyk`，再按名称安装 `com.kyrylokuzyk.primetween` 的 1.5.0 版。仓库工程已配置该依赖；依据许可，发布包不附带 PrimeTween 源码或 tarball。
- **Newtonsoft Json 3.2.2**：按名称添加 `com.unity.nuget.newtonsoft-json`；宿主同时需要 Unity UI 和 Input System。Unity 2022 的宿主另需 TextMeshPro 3.x；本发布包以 Unity 6.3 验证。

已有同名依赖时复用，不要导入第二份。默认字体是 Noto Sans SC 的常用简体字子集，约 2.3 MB；生僻字和其他语言请换成项目自己的完整字体。

AI 或脚本调用前，先在目标 Unity 启动上述网页服务。CLI 随模块一起导入，不需要安装 Codex 或额外的 Unity 控制桥。在工程根目录执行：

```powershell
pwsh -File Assets/AmanitaWebStory/CLI/story.ps1 capabilities
pwsh -File Assets/AmanitaWebStory/CLI/story.ps1 validate -File story.json
pwsh -File Assets/AmanitaWebStory/CLI/story.ps1 import -File story.json
pwsh -File Assets/AmanitaWebStory/CLI/story.ps1 read -Id story_id
```

完整命令为 `capabilities / list / read / validate / import / delete / audit / status` 更新和删除必须带 `read` 返回的 `-Revision` 删除进入回收目录 数据结构见随模块提供的 `CLI/story.schema.json` 运行中的 Unity 不接受回填和删除

服务启动后会在脚本编译重载结束时恢复同一网页地址 菜单手动停止后 重载不会重新启动服务

## 数据与表现分离试用

这是新版默认流程 独立执行扩展 试用入口是 **Tools → MmFungus → 打开分离试用示例** 首次使用自动生成场景和资源 进入 Play 后点击 对白界面 或 通讯界面 同一份城门剧情使用不同 UI 播放中切换会从头开始 点击 停止 取消本次播放 两个答案分别走入城和离开分支 信任值每次从 0 开始 允许通行时业务脚本接收事件

从自己的网页剧情开始：

1. 网页回填后 在 Project 中选中生成的剧情 SO 可直接用于播放
2. 场景放一个 `StoryPlayer` 把剧情 SO 和场景中的 `DialogueView` 拖入 Inspector 试用场景中的两个 View Prefab 可直接复用 场景统一提供 EventSystem
3. 勾选自动播放 或把按钮绑定到 `StoryPlayer.Play()` 程序使用 `await Player.PlayAsync(Story, Token)` 临时指定另一界面使用 `await Player.PlayAsync(Story, View, Token)`
4. 业务订阅 `Player.EventRaised` 并在生命周期结束时退订 示例接入代码见 `Assets/AmanitaWebStory/Samples/Separation/StorySeparationDemo.cs`

```csharp
private async UniTask TalkAsync(StoryAsset Story, CancellationToken Token)
{
    await player.PlayAsync(Story, Token);
    int Trust = player.Session.Variables["trust"].Integer;
}
```

`Stop()`、外部取消、禁用播放器或禁用 View 都会取消播放并清理界面；异步调用通过取消异常通知调用者。重播须等原调用结束；同时使用同一播放器或同一 View 会明确报错。需要并行播放时配置独立的播放器和 View，剧情 SO 可以共享。结束后 `Session` 保留最后一次结果供读取，下一次播放创建新状态，不修改 SO。

源码阅读顺序：

| 位置 | 负责什么 |
|---|---|
| `Runtime/Data/StoryAsset.cs` `StoryStep.cs` `StoryActor.cs` | 剧情 初始值 角色立绘 图片与音频内容引用 不引用界面 GameObject 或播放器 |
| `Runtime/Playback/StoryPlayer.cs` | 对外播放入口 取消 界面占用和清理 |
| `Runtime/Playback/StorySession.cs` | 八类节点执行 变量 分支 文本变量替换 |
| `Runtime/Playback/DialogueView.cs` | 界面生命周期和异步显示约定 |
| `Runtime/Views/UguiDialogueView.cs` | 通过 Fungus `Writer` 打字 继续 动态选项 多人立绘站位与音频播放 |
| `Runtime/Views/PortraitSlot.cs` | 单个立绘站位 显示角色 Sprite 与说话高亮 |

Data Playback Views 用程序集单向约束依赖 更换布局只改 View Prefab 改为其他 UI 框架时继承 `DialogueView` 不修改剧情和执行器 后台测试可直接给 `StorySession` 传入 `IStoryView`

当前边界 支持网页八类节点 `{$变量}` TMP 富文本与 Fungus `Writer` 文字标签 背景是剧情 SO 上配置的单张内容图 同屏立绘数量等于 View 上的站位数 满员时替换最久未说话的角色 尚无背景切换节点 存档或本地化 网页选项上限为 6

源码保留 Fungus 命名空间和已有资源 GUID；部分历史命令类名仍带 Lean/iTween 字样，执行已改成 PrimeTween，不含旧补间库。上游快照为 `71f7bea8e2d6842cefd8a84d47d9f86506637e6a`。Fungus 许可见 [LICENSE](Assets/Fungus/LICENSE.txt)，字体许可见 [OFL](Assets/AmanitaWebStory/Fonts/OFL.txt)，PrimeTween 包内保留其原始许可。
