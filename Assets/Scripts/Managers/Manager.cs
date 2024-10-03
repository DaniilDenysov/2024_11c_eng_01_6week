using Zenject;
using UnityEngine;
using CustomTools;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Managers
{
    public abstract class Manager : MonoBehaviour
    {
        [SerializeField, ReadOnly] private SceneContext _sceneContext;


        [PreCompilationConstructor]
        public void Construct ()
        {
            _sceneContext = FindObjectOfType<SceneContext>();
            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }

        public virtual void Awake()
        {
            if (_sceneContext != null)
            {
                _sceneContext.Container.BindInstance(this).AsSingle();

                Debug.Log($"{this.GetType().Name} has been autowired to the SceneContext.");
            }
            else
            {
                Debug.LogError("No SceneContext found in the current scene.");
            }
        }
    }
}
