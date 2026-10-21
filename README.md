# Mm-Fungus DialogueSystem — Unity 对话系统工程

基于 [Amanita / Fungus](https://github.com/Atelier-Mycelia/Amanita) 修改的中文对话工具。策划在网页里搭剧情，回填成 Unity 预制体；AI 通过随模块提供的 CLI 读写同一份数据。

工程使用 Unity **6000.3.26f1**。可直接使用源码工程，或导出 `.unitypackage` 导入自己的工程。核心包的本地输出目录是 `Releases/`；菜单 **Tools → Fungus → 导出核心包（含网页和 CLI）** 可重新导出。

## 有什么用

- 八类剧情卡片：对白、玩家选择、条件判断、修改变量、等待、事件、音效、结束。
- 网页支持多段剧情管理、拖线连接、复制剪切粘贴、撤销重做、黑白主题和分支试读。
- 一键回填生成 Fungus 流程预制体；自定义事件自动生成 SO，读取和更新使用版本号防止覆盖新数据。
- 原生流程使用 UniTask，动画统一使用 PrimeTween；开发界面保留中文，运行不依赖 Odin 或 Lua。
- 保留对话、流程、立绘、镜头与音频核心；去掉官方示例、自制演示和一批通用集合、数学、属性命令。没有附带演示关卡。

事件卡片负责声明和发送事件。它不会自动实现“发任务、给物品”等玩法；已有组件可用 `StorySignalReceiver` 绑定 UnityEvent，新玩法仍需程序实现接收逻辑。AI 使用 JSON 契约，不会在回填时偷偷生成或执行 C#。

## 怎么用

1. 克隆仓库，用 Unity Hub 打开根目录，等待依赖安装和编译完成。
2. 点击 **Tools → Fungus → 打开网页剧情编辑器**。新建剧情、编辑卡片并连线，再点“回填 Unity”。
3. 把 `Assets/AmanitaWebStory/Generated/` 中生成的剧情预制体拖进自己的场景，运行即可播放。`SampleScene` 只是原有空场景。
4. 角色、立绘和对白界面在 `Assets/AmanitaWebStory/StoryTemplate.prefab` 配置。网页试读只模拟文本和变量，事件与音效的实际效果请在 Unity 验证。

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

完整命令为 `capabilities / list / read / validate / import / delete / audit / status`。更新和删除必须带 `read` 返回的 `-Revision`；删除进入回收目录。数据结构见随模块提供的 `CLI/story.schema.json`。运行中的 Unity 不接受回填和删除。

源码保留 Fungus 命名空间和已有资源 GUID；部分历史命令类名仍带 Lean/iTween 字样，执行已改成 PrimeTween，不含旧补间库。上游快照为 `71f7bea8e2d6842cefd8a84d47d9f86506637e6a`。Fungus 许可见 [LICENSE](Assets/Fungus/LICENSE.txt)，字体许可见 [OFL](Assets/AmanitaWebStory/Fonts/OFL.txt)，PrimeTween 包内保留其原始许可。
