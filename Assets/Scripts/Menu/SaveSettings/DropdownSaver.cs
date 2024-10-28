using UnityEngine;
using UnityEngine.UI;
using System;

namespace SaveSystem
{
    /// <summary>
    /// Saves and loads the state of a Dropdown component.
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    public class DropdownSaver : Saver // TODO: add better comments
    {
        [SerializeField] private Dropdown _dropdown;

        public override void Construct()
        {
            _dropdown = GetComponent<Dropdown>();
            base.Construct();
        }

        public override void Load(Tuple<string, object> data)
        {
            if (id != data.Item1) return;
            _dropdown.value = (int)((Int64)data.Item2);
            Debug.Log($"Loaded data for Dropdown with GUID: {id}");
        }

        public override Tuple<string, object> Save()
        {
            return new Tuple<string, object>(id, _dropdown.value);
        }
    }
}