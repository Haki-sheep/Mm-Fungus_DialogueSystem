using Fungus.Inspector;
using Cysharp.Threading.Tasks;
using System.Threading;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using PrimeTween;

namespace Fungus
{
    /// <summary>
    /// The type of audio control to perform.
    /// </summary>
    public enum ControlAudioType
    {
        /// <summary> Play the audiosource once. </summary>
        [UnityEngine.InspectorName("播放一次")]
        PlayOnce,
        /// <summary> Play the audiosource in a loop. </summary>
        [UnityEngine.InspectorName("循环播放")]
        PlayLoop,
        /// <summary> Pause a looping audiosource. </summary>
        [UnityEngine.InspectorName("暂停循环音频")]
        PauseLoop,
        /// <summary> Stop a looping audiosource. </summary>
        [UnityEngine.InspectorName("停止循环音频")]
        StopLoop,
        /// <summary> Change the volume level of an audiosource. </summary>
        [UnityEngine.InspectorName("修改音量")]
        ChangeVolume
    }

    /// <summary>
    /// Plays, loops, or stops an audiosource. Any AudioSources with the same tag as the target Audio Source will automatically be stoped.
    /// </summary>
    [CommandInfo("声音", "控制声音", "对配置的目标执行控制声音 使用下方参数设置具体行为")]
    [ExecuteInEditMode]
    public class ControlAudio : Command
    {
        [Tooltip("设置本步骤使用的控制方式")]
        [LabelText("控制方式")]
        [SerializeField]
        protected ControlAudioType control;
        public virtual ControlAudioType Control { get { return control; } }

        [Tooltip("设置本步骤使用的音频播放器")]
        [LabelText("音频播放器")]
        [SerializeField]
        protected AudioSourceData _audioSource;

        [Range(0,1)]
        [Tooltip("设置本步骤使用的起始音量")]
        [LabelText("起始音量")]
        [SerializeField]
        protected float startVolume = 1;

        [Range(0,1)]
        [Tooltip("设置本步骤使用的结束音量")]
        [LabelText("结束音量")]
        [SerializeField]
        protected float endVolume = 1;
        
        [Tooltip("设置本步骤使用的淡入淡出时间（秒）")]
        [LabelText("淡入淡出时间（秒）")]
        [SerializeField]
        protected float fadeDuration; 

        [Tooltip("勾选后等待本步骤完成再继续 不勾选则立即执行下一步")]
        [LabelText("等完成后再执行下一步")]
        [SerializeField]
        protected bool waitUntilFinished = false;

        // If there's other music playing in the scene, assign it the same tag as the new music you want to play and
        // the old music will be automatically stopped.
        protected virtual void StopAudioWithSameTag()
        {
            // Don't stop audio if there's no tag assigned
            if (_audioSource.Value == null ||
                _audioSource.Value.tag == "Untagged")
            {
                return;
            }

        #if UNITY_6000
            var audioSources = GameObject.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        #else
            var audioSources = GameObject.FindObjectsOfType<AudioSource>();
        #endif
        
            for (int i = 0; i < audioSources.Length; i++)
            {
                var a = audioSources[i];
                if (a != _audioSource.Value && a.tag == _audioSource.Value.tag)
                {
                    StopLoop(a);
                }
            }
        }

        protected virtual void PlayOnce() 
        {
            var Source = _audioSource.Value;
            if (fadeDuration > 0)
            {
                // Fade volume in
                TrackTween(FungusTween.Track(Source.gameObject,
                    Tween.AudioVolume(Source, endVolume, fadeDuration)));
            }
            Source.PlayOneShot(Source.clip);
            if (waitUntilFinished)
                asyncScope.Run(this, Token => WaitAndContinueAsync(Token)).Forget();
        }

        protected virtual async UniTask WaitAndContinueAsync(CancellationToken CancellationToken = default)
        {
            // Poll the audiosource until playing has finished
            // This allows for things like effects added to the audiosource.
            while (_audioSource.Value.isPlaying)
            {
                await UniTask.NextFrame(cancellationToken: CancellationToken);
            }

            Continue();
        }

        protected virtual void PlayLoop()
        {
            var Source = _audioSource.Value;
            Source.loop = true;
            Source.Play();
            if (fadeDuration > 0)
            {
                var Complete = waitUntilFinished ? CreateContinueCallback() : (System.Action)null;
                TrackTween(FungusTween.Track(Source.gameObject,
                    Tween.AudioVolume(Source, endVolume, fadeDuration))).OnComplete(Complete);
            }
            else
            {
                Source.volume = endVolume;
                AudioFinished();
            }
        }

        protected virtual void PauseLoop()
        {
            var Source = _audioSource.Value;
            if (fadeDuration > 0)
            {
                var Complete = waitUntilFinished ? CreateContinueCallback() : (System.Action)null;
                TrackTween(FungusTween.Track(Source.gameObject,
                    Tween.AudioVolume(Source, 0f, fadeDuration))).OnComplete(() =>
                    {
                        Source.Pause();
                        Complete?.Invoke();
                    });
            }
            else
            {
                Source.Pause();
                AudioFinished();
            }
        }

        protected virtual void StopLoop(AudioSource source)
        {
            bool IsPrimarySource = source == _audioSource.Value;
            if (fadeDuration > 0)
            {
                var Complete = waitUntilFinished && IsPrimarySource ? CreateContinueCallback() : (System.Action)null;
                TrackTween(FungusTween.Track(source.gameObject,
                    Tween.AudioVolume(source, 0f, fadeDuration))).OnComplete(() =>
                    {
                        source.Stop();
                        Complete?.Invoke();
                    });
            }
            else
            {
                source.Stop();
                if (IsPrimarySource) AudioFinished();
            }
        }

        protected virtual void ChangeVolume()
        {
            var Source = _audioSource.Value;
            var Complete = waitUntilFinished ? CreateContinueCallback() : (System.Action)null;
            TrackTween(FungusTween.Track(Source.gameObject,
                Tween.AudioVolume(Source, endVolume, fadeDuration))).OnComplete(Complete);
        }

        protected virtual void AudioFinished()
        {
            if (waitUntilFinished)
            {
                Continue();
            }
        }

        #region Public members

        public override void OnEnter()
        {
            if (_audioSource.Value == null)
            {
                Continue();
                return;
            }

            if (control == ControlAudioType.PlayOnce || control == ControlAudioType.PlayLoop)
                _audioSource.Value.volume = fadeDuration > 0f ? startVolume : endVolume;

            switch(control)
            {
                case ControlAudioType.PlayOnce:
                    StopAudioWithSameTag();
                    PlayOnce();
                    break;
                case ControlAudioType.PlayLoop:
                    StopAudioWithSameTag();
                    PlayLoop();
                    break;
                case ControlAudioType.PauseLoop:
                    PauseLoop();
                    break;
                case ControlAudioType.StopLoop:
                    StopLoop(_audioSource.Value);
                    break;
                case ControlAudioType.ChangeVolume:
                    ChangeVolume(); 
                    break;
            }
            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            if (_audioSource.Value == null)
            {
                return "请先设置音效片段";
            }
            string fadeType = "";
            if (fadeDuration > 0)
            {
                fadeType = " 淡出";
                if (control != ControlAudioType.StopLoop)
                {
                    fadeType = " 音量渐变到 " + endVolume;
                }
                if (control == ControlAudioType.ChangeVolume)
                {
                    fadeType = " 到 " + endVolume;
                }
                fadeType += " 持续 " + fadeDuration + " 秒";
            }
            return AuthoringValueLabel.Format(control) + " \"" + _audioSource.Value.name + "\"" + fadeType;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _audioSource.audioSourceRef == variable || base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("audioSource")] public AudioSource audioSourceOLD;

        protected virtual void OnEnable()
        {
            if (audioSourceOLD != null)
            {
                _audioSource.Value = audioSourceOLD;
                audioSourceOLD = null;
            }
        }

        #endregion
    }    
}