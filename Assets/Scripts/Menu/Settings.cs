using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Settings
{
    public class Settings : MonoBehaviour
    {
        public void SetName (string name)
        {
            PlayerPrefs.SetString("Nickname",name);
            PlayerPrefs.Save();
        }
    }
}
