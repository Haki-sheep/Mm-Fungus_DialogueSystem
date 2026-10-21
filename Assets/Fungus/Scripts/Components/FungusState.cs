using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Used by the Flowchart window to serialize the currently active Flowchart object
    /// so that the same Flowchart can be displayed while editing & playing.
    /// </summary>
    [AddComponentMenu("")]
    public class FungusState : MonoBehaviour
    {
        [LabelText("当前选中流程图")]
        [SerializeField]
        protected Flowchart selectedFlowchart;

        #region Public members

        /// <summary>
        /// The currently selected Flowchart.
        /// </summary>
        public virtual Flowchart SelectedFlowchart { get { return selectedFlowchart; } set { selectedFlowchart = value; } }

        #endregion
    }
}