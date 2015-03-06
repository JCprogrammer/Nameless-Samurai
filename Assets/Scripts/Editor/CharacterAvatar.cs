using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CharacterAvatar: EditorWindow
{
    public BodyPart[] bodyParts;
    public Rect avatarPosition;
    static CharacterAvatar window;
    [MenuItem("Spooky Guys/CharacterAvatar")]
    static void Init()
    {
        window = (CharacterAvatar)EditorWindow.GetWindow(typeof(CharacterAvatar));
        window.minSize = new Vector2(1000, 700);
        window.avatarPosition = new Rect(Screen.width / 2 - 150, Screen.height - 150,   300, 300);   
        window.wantsMouseMove = true;
       
    }

    void OnGUI()
    {
        GUI.DrawTexture(avatarPosition, Resources.Load<Texture>("Avatar/avatar"));
    }



    
}

