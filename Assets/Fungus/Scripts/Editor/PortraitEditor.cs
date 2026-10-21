// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Portrait))]
    public class PortraitEditor : CommandEditor
    {
        protected SerializedProperty stageProp;
        protected SerializedProperty displayProp;
        protected SerializedProperty characterProp;
        protected SerializedProperty replacedCharacterProp;
        protected SerializedProperty portraitProp;
        protected SerializedProperty offsetProp;
        protected SerializedProperty fromPositionProp;
        protected SerializedProperty toPositionProp;
        protected SerializedProperty facingProp;
        protected SerializedProperty useDefaultSettingsProp;
        protected SerializedProperty fadeDurationProp;
        protected SerializedProperty moveDurationProp;
        protected SerializedProperty shiftOffsetProp;
        protected SerializedProperty waitUntilFinishedProp;
        protected SerializedProperty moveProp;
        protected SerializedProperty shiftIntoPlaceProp;

        public override void OnEnable()
        {
            base.OnEnable();

            stageProp = serializedObject.FindProperty("stage");
            displayProp = serializedObject.FindProperty("display");
            characterProp = serializedObject.FindProperty("character");
            replacedCharacterProp = serializedObject.FindProperty("replacedCharacter");
            portraitProp = serializedObject.FindProperty("portrait");
            offsetProp = serializedObject.FindProperty("offset");
            fromPositionProp = serializedObject.FindProperty("fromPosition");
            toPositionProp = serializedObject.FindProperty("toPosition");
            facingProp = serializedObject.FindProperty("facing");
            useDefaultSettingsProp = serializedObject.FindProperty("useDefaultSettings");
            fadeDurationProp = serializedObject.FindProperty("fadeDuration");
            moveDurationProp = serializedObject.FindProperty("moveDuration");
            shiftOffsetProp = serializedObject.FindProperty("shiftOffset");
            waitUntilFinishedProp = serializedObject.FindProperty("waitUntilFinished");
            moveProp = serializedObject.FindProperty("move");
            shiftIntoPlaceProp = serializedObject.FindProperty("shiftIntoPlace");
        }
        
        public override void DrawCommandGUI() 
        {
            serializedObject.Update();
            
            Portrait t = target as Portrait;

            if (Stage.ActiveStages.Count > 1)
            {
                CommandEditor.ObjectField<Stage>(stageProp, 
                                                         new GUIContent("立绘舞台", "选择用于显示角色立绘的舞台"), 
                                                         new GUIContent("默认"),
                                                         Stage.ActiveStages);
            }
            else
            {
                t._Stage = null;
            }
            // Format Enum names
            string[] displayLabels = StringFormatter.FormatEnumNames(t.Display,"<None>");
            displayProp.enumValueIndex = EditorGUILayout.Popup("立绘操作", (int)displayProp.enumValueIndex, displayLabels);

            string characterLabel = "Character";
            if (t.Display == DisplayType.Replace)
            {
                CommandEditor.ObjectField<Character>(replacedCharacterProp, 
                                                     new GUIContent("替换", "被替换的角色"), 
                                                     new GUIContent("未设置"),
                                                     Character.ActiveCharacters);
                characterLabel = "With";
            }
            
            CommandEditor.ObjectField<Character>(characterProp, 
                                                 new GUIContent(characterLabel, "要显示的角色"), 
                                                 new GUIContent("未设置"),
                                                 Character.ActiveCharacters);

            bool showOptionalFields = true;
            Stage s = t._Stage;
            // Only show optional portrait fields once required fields have been filled...
            if (t._Character != null)                // Character is selected
            {
                if (t._Character.Portraits == null ||    // Character has a portraits field
                    t._Character.Portraits.Count <= 0 )   // Character has at least one portrait
                {
                    EditorGUILayout.HelpBox("这个角色还没有立绘 请先在角色组件中添加图片", MessageType.Error);
                    showOptionalFields = false; 
                }
                if (t._Stage == null)            // If default portrait stage selected
                {
                    if (t._Stage == null)        // If no default specified, try to get any portrait stage in the scene
                    {
                    #if UNITY_6000
                        s = GameObject.FindFirstObjectByType<Stage>();
                    #else
                        s = GameObject.FindObjectOfType<Stage>();
                    #endif
                    }
                }
                if (s == null)
                {
                    EditorGUILayout.HelpBox("请先指定立绘舞台", MessageType.Error);
                    showOptionalFields = false; 
                }
            }
            if (t.Display != DisplayType.None && t._Character != null && showOptionalFields) 
            {
                if (t.Display != DisplayType.Hide && t.Display != DisplayType.MoveToFront) 
                {
                    // PORTRAIT
                    CommandEditor.ObjectField<Sprite>(portraitProp, 
                                                      new GUIContent("角色立绘", "用于表示该角色的立绘"), 
                                                      new GUIContent("沿用上一个"),
                                                      t._Character.Portraits);
                    if (t._Character.PortraitsFace != FacingDirection.None)
                    {
                        // FACING
                        // Display the values of the facing enum as <-- and --> arrows to avoid confusion with position field
                        string[] facingArrows = new string[]
                        {
                            "<Previous>",
                            "<--",
                            "-->",
                        };
                        facingProp.enumValueIndex = EditorGUILayout.Popup("朝向", (int)facingProp.enumValueIndex, facingArrows);
                    }
                    else
                    {
                        t.Facing = FacingDirection.None;
                    }
                }
                else
                {
                    t._Portrait = null;
                    t.Facing = FacingDirection.None;
                }
                string toPositionPrefix = "";
                if (t.Move)
                {
                    // MOVE
                    FungusEditorGUI.PropertyField(moveProp);
                }
                if (t.Move)
                {
                    if (t.Display != DisplayType.Hide) 
                    {
                        // START FROM OFFSET
                        FungusEditorGUI.PropertyField(shiftIntoPlaceProp);
                    }
                }
                if (t.Move)
                {
                    if (t.Display != DisplayType.Hide) 
                    {
                        if (t.ShiftIntoPlace)
                        {
                            t.FromPosition = null;
                            // OFFSET
                            // Format Enum names
                            string[] offsetLabels = StringFormatter.FormatEnumNames(t.Offset,"<Previous>");
                            offsetProp.enumValueIndex = EditorGUILayout.Popup("起始偏移", (int)offsetProp.enumValueIndex, offsetLabels);
                        }
                        else
                        {
                            t.Offset = PositionOffset.None;
                            // FROM POSITION
                            CommandEditor.ObjectField<RectTransform>(fromPositionProp, 
                                                                     new GUIContent("起始站位", "将立绘移动到这个站位"), 
                                                                     new GUIContent("沿用上一个"),
                                                                     s.Positions);
                        }
                    }
                    toPositionPrefix = "To ";
                }
                else
                {
                    t.ShiftIntoPlace = false;
                    t.FromPosition = null;
                    toPositionPrefix = "At ";
                }
                if (t.Display == DisplayType.Show || (t.Display == DisplayType.Hide && t.Move) )
                {
                    // TO POSITION
                    CommandEditor.ObjectField<RectTransform>(toPositionProp, 
                                                             new GUIContent(toPositionPrefix+"站位", "将立绘移动到这个站位"), 
                                                             new GUIContent("沿用上一个"),
                                                             s.Positions);
                }
                else
                {
                    t.ToPosition = null;
                }
                if (!t.Move && t.Display != DisplayType.MoveToFront)
                {
                    // MOVE
                    FungusEditorGUI.PropertyField(moveProp);
                }
                if (t.Display != DisplayType.MoveToFront)
                {
                
                    EditorGUILayout.Separator();

                    // USE DEFAULT SETTINGS
                    FungusEditorGUI.PropertyField(useDefaultSettingsProp);
                    if (!t.UseDefaultSettings) {
                        // FADE DURATION
                        FungusEditorGUI.PropertyField(fadeDurationProp);
                        if (t.Move)
                        {
                            // MOVE SPEED
                            FungusEditorGUI.PropertyField(moveDurationProp);
                        }
                        if (t.ShiftIntoPlace)
                        {
                            // SHIFT OFFSET
                            FungusEditorGUI.PropertyField(shiftOffsetProp);
                        }
                    }
                }
                else
                {
                    t.Move = false;
                    t.UseDefaultSettings = true;
                    EditorGUILayout.Separator();
                }

                FungusEditorGUI.PropertyField(waitUntilFinishedProp);


                if (t._Portrait != null && t.Display != DisplayType.Hide)
                {
                    Texture2D characterTexture = t._Portrait.texture;

                    float aspect = (float)characterTexture.width / (float)characterTexture.height;
                    Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(100), GUILayout.ExpandWidth(true));

                    if (characterTexture != null)
                    {
                        GUI.DrawTexture(previewRect,characterTexture,ScaleMode.ScaleToFit,true,aspect);
                    }
                }

                if (t.Display != DisplayType.Hide)
                {
                    string portraitName = "<Previous>";
                    if (t._Portrait != null)
                    {
                        portraitName = t._Portrait.name;
                    }
                    string portraitSummary = " " + portraitName;
                    int toolbarInt = 1;
                    string[] toolbarStrings = {"<--",  portraitSummary, "-->"};
                    toolbarInt = GUILayout.Toolbar (toolbarInt, toolbarStrings, GUILayout.MinHeight(20));
                    int portraitIndex = -1;

                    if (toolbarInt != 1)
                    {
                        for(int i=0; i<t._Character.Portraits.Count; i++){
                            if(portraitName == t._Character.Portraits[i].name) 
                            {
                                portraitIndex = i;
                            }
                        }
                    }

                    if (toolbarInt == 0)
                    {
                        if(portraitIndex > 0)
                        {
                            t._Portrait = t._Character.Portraits[--portraitIndex];
                        }
                        else
                        {
                            t._Portrait = null;
                        }
                    }
                    if (toolbarInt == 2)
                    {
                        if(portraitIndex < t._Character.Portraits.Count-1)
                        {
                            t._Portrait = t._Character.Portraits[++portraitIndex];
                        }
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}