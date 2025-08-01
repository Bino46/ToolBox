using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

//[CustomEditor(typeof(SpellData))]
public class SpellDataEditor : Editor
{
    public VisualTreeAsset visualTree;

    private SpellData data;
    

    void OnEnable()
    {
        data = (SpellData)target;
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();

        visualTree.CloneTree(root);
        Debug.Log("create window");

        return root;
    }

}
