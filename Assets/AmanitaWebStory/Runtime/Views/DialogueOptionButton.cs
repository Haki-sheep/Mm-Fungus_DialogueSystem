using System;
using TMPro;
using UnityEngine;

namespace AmanitaWebStory
{
    public sealed class DialogueOptionButton : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Button button;
        [SerializeField]
        private TMP_Text label;

        private Action<int> select;
        private int index;

        private void Awake() { InitComponents(); }
        private void InitComponents() { button.onClick.AddListener(Select); }
        private void OnDestroy() { button.onClick.RemoveListener(Select); }
        private void Select() { select(index); }

        public void Bind(string Text, int Index, Action<int> Select)
        {
            label.text = Text;
            index = Index;
            select = Select;
        }
    }
}
