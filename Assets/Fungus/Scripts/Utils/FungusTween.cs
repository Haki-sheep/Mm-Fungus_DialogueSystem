using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    public static class FungusTween
    {
        public static Tween Track(GameObject Owner, Tween Tween, string Name = "")
        {
            var Group = Owner.GetComponent<FungusTweenGroup>();
            if (Group == null) Group = Owner.AddComponent<FungusTweenGroup>();
            return Group.Track(Tween, Name);
        }

        public static Tween Value(GameObject Owner, float Start, float End, float Duration, Action<float> SetValue, eTweenEase Ease = eTweenEase.linear)
        {
            return Track(Owner, Tween.Custom(Owner, Start, End, Duration, (Target, Value) => SetValue(Value), TweenEasing.Get(Ease)));
        }

        public static Tween Move(GameObject Owner, Vector3 End, float Duration, eTweenEase Ease = eTweenEase.linear, bool Local = false, int Cycles = 1, CycleMode Mode = CycleMode.Restart)
        {
            return Track(Owner, Local
                ? Tween.LocalPosition(Owner.transform, End, Duration, TweenEasing.Get(Ease), Cycles, Mode)
                : Tween.Position(Owner.transform, End, Duration, TweenEasing.Get(Ease), Cycles, Mode));
        }

        public static Tween Rotate(GameObject Owner, Vector3 End, float Duration, eTweenEase Ease = eTweenEase.linear, bool Local = false, int Cycles = 1, CycleMode Mode = CycleMode.Restart)
        {
            return Track(Owner, Local
                ? Tween.LocalEulerAngles(Owner.transform, Owner.transform.localEulerAngles, End, Duration, TweenEasing.Get(Ease), Cycles, Mode)
                : Tween.EulerAngles(Owner.transform, Owner.transform.eulerAngles, End, Duration, TweenEasing.Get(Ease), Cycles, Mode));
        }

        public static Tween Scale(GameObject Owner, Vector3 End, float Duration, eTweenEase Ease = eTweenEase.linear, int Cycles = 1, CycleMode Mode = CycleMode.Restart)
        {
            return Track(Owner, Tween.Scale(Owner.transform, End, Duration, TweenEasing.Get(Ease), Cycles, Mode));
        }

        public static Tween Alpha(RectTransform Target, float End, float Duration, eTweenEase Ease = eTweenEase.linear)
        {
            return Track(Target.gameObject, Tween.Alpha(Target.GetComponent<Graphic>(), End, Duration, TweenEasing.Get(Ease)));
        }

        public static Tween Color(RectTransform Target, Color End, float Duration, eTweenEase Ease = eTweenEase.linear)
        {
            return Track(Target.gameObject, Tween.Color(Target.GetComponent<Graphic>(), End, Duration, TweenEasing.Get(Ease)));
        }

        public static void Stop(GameObject Owner, bool Complete = false) => Owner.GetComponent<FungusTweenGroup>()?.Stop(Complete: Complete);
        public static bool IsAnimating(GameObject Owner) => Owner.GetComponent<FungusTweenGroup>()?.IsAnimating == true;

        public static void StopAll(string Name = null)
        {
            foreach (var Group in UnityEngine.Object.FindObjectsOfType<FungusTweenGroup>()) Group.Stop(Name);
        }

        public static void Finish(ref Tween Handle)
        {
            var Previous = Handle;
            Handle = default;
            if (Previous.isAlive) Previous.Complete();
        }
    }
}
