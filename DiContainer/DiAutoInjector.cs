using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Whaledevelop.DiContainer
{
    public class DiAutoInjector : MonoBehaviour
    {
        public enum InjectType : byte
        {
            ProjectContext,
            SceneContext
        }

        [SerializeField]
        private InjectType _injectType;

        private void Awake()
        {
            var container = _injectType switch
            {
                InjectType.ProjectContext => DiContainerUtility.MainContainer,
                InjectType.SceneContext => DiContainerUtility.GetContainerById(SceneManager.GetActiveScene().name),
                _ => throw new ArgumentOutOfRangeException(nameof(_injectType), _injectType, null)
            };
            container.InjectGameObject(gameObject);
            Destroy(this);
        }
    }
}