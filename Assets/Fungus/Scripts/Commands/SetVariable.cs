using Fungus.Inspector;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Sets a Boolean, Integer, Float or String variable to a new value using a simple arithmetic operation. The value can be a constant or reference another variable of the same type.
    /// </summary>
    [CommandInfo("变量", "修改变量", "给变量赋值 或执行加减乘除等运算")]
    [AddComponentMenu("")]
    public class SetVariable : Command, ISerializationCallbackReceiver
    {
        [LabelText("目标变量和值")]
        [SerializeField]
        protected AnyVariableAndDataPair anyVar = new AnyVariableAndDataPair();
        
        [Tooltip("设置本步骤使用的赋值方式")]
        [LabelText("赋值方式")]
        [SerializeField]
        protected SetOperator setOperator;
               
        protected virtual void DoSetOperation()
        {
            if (anyVar.variable == null)
            {
                return;
            }

            anyVar.SetOp(setOperator);
        }

        #region Public members

        /// <summary>
        /// The type of math operation to be performed.
        /// </summary>
        public virtual SetOperator _SetOperator { get { return setOperator; } }

        public override void OnEnter()
        {
            DoSetOperation();

            Continue();
        }

        public override string GetSummary()
        {
            if (anyVar.variable == null)
            {
                return "请先设置变量";
            }

            string description = anyVar.variable.Key;
            description += " " + VariableUtil.GetSetOperatorDescription(setOperator) + " ";
            description += anyVar.GetDataDescription();


            return description;
        }

        public override bool HasReference(Variable variable)
        {
            return anyVar.HasReference(variable);
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        #endregion



        #region Editor caches
#if UNITY_EDITOR
        protected override void RefreshVariableCache()
        {
            base.RefreshVariableCache();

            if(anyVar != null)
                anyVar.RefreshVariableCacheHelper(GetFlowchart(), ref referencedVariables);
        }
#endif
        #endregion Editor caches

        #region backwards compat


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

        public void OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (variable == null)
            {
                return;
            }
            else
            {
                anyVar.variable = variable;
            }

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

            //now converted to new AnyVar storage, remove the legacy.
            variable = null;
        }
        #endregion
    }
}
