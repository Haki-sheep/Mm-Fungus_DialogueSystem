using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// class for a single condition. A list of this is used for multiple conditions.
    /// </summary>
    [System.Serializable]
    public class ConditionExpression
    {
        [LabelText("比较方式")]
        [SerializeField]
        protected CompareOperator compareOperator;
        [LabelText("目标变量和值")]
        [SerializeField]
        protected AnyVariableAndDataPair anyVar;

        public virtual AnyVariableAndDataPair AnyVar { get { return anyVar; } }
        public virtual CompareOperator CompareOperator { get { return compareOperator; } }

        public ConditionExpression()
        {
        }

        public ConditionExpression(CompareOperator op, AnyVariableAndDataPair variablePair)
        {
            compareOperator = op;
            anyVar = variablePair;
        }
    }

    public abstract class VariableCondition : Condition, ISerializationCallbackReceiver
    {
        public enum AnyOrAll
        {
            [UnityEngine.InspectorName("任一满足")]
            AnyOf_OR,//Use as a chain of ORs
            [UnityEngine.InspectorName("全部满足")]
            AllOf_AND,//Use as a chain of ANDs
        }

        [Tooltip("选择所有条件都满足时成立 还是任一条件满足就成立")]
        [LabelText("条件满足方式")]
        [SerializeField]
        protected AnyOrAll anyOrAllConditions;

        [LabelText("判断条件")]
        [SerializeField]
        protected List<ConditionExpression> conditions = new List<ConditionExpression>();

        /// <summary>
        /// Called when the script is loaded or a value is changed in the
        /// inspector (Called in the editor only).
        /// </summary>
        public override void OnValidate()
        {
            base.OnValidate();

            if (conditions == null)
            {
                conditions = new List<ConditionExpression>();
            }

            if (conditions.Count == 0)
            {
                conditions.Add(new ConditionExpression());
            }
        }

        protected override bool EvaluateCondition()
        {
            if (conditions == null || conditions.Count == 0)
            {
                return false;
            }

            bool resultAny = false, resultAll = true;
            foreach (ConditionExpression condition in conditions)
            {
                bool curResult = false;
                if (condition.AnyVar == null)
                {
                    resultAll &= curResult;
                    resultAny |= curResult;
                    continue;
                }
                condition.AnyVar.Compare(condition.CompareOperator, ref curResult);
                resultAll &= curResult;
                resultAny |= curResult;
            }

            if (anyOrAllConditions == AnyOrAll.AnyOf_OR) return resultAny;

            return resultAll;
        }

        protected override bool HasNeededProperties()
        {
            if (conditions == null || conditions.Count == 0)
            {
                return false;
            }

            foreach (ConditionExpression condition in conditions)
            {
                if (condition.AnyVar == null || condition.AnyVar.variable == null)
                {
                    return false;
                }
            }
            return true;
        }
        
        public override string GetSummary()
        {
            if (!this.HasNeededProperties())
            {
                return "请先设置变量";
            }

            string connector = "";
            if (anyOrAllConditions == AnyOrAll.AnyOf_OR)
            {
                connector = " <b>或者</b> ";
            }
            else
            {
                connector = " <b>并且</b> ";
            }

            StringBuilder summary = new StringBuilder("");
            for (int i = 0; i < conditions.Count; i++)
            {
                summary.Append(conditions[i].AnyVar.variable.Key + " " +
                               VariableUtil.GetCompareOperatorDescription(conditions[i].CompareOperator) + " " +
                               conditions[i].AnyVar.GetDataDescription());

                if (i < conditions.Count - 1)
                {
                    summary.Append(connector);
                }
            }
            return AuthoringValueLabel.Format(summary);
        }

        public override bool HasReference(Variable variable)
        {
            return anyVar.HasReference(variable);
        }



        #region Editor caches
#if UNITY_EDITOR
        protected override void RefreshVariableCache()
        {
            base.RefreshVariableCache();

            if (conditions != null)
            {
                foreach (var item in conditions)
                {
                    item.AnyVar.RefreshVariableCacheHelper(GetFlowchart(), ref referencedVariables);
                }
            }
        }
#endif
        #endregion Editor caches

        #region backwards compat

        [HideInInspector]
        [LabelText("比较方式")]
        [SerializeField]
        protected CompareOperator compareOperator;

        [HideInInspector]
        [LabelText("目标变量和值")]
        [SerializeField]
        protected AnyVariableAndDataPair anyVar;

        [Tooltip("设置本步骤使用的使用的变量")]
        [VariableProperty(AllVariableTypes.VariableAny.Any)]
        [LabelText("使用的变量")]
        [SerializeField]
        protected Variable variable;

        [Tooltip("设置本步骤使用的布尔值值")]
        [LabelText("布尔值")]
        [SerializeField]
        protected BooleanData booleanData;

        [Tooltip("设置本步骤使用的整数值")]
        [LabelText("整数值")]
        [SerializeField]
        protected IntegerData integerData;

        [Tooltip("设置本步骤使用的小数值")]
        [LabelText("小数值")]
        [SerializeField]
        protected FloatData floatData;

        [Tooltip("设置本步骤使用的文字值")]
        [LabelText("文字值")]
        [SerializeField]
        protected StringDataMulti stringData;

        [Tooltip("设置本步骤使用的动画控制器值")]
        [LabelText("动画控制器值")]
        [SerializeField]
        protected AnimatorData animatorData;

        [Tooltip("设置本步骤使用的音频播放器值")]
        [LabelText("音频播放器值")]
        [SerializeField]
        protected AudioSourceData audioSourceData;

        [Tooltip("设置本步骤使用的颜色值")]
        [LabelText("颜色值")]
        [SerializeField]
        protected ColorData colorData;

        [Tooltip("设置本步骤使用的场景对象值")]
        [LabelText("场景对象值")]
        [SerializeField]
        protected GameObjectData gameObjectData;

        [Tooltip("设置本步骤使用的材质值")]
        [LabelText("材质值")]
        [SerializeField]
        protected MaterialData materialData;

        [Tooltip("设置本步骤使用的对象值")]
        [LabelText("对象值")]
        [SerializeField]
        protected ObjectData objectData;

        [Tooltip("设置本步骤使用的二维刚体值")]
        [LabelText("二维刚体值")]
        [SerializeField]
        protected Rigidbody2DData rigidbody2DData;

        [Tooltip("设置本步骤使用的精灵值")]
        [LabelText("精灵值")]
        [SerializeField]
        protected SpriteData spriteData;

        [Tooltip("设置本步骤使用的贴图值")]
        [LabelText("贴图值")]
        [SerializeField]
        protected TextureData textureData;

        [Tooltip("设置本步骤使用的空间变换值")]
        [LabelText("空间变换值")]
        [SerializeField]
        protected TransformData transformData;

        [Tooltip("设置本步骤使用的二维向量值")]
        [LabelText("二维向量值")]
        [SerializeField]
        protected Vector2Data vector2Data;

        [Tooltip("设置本步骤使用的三维向量值")]
        [LabelText("三维向量值")]
        [SerializeField]
        protected Vector3Data vector3Data;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (variable != null)
            {
                anyVar.variable = variable;

                if (variable.GetType() == typeof(BooleanVariable) && !booleanData.Equals(new BooleanData()))
                {
                    anyVar.data.booleanData = booleanData;
                    booleanData = new BooleanData();
                }
                else if (variable.GetType() == typeof(IntegerVariable) && !integerData.Equals(new IntegerData()))
                {
                    anyVar.data.integerData = integerData;
                    integerData = new IntegerData();
                }
                else if (variable.GetType() == typeof(FloatVariable) && !floatData.Equals(new FloatData()))
                {
                    anyVar.data.floatData = floatData;
                    floatData = new FloatData();
                }
                else if (variable.GetType() == typeof(StringVariable) && !stringData.Equals(new StringDataMulti()))
                {
                    anyVar.data.stringData.stringRef = stringData.stringRef;
                    anyVar.data.stringData.stringVal = stringData.stringVal;
                    stringData = new StringDataMulti();
                }
                else if (variable.GetType() == typeof(AnimatorVariable) && !animatorData.Equals(new AnimatorData()))
                {
                    anyVar.data.animatorData = animatorData;
                    animatorData = new AnimatorData();
                }
                else if (variable.GetType() == typeof(AudioSourceVariable) && !audioSourceData.Equals(new AudioSourceData()))
                {
                    anyVar.data.audioSourceData = audioSourceData;
                    audioSourceData = new AudioSourceData();
                }
                else if (variable.GetType() == typeof(ColorVariable) && !colorData.Equals(new ColorData()))
                {
                    anyVar.data.colorData = colorData;
                    colorData = new ColorData();
                }
                else if (variable.GetType() == typeof(GameObjectVariable) && !gameObjectData.Equals(new GameObjectData()))
                {
                    anyVar.data.gameObjectData = gameObjectData;
                    gameObjectData = new GameObjectData();
                }
                else if (variable.GetType() == typeof(MaterialVariable) && !materialData.Equals(new MaterialData()))
                {
                    anyVar.data.materialData = materialData;
                    materialData = new MaterialData();
                }
                else if (variable.GetType() == typeof(ObjectVariable) && !objectData.Equals(new ObjectData()))
                {
                    anyVar.data.objectData = objectData;
                    objectData = new ObjectData();
                }
                else if (variable.GetType() == typeof(Rigidbody2DVariable) && !rigidbody2DData.Equals(new Rigidbody2DData()))
                {
                    anyVar.data.rigidbody2DData = rigidbody2DData;
                    rigidbody2DData = new Rigidbody2DData();
                }
                else if (variable.GetType() == typeof(SpriteVariable) && !spriteData.Equals(new SpriteData()))
                {
                    anyVar.data.spriteData = spriteData;
                    spriteData = new SpriteData();
                }
                else if (variable.GetType() == typeof(TextureVariable) && !textureData.Equals(new TextureData()))
                {
                    anyVar.data.textureData = textureData;
                    textureData = new TextureData();
                }
                else if (variable.GetType() == typeof(TransformVariable) && !transformData.Equals(new TransformData()))
                {
                    anyVar.data.transformData = transformData;
                    transformData = new TransformData();
                }
                else if (variable.GetType() == typeof(Vector2Variable) && !vector2Data.Equals(new Vector2Data()))
                {
                    anyVar.data.vector2Data = vector2Data;
                    vector2Data = new Vector2Data();
                }
                else if (variable.GetType() == typeof(Vector3Variable) && !vector3Data.Equals(new Vector3Data()))
                {
                    anyVar.data.vector3Data = vector3Data;
                    vector3Data = new Vector3Data();
                }

                //moved to new anyvar storage, clear legacy.
                variable = null;
            }

            // just checking for anyVar != null fails here. is any var being reintilaized somewhere?

            if (anyVar != null && anyVar.variable != null)
            {
                ConditionExpression c = new ConditionExpression(compareOperator, anyVar);
                if (!conditions.Contains(c))
                {
                    conditions.Add(c);
                }

                anyVar = null;
                variable = null;
            }
        }

        #endregion backwards compat
    }
}