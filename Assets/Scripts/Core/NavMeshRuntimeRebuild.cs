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

        private void OnEnable()
        {
            // Rebuild NavMesh whenever scene loads or this component is enabled
            StartCoroutine(RebuildNavMeshOnLoad());
        }

        private IEnumerator RebuildNavMeshOnLoad()
        {
            // Wait for scene to fully load and all obstacles to be in place
            yield return new WaitForSeconds(0.5f);
            
            if (surface != null)
            {
                Debug.Log("[NavMeshRuntimeRebuild] Rebuilding NavMesh...");
                surface.BuildNavMesh();
                Debug.Log("[NavMeshRuntimeRebuild] NavMesh rebuilt successfully!");
            }
            else
            {
                Debug.LogWarning("[NavMeshRuntimeRebuild] NavMeshSurface not found!");
            }
        }

        // Optional: Also rebuild on demand if needed
        public void ForceRebuild()
        {
            if (surface != null)
            {
                Debug.Log("[NavMeshRuntimeRebuild] Force rebuilding NavMesh...");
                surface.BuildNavMesh();
            }
        }
    }
}
