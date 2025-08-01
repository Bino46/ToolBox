using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Rendering;
using System;

//[CustomEditor(typeof(AddedBehavior))]
public class NewBehaviorDataEditor : Editor
{
    public VisualTreeAsset visualTree;
    private VisualElement explosionElement;
    private VisualElement delayElement;
    private EnumField enumField;
    private SerializedProperty enumProperty;

    void OnEnable()
    {
        enumProperty = serializedObject.FindProperty("newBehavior");
        Debug.Log(enumProperty);
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        visualTree.CloneTree(root);

        enumField = root.Q<EnumField>("ChooseType");;
        //enumField.RegisterCallback<ChangeEvent<AddedBehavior.behaviorType>>(OnEnumChanged);
        enumField.RegisterCallback<ChangeEvent<Enum>>(OnEnumChanged);

        return root;
    }

    private void OnEnumChanged(ChangeEvent<Enum> evt)
    {
        CheckForDisplayChange();
    }

    private void CheckForDisplayChange()
    {
        // if (enumProperty.enumValueFlag == (int)AddedBehavior.behaviorType.Delay)
        // {
        //     Debug.Log("delay");
        // }
    }
}
