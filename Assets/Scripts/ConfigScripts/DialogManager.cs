using UnityEngine;

[CreateAssetMenu(menuName = "Managers/DialogManager")]
public class DialogManager : ScriptableObject
{

    public bool HasParameter(string _name, out DialogParameter _parameter)
    {
        for (int i = 0; i < dialogParameters.Length; i++)
        {
            if (dialogParameters[i].inkTag == _name)
            {
                _parameter = dialogParameters[i];
                return true;
            }
        }

        _parameter = null;
        return false;
    }

    public DialogParameter[] dialogParameters;

    [System.Serializable]
    public class DialogParameter
    {
        public string inkTag;
        public string nameInUnity;
        public Sprite sprite;
    }
}
