using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Fungus
{
    [AddComponentMenu("")]
    public sealed class FungusTweenGroup : MonoBehaviour
    {
        private readonly List<(Tween Tween, string Name)> tweenList = new List<(Tween, string)>();

        public bool IsAnimating => tweenList.Exists(Item => Item.Tween.isAlive);

        public Tween Track(Tween Tween, string Name)
        {
            tweenList.Add((Tween, Name));
            return Tween;
        }

        public void Stop(string Name = null, bool Complete = false)
        {
            var PendingList = new List<Tween>();
            for (int Index = tweenList.Count - 1; Index >= 0; Index--)
            {
                var Item = tweenList[Index];
                if (Name != null && Item.Name != Name) continue;
                tweenList.RemoveAt(Index);
                if (Item.Tween.isAlive) PendingList.Add(Item.Tween);
            }
            foreach (var Tween in PendingList)
            {
                if (Complete) Tween.Complete();
                else Tween.Stop();
            }
        }

        private void LateUpdate()
        {
            for (int Index = tweenList.Count - 1; Index >= 0; Index--)
                if (!tweenList[Index].Tween.isAlive) tweenList.RemoveAt(Index);
        }

        private void OnDisable() => Stop();
    }
}
