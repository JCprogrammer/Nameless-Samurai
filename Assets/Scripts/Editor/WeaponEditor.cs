using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Weapon weapon = (Weapon)target;

        if (weapon.weaponObject != null)
        {
            SGAnimation animator = weapon.weaponObject.GetComponent<SGAnimation>();
            if (animator == null)
            {
                EditorGUILayout.HelpBox("Weapon Object does not have any SGAnimation Assigned to it.", MessageType.Warning, false);
            }

            Collider2D collider = weapon.weaponObject.GetComponent<Collider2D>();
            if (collider == null)
            {
                EditorGUILayout.HelpBox("Weapon Object does not have any Collider2D Assigned to it.", MessageType.Warning, false);
            }
        }
    }
}
