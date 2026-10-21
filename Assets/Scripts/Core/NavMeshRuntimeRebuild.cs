using System.Collections;
using UnityEngine;
using NavMeshPlus.Components;

namespace MicroDefense.Core
{
    public sealed class NavMeshRuntimeRebuild : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface surface;

        private void Awake()
        {
            if (surface == null)
                surface = FindAnyObjectByType<NavMeshSurface>();
        }

        private IEnumerator Start()
        {
            if (surface == null) yield break;
            yield return null;
            surface.BuildNavMesh();
        }
    }
}
