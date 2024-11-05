using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
public class DistributeOnFaces : MonoBehaviour
{
    public GameObject objectToDistribute; // Dağıtılacak obje
    public GameObject targetObject; // Hedef obje
    public Vector3 sizeMultiplier = Vector3.one; // Boyut ayarlama için çarpan
    public List<GameObject> distributedObjects = new List<GameObject>(); // Dağıtılan objeleri tutmak için liste
    private List<Vector2> uvCoordinates = new List<Vector2>(); // UV koordinatları tutmak için liste
    void Start()
    {
        // Distribute();
    }
    [Button]
    void ClearDistribution()
    {
        foreach (GameObject obj in distributedObjects) DestroyImmediate(obj);
        distributedObjects = new();
    }
    [Button]
    void Distribute()
    {
        ClearDistribution();

        Mesh mesh = targetObject.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;
        Vector2[] uvs = mesh.uv; // UV koordinatları
        HashSet<int> processedVertices = new HashSet<int>(); // İşlenen vertexleri takip etmek için set
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Yüzeyin üç köşesini al
            Vector3 p0 = vertices[triangles[i]];
            Vector3 p1 = vertices[triangles[i + 1]];
            Vector3 p2 = vertices[triangles[i + 2]];
            // Dünya pozisyonuna çevir
            p0 = targetObject.transform.TransformPoint(p0);
            p1 = targetObject.transform.TransformPoint(p1);
            p2 = targetObject.transform.TransformPoint(p2);
            // Yüzey boyutunu hesapla
            float faceSize = (Vector3.Distance(p0, p1) + Vector3.Distance(p1, p2) + Vector3.Distance(p0, p2)) / 3f; // Boyut
            // Üç köşe için objeleri yerleştir
            for (int j = 0; j < 3; j++)
            {
                int vertexIndex = triangles[i + j];
                // Aynı vertexi birden fazla işlememek için kontrol
                if (processedVertices.Contains(vertexIndex))
                    continue;
                // Vertex pozisyonu ve normalini al
                Vector3 vertexPosition = targetObject.transform.TransformPoint(vertices[vertexIndex]);
                Vector3 vertexNormal = targetObject.transform.TransformDirection(normals[vertexIndex]);
                // Yeni obje oluştur ve pozisyon, rotasyon ayarla
                GameObject newObj = Instantiate(objectToDistribute, vertexPosition, Quaternion.LookRotation(vertexNormal));
                newObj.transform.parent = targetObject.transform;
                // Yüzey boyutuna göre ölçek ayarla
                newObj.transform.localScale = new Vector3(faceSize, faceSize, faceSize) * sizeMultiplier.magnitude;
                // İşlenen vertexi ekle
                processedVertices.Add(vertexIndex);
                // UV koordinatını listeye ekle
                uvCoordinates.Add(uvs[vertexIndex]);
                // Dağıtılan objeyi listeye ekle
                distributedObjects.Add(newObj);
            }
        }
        // Objeleri UV koordinatlarına göre sıralama
        SortDistributedObjectsByUV();
    }
    void SortDistributedObjectsByUV()
    {
        // UV koordinatlarına göre objeleri sıralamak için bir liste oluşturalım
        List<(GameObject obj, Vector2 uv)> uvObjectPairs = new List<(GameObject, Vector2)>();
        for (int i = 0; i < distributedObjects.Count; i++)
        {
            uvObjectPairs.Add((distributedObjects[i], uvCoordinates[i]));
        }
        // UV koordinatlarına göre sıralama
        uvObjectPairs.Sort((a, b) => a.uv.x.CompareTo(b.uv.x)); // X koordinatına göre sıralıyoruz
        // Sıralı listeyi konsola yazdır
        for (int i = 0; i < uvObjectPairs.Count; i++)
        {
            Debug.Log($"Sorted Object {i}: {uvObjectPairs[i].obj.transform.position}, UV: {uvObjectPairs[i].uv}");
        }
    }
}