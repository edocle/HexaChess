using edocle.tools;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace hexaChess.prototyping.generateMesh
{
    public class ProtoScene_GenerateMesh : MonoBehaviour
    {
        [SerializeField] private MeshGenData m_MeshGenData = null;
        [SerializeField] private Material m_Material = null;

        private void Start()
        {
            // 1) Génération du mesh
            Mesh mesh = new Mesh();
            MeshGeneration.GenerateMesh(ref mesh, m_MeshGenData);

            // 2) Création de l'objet en scène
            GameObject meshObject = new GameObject("GeneratedMesh", typeof(MeshFilter), typeof(MeshRenderer));
            meshObject.GetComponent<MeshFilter>().mesh = mesh;
            meshObject.GetComponent<MeshRenderer>().material = m_Material;

#if UNITY_EDITOR
            // 3) Sauvegarde de l'asset dans le dossier Assets/GeneratedMeshes
            string folder = "Assets/GeneratedMeshes";
            if (!Directory.Exists(folder))
            {
                // Création du dossier côté système de fichiers
                Directory.CreateDirectory(folder);
                // S'assurer que l'éditeur connait le nouveau dossier
                AssetDatabase.ImportAsset(folder);
            }

            // Construire un chemin unique pour éviter d'écraser des assets existants
            string basePath = Path.Combine(folder, "GeneratedMesh.asset").Replace("\\", "/");
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(basePath);

            // Instancier le mesh pour en faire un asset distinct de l'instance runtime
            Mesh meshAsset = Object.Instantiate(mesh);
            meshAsset.name = "GeneratedMesh";

            // Créer l'asset, marquer dirty et sauvegarder
            AssetDatabase.CreateAsset(meshAsset, uniquePath);
            EditorUtility.SetDirty(meshAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Mesh sauvegardé en tant qu'asset : {uniquePath}");
#endif
        }
    }
}
