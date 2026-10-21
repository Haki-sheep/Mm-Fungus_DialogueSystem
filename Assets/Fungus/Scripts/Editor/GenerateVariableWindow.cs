using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Fungus.EditorUtils
{
    /// <summary>
    /// Adds window that generates the require scripts to create a new FungusVariable that wraps an existing type. 
    /// 
    /// These can then be used in the fungus flowcharts. It also generates a *Property command to allow Gets and Sets
    /// on all the elements of that variable that Fungus Understands. The Quaternion and Matrix4x4 have been auto
    /// generated and then auto formatted in visual studio and set to preview only as examples of it's use. 
    /// 
    /// It can be used to help building variable wrappers for builtin Unity types or your own components or classes.
    /// 
    /// To add new types see the VariableScriptGenerator constructor.
    /// </summary>
    public class GenerateVariableWindow : EditorWindow
    {
        private VariableScriptGenerator generator = new VariableScriptGenerator();
        private string userInputClassName = "";
        private List<Type> typeList = new List<Type>();

        public void OnGUI()
        {
            DrawMenuPanel();
        }

        private void DrawMenuPanel()
        {
            EditorGUI.BeginChangeCheck();

            if(GUILayout.Button("按列表全部生成"))
            {
                foreach (var item in VariableScriptGenerator.AllGeneratedVariableTypeClassNames)
                {
                    generator.TargetType = item;
                    generator.Generate();

                    generator = new VariableScriptGenerator();
                }
            }

            userInputClassName = EditorGUILayout.TextField("类名", userInputClassName);

            if (EditorGUI.EndChangeCheck())
            {
                generator.TargetType = null;

                try
                {
                    typeList = generator.types.Where(x =>  string.Compare(x.Name,userInputClassName,StringComparison.InvariantCultureIgnoreCase) == 0).ToList();
                }
                catch (Exception)
                {
                }
            }

            try
            {
                int index = typeList.IndexOf(generator.TargetType);
                EditorGUI.BeginChangeCheck();
                index = GUILayout.SelectionGrid(index, typeList.Select(x => x.FullName).ToArray(), 1);

                if (index < 0 || index > typeList.Count)
                    index = 0;

                if (EditorGUI.EndChangeCheck() || generator.TargetType == null)
                    generator.TargetType = typeList[index];
            }
            catch (Exception)
            {
                generator.TargetType = null;
            }


            EditorGUILayout.Space();

            if (generator.TargetType == null)
            {
                EditorGUILayout.HelpBox("请先选择类型", MessageType.Info);
            }
            else
            {
                generator.generateVariableClass = EditorGUILayout.Toggle("生成变量类型", generator.generateVariableClass);
                generator.generateVariableDataClass = EditorGUILayout.Toggle("生成变量数据类型", generator.generateVariableDataClass);
                generator.PreviewOnly = EditorGUILayout.Toggle("变量列表仅供预览", generator.PreviewOnly);

                if (generator.TargetType.IsAbstract)
                {
                    EditorGUILayout.HelpBox(generator.TargetType.FullName + " 是抽象类型 无法生成变量", MessageType.Error);
                    generator.generateVariableClass = false;
                }

                if (generator.generateVariableClass)
                {
                    if (generator.ExistingGeneratedClass != null)
                    {
                        EditorGUILayout.HelpBox("变量类型已经存在 继续生成可能覆盖原文件", MessageType.Warning);
                    }
                    if (generator.ExistingGeneratedDrawerClass != null)
                    {
                        EditorGUILayout.HelpBox("变量绘制器已经存在 继续生成可能覆盖原文件", MessageType.Warning);
                    }

                    generator.Category = EditorGUILayout.TextField("分类", generator.Category);
                    generator.NamespaceUsingDeclare = EditorGUILayout.TextField("命名空间引用", generator.NamespaceUsingDeclare);
                }

                EditorGUILayout.Space();
                generator.generatePropertyCommand = EditorGUILayout.Toggle("生成成员读写步骤", generator.generatePropertyCommand);
                if (generator.generatePropertyCommand)
                {
                    generator.generateOnlyDeclaredMembers = EditorGUILayout.Toggle("只显示本类型声明的成员", generator.generateOnlyDeclaredMembers);
                    if (generator.ExistingGeneratedPropCommandClass != null)
                    {
                        EditorGUILayout.HelpBox("成员读写步骤已经存在 继续生成可能覆盖原文件", MessageType.Warning);
                    }
                }

                EditorGUILayout.Space();
                if (GUILayout.Button("立即生成"))
                {
                    try
                    {
                        generator.Generate();
                        EditorUtility.DisplayProgressBar("Generating " + userInputClassName, "Importing Scripts", 0);
                        AssetDatabase.Refresh();
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e.Message);
                        //throw e;
                    }
                    generator = new VariableScriptGenerator();
                    EditorUtility.ClearProgressBar();
                    userInputClassName = "";
                }
            }
        }

        [MenuItem("Tools/Amanita/工具/生成变量类型")]
        public static GenerateVariableWindow ShowWindow()
        {
            var w = GetWindow(typeof(GenerateVariableWindow), true, "生成剧情变量类型", true);
            w.Show();
            return w as GenerateVariableWindow;
        }
    }
}