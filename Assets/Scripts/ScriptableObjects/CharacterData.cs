#if UNITY_EDITOR
using UnityEditor;
#endif
using CustomTools;
using UnityEngine;

[CreateAssetMenu(fileName = "Character data",menuName = "create character data")]
public class CharacterData : ScriptableObject
{
    [ReadOnly] public string CharacterGUID;
    public Sprite CharacterIcon;
    public string ChracterName;
    [TextArea] public string CharacterDescription;

#if UNITY_EDITOR
    [Button]
    public void GenerateID()
    {
      //  if (!(string.IsNullOrEmpty(CharacterGUID) || string.IsNullOrWhiteSpace(CharacterGUID))) return;
        CharacterGUID = GUID.Generate().ToString();
        EditorUtility.SetDirty(this);
    }
#endif
}
