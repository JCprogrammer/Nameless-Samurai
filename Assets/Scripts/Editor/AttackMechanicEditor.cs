using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
[CustomEditor(typeof(AttackMechanic))]
public class AttackMechanicEditor : Editor
{
    string lastObject = "";
    int targetTagsnumbers = 0;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var attackMechanic = target as AttackMechanic;
        attackMechanic.againstEveryObject = GUILayout.Toggle(attackMechanic.againstEveryObject, "AgainstEveryObject");
        if (!attackMechanic.againstEveryObject)
        {
            ShowTagList(attackMechanic);
        }
    }

    void ShowTagList(AttackMechanic attackMechanic)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Target Tags:");
        targetTagsnumbers = EditorGUILayout.IntField(targetTagsnumbers);
        if ((Event.current.keyCode == KeyCode.Return))
        {
            if (targetTagsnumbers < attackMechanic.targetTags.Count)
            {
                attackMechanic.targetTags.RemoveRange(targetTagsnumbers,
                    attackMechanic.targetTags.Count - targetTagsnumbers);
            }
            else if (targetTagsnumbers > attackMechanic.targetTags.Count)
            {
                for (int i = 0; i < targetTagsnumbers + 1 - attackMechanic.targetTags.Count; i++)
                {
                    attackMechanic.targetTags.Add(attackMechanic.targetTags.Count > 0 ?
                        attackMechanic.targetTags[attackMechanic.targetTags.Count - 1] :
                        "");
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < attackMechanic.targetTags.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Element" + i);
            attackMechanic.targetTags[i] = EditorGUILayout.TextField(attackMechanic.targetTags[i]);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("(New) Element" + attackMechanic.targetTags.Count);
        lastObject = EditorGUILayout.TextField(lastObject);
        EditorGUILayout.EndHorizontal();

        if (lastObject != "")
        {
            if (Event.current.keyCode == KeyCode.Return)
            {
                targetTagsnumbers += 1;
                attackMechanic.targetTags.Add(lastObject);
                lastObject = "";
            }
        }

        EditorGUILayout.EndVertical(); 
    }
}