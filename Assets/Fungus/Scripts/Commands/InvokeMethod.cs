using Fungus.Inspector;
using Cysharp.Threading.Tasks;
using System.Threading;
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using MarkerMetro.Unity.WinLegacy.Reflection;
using System.Linq;

namespace Fungus
{
    /// <summary>
    /// Invokes a method of a component via reflection. Supports passing multiple parameters and storing returned values in a Fungus variable.
    /// </summary>
    [CommandInfo("脚本与事件", "调用组件方法", "调用场景组件的方法 可传入参数 保存返回值或等待异步方法")]
    public class InvokeMethod : Command
    {
        [Tooltip("填写这段流程或对白的用途 只用于编辑时说明")]
        [LabelText("用途说明")]
        [SerializeField]
        protected string description = "";

        [Tooltip("设置本步骤使用的目标对象")]
        [LabelText("目标对象")]
        [SerializeField]
        protected GameObject targetObject;

        [HideInInspector]
        [Tooltip("设置本步骤使用的组件程序集名称")]
        [LabelText("组件程序集名称")]
        [SerializeField]
        protected string targetComponentAssemblyName;

        [HideInInspector]
        [Tooltip("设置本步骤使用的组件完整类型名")]
        [LabelText("组件完整类型名")]
        [SerializeField]
        protected string targetComponentFullname;

        [HideInInspector]
        [Tooltip("设置本步骤使用的组件显示名称")]
        [LabelText("组件显示名称")]
        [SerializeField]
        protected string targetComponentText;

        [HideInInspector]
        [Tooltip("设置本步骤使用的目标方法")]
        [LabelText("目标方法")]
        [SerializeField]
        protected string targetMethod;

        [HideInInspector]
        [Tooltip("设置本步骤使用的方法显示名称")]
        [LabelText("方法显示名称")]
        [SerializeField]
        protected string targetMethodText;

        [HideInInspector]
        [Tooltip("设置本步骤使用的方法参数")]
        [LabelText("方法参数")]
        [SerializeField]
        protected InvokeMethodParameter[] methodParameters;

        [HideInInspector]
        [Tooltip("设置本步骤使用的保存方法返回值")]
        [LabelText("保存方法返回值")]
        [SerializeField]
        protected bool saveReturnValue;

        [HideInInspector]
        [Tooltip("设置本步骤使用的接收返回值的变量")]
        [LabelText("接收返回值的变量")]
        [SerializeField]
        protected string returnValueVariableKey;

        [HideInInspector]
        [Tooltip("设置本步骤使用的返回值类型")]
        [LabelText("返回值类型")]
        [SerializeField]
        protected string returnValueType;

        [HideInInspector]
        [Tooltip("设置本步骤使用的显示继承的方法")]
        [LabelText("显示继承的方法")]
        [SerializeField]
        protected bool showInherited;

        [HideInInspector]
        [Tooltip("选择调用目标后当前流程如何继续 可等待目标结束 同时继续 或停止当前流程")]
        [LabelText("后续执行方式")]
        [SerializeField]
        protected CallMode callMode;

        protected Type componentType;
        protected Component objComponent;
        protected Type[] parameterTypes = null;
        protected MethodInfo objMethod;

        protected virtual void Awake()
        {
            try
            {
                PrepareTargets();
            }
            catch (Exception)
            {
                Debug.LogError("Rethrowing Exception thrown by:" + GetLocationIdentifier());
                throw;
            }
        }
        
        protected virtual void PrepareTargets()
        { 
            if (componentType == null)
            {
                componentType = ReflectionHelper.GetType(targetComponentAssemblyName);
            }

            if (objComponent == null)
            {
                objComponent = targetObject.GetComponent(componentType);
            }

            if (parameterTypes == null)
            {
                parameterTypes = GetParameterTypes();
            }

            if (objMethod == null)
            {
                objMethod = UnityEvent.GetValidMethodInfo(objComponent, targetMethod, parameterTypes);
            }
        }

        protected virtual async UniTask ExecuteMethodAsync(CancellationToken CancellationToken = default)
        {
            object Result = objMethod.Invoke(objComponent, GetParameterValues());
            if (Result is UniTask Operation)
                await Operation.AttachExternalCancellation(CancellationToken);
            else
                await ((IEnumerator)Result).ToUniTask(cancellationToken: CancellationToken);

            if (callMode == CallMode.WaitUntilFinished)
            {
                Continue();
            }
        }

        protected virtual System.Type[] GetParameterTypes()
        {
            System.Type[] types = new System.Type[methodParameters.Length];

            for (int i = 0; i < methodParameters.Length; i++)
            {
                var item = methodParameters[i];
                var objType = ReflectionHelper.GetType(item.objValue.typeAssemblyname);

                types[i] = objType;
            }

            return types;
        }

        protected virtual object[] GetParameterValues()
        {
            object[] values = new object[methodParameters.Length];
            var flowChart = GetFlowchart();

            for (int i = 0; i < methodParameters.Length; i++)
            {
                var item = methodParameters[i];

                if (string.IsNullOrEmpty(item.variableKey))
                {
                    values[i] = item.objValue.GetValue();
                }
                else
                {
                    object objValue = null;

                    switch (item.objValue.typeFullname)
                    {
                        case "System.Int32":
                            var intvalue = flowChart.GetVariable<IntegerVariable>(item.variableKey);
                            if (intvalue != null)
                                objValue = intvalue.Value;
                            break;
                        case "System.Boolean":
                            var boolean = flowChart.GetVariable<BooleanVariable>(item.variableKey);
                            if (boolean != null)
                                objValue = boolean.Value;
                            break;
                        case "System.Single":
                            var floatvalue = flowChart.GetVariable<FloatVariable>(item.variableKey);
                            if (floatvalue != null)
                                objValue = floatvalue.Value;
                            break;
                        case "System.String":
                            var stringvalue = flowChart.GetVariable<StringVariable>(item.variableKey);
                            if (stringvalue != null)
                                objValue = stringvalue.Value;
                            break;
                        case "UnityEngine.Color":
                            var color = flowChart.GetVariable<ColorVariable>(item.variableKey);
                            if (color != null)
                                objValue = color.Value;
                            break;
                        case "UnityEngine.GameObject":
                            var gameObject = flowChart.GetVariable<GameObjectVariable>(item.variableKey);
                            if (gameObject != null)
                                objValue = gameObject.Value;
                            break;
                        case "UnityEngine.Material":
                            var material = flowChart.GetVariable<MaterialVariable>(item.variableKey);
                            if (material != null)
                                objValue = material.Value;
                            break;
                        case "UnityEngine.Sprite":
                            var sprite = flowChart.GetVariable<SpriteVariable>(item.variableKey);
                            if (sprite != null)
                                objValue = sprite.Value;
                            break;
                        case "UnityEngine.Texture":
                            var texture = flowChart.GetVariable<TextureVariable>(item.variableKey);
                            if (texture != null)
                                objValue = texture.Value;
                            break;
                        case "UnityEngine.Vector2":
                            var vector2 = flowChart.GetVariable<Vector2Variable>(item.variableKey);
                            if (vector2 != null)
                                objValue = vector2.Value;
                            break;
                        case "UnityEngine.Vector3":
                            var vector3 = flowChart.GetVariable<Vector3Variable>(item.variableKey);
                            if (vector3 != null)
                                objValue = vector3.Value;
                            break;
                        default:
                            var obj = flowChart.GetVariable<ObjectVariable>(item.variableKey);
                            if (obj != null)
                                objValue = obj.Value;
                            break;
                    }

                    values[i] = objValue;
                }
            }

            return values;
        }

        protected virtual void SetVariable(string key, object value, string returnType)
        {
            var flowChart = GetFlowchart();

            switch (returnType)
            {
                case "System.Int32":
                    flowChart.GetVariable<IntegerVariable>(key).Value = (int)value;
                    break;
                case "System.Boolean":
                    flowChart.GetVariable<BooleanVariable>(key).Value = (bool)value;
                    break;
                case "System.Single":
                    flowChart.GetVariable<FloatVariable>(key).Value = (float)value;
                    break;
                case "System.String":
                    flowChart.GetVariable<StringVariable>(key).Value = (string)value;
                    break;
                case "UnityEngine.Color":
                    flowChart.GetVariable<ColorVariable>(key).Value = (UnityEngine.Color)value;
                    break;
                case "UnityEngine.GameObject":
                    flowChart.GetVariable<GameObjectVariable>(key).Value = (UnityEngine.GameObject)value;
                    break;
                case "UnityEngine.Material":
                    flowChart.GetVariable<MaterialVariable>(key).Value = (UnityEngine.Material)value;
                    break;
                case "UnityEngine.Sprite":
                    flowChart.GetVariable<SpriteVariable>(key).Value = (UnityEngine.Sprite)value;
                    break;
                case "UnityEngine.Texture":
                    flowChart.GetVariable<TextureVariable>(key).Value = (UnityEngine.Texture)value;
                    break;
                case "UnityEngine.Vector2":
                    flowChart.GetVariable<Vector2Variable>(key).Value = (UnityEngine.Vector2)value;
                    break;
                case "UnityEngine.Vector3":
                    flowChart.GetVariable<Vector3Variable>(key).Value = (UnityEngine.Vector3)value;
                    break;
                default:
                    flowChart.GetVariable<ObjectVariable>(key).Value = (UnityEngine.Object)value;
                    break;
            }
        }

        #region Public members

        /// <summary>
        /// GameObject containing the component method to be invoked.
        /// </summary>
        public virtual GameObject TargetObject { get { return targetObject; } }

        public override void OnEnter()
        {
            PrepareTargets();

            if (targetObject == null || string.IsNullOrEmpty(targetComponentAssemblyName) || string.IsNullOrEmpty(targetMethod))
            {
                Continue();
                return;
            }

            if (returnValueType != "System.Collections.IEnumerator" && returnValueType != typeof(UniTask).FullName)
            {
                var objReturnValue = objMethod.Invoke(objComponent, GetParameterValues());

                if (saveReturnValue)
                {
                    SetVariable(returnValueVariableKey, objReturnValue, returnValueType);
                }

                Continue();
            }
            else
            {
                asyncScope.Run(this, Token => ExecuteMethodAsync(Token)).Forget();

                if (callMode == CallMode.Continue)
                {
                    Continue();
                }
                else if (callMode == CallMode.Stop)
                {
                    StopParentBlock();
                }
            }
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override string GetSummary()
        {
            if (targetObject == null)
            {
                return "请先设置目标对象";
            }

            if (!string.IsNullOrEmpty(description))
            {
                return description;
            }

            return targetObject.name + "." + targetComponentText + "." + targetMethodText;
        }

        #endregion
    }

    [System.Serializable]
    public class InvokeMethodParameter
    {
        [LabelText("参数值")]
        [SerializeField]
        public ObjectValue objValue;

        [LabelText("变量名称")]
        [SerializeField]
        public string variableKey;
    }

    [System.Serializable]
    public class ObjectValue
    {
        [LabelText("参数类型的程序集")]
        public string typeAssemblyname;
        [LabelText("参数完整类型名")]
        public string typeFullname;

        [LabelText("整数值")]
        public int intValue;
        [LabelText("布尔值")]
        public bool boolValue;
        [LabelText("小数值")]
        public float floatValue;
        [LabelText("文字值")]
        public string stringValue;

        [LabelText("颜色值")]
        public Color colorValue;
        [LabelText("场景对象值")]
        public GameObject gameObjectValue;
        [LabelText("材质值")]
        public Material materialValue;
        [LabelText("对象值")]
        public UnityEngine.Object objectValue;
        [LabelText("精灵值")]
        public Sprite spriteValue;
        [LabelText("贴图值")]
        public Texture textureValue;
        [LabelText("二维向量值")]
        public Vector2 vector2Value;
        [LabelText("三维向量值")]
        public Vector3 vector3Value;

        public object GetValue()
        {
            switch (typeFullname)
            {
            case "System.Int32":
                return intValue;
            case "System.Boolean":
                return boolValue;
            case "System.Single":
                return floatValue;
            case "System.String":
                return stringValue;
            case "UnityEngine.Color":
                return colorValue;
            case "UnityEngine.GameObject":
                return gameObjectValue;
            case "UnityEngine.Material":
                return materialValue;
            case "UnityEngine.Sprite":
                return spriteValue;
            case "UnityEngine.Texture":
                return textureValue;
            case "UnityEngine.Vector2":
                return vector2Value;
            case "UnityEngine.Vector3":
                return vector3Value;
            default:
                var objType = ReflectionHelper.GetType(typeAssemblyname);

                if (objType.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    return objectValue;
                }
                else if (objType.IsEnum())
                    return System.Enum.ToObject(objType, intValue);

                break;
            }

            return null;
        }
    }

    public static class ReflectionHelper
    {
        static Dictionary<string, System.Type> types = new Dictionary<string, System.Type>();

        public static System.Type GetType(string AssemblyQualifiedNameTypeName)
        {
            if (types.ContainsKey(AssemblyQualifiedNameTypeName) && types[AssemblyQualifiedNameTypeName] != null)
                return types[AssemblyQualifiedNameTypeName];

            types[AssemblyQualifiedNameTypeName] = AppDomain.CurrentDomain.GetAssemblies().
                SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.AssemblyQualifiedName == AssemblyQualifiedNameTypeName);

            return types[AssemblyQualifiedNameTypeName];
        }
    }
}
