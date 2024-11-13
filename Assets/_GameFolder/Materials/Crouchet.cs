
using UnityEngine;

public class DistributeOnFaces : MonoBehaviour
{
    public GameObject objectToDistribute; // Dağıtılacak obje
    public GameObject targetObject; // Hedef obje
    public Vector3 sizeMultiplier = Vector3.one; // Boyut ayarlama için çarpan

    private bool distributed = false; // Dağıtımın yapılıp yapılmadığını kontrol etmek için

    // Unity editöründe butonun olayını bağlamak için bu metodu kullanıyoruz
    public void Generate()
    {
        if (!distributed) // Daha önce dağıtım yapılmadıysa
        {
            Distribute();
            distributed = true; // Dağıtım yapıldı olarak işaretle
        }
    }

    void Distribute()
    {
        Mesh mesh = targetObject.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

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

            // Yüzey merkezini hesapla
            Vector3 faceCenter = (p0 + p1 + p2) / 3f;

            // Normali hesapla
            Vector3 faceNormal = Vector3.Cross(p1 - p0, p2 - p0).normalized;

            // Yeni obje oluştur ve pozisyon, rotasyon ayarla
            GameObject newObj = Instantiate(objectToDistribute, faceCenter, Quaternion.LookRotation(faceNormal));
            newObj.transform.parent = targetObject.transform;

            // Yüzey boyutuna göre ölçek ayarla
            float faceWidth = Vector3.Distance(p0, p1); // Yüzey genişliği
            float faceHeight = Vector3.Distance(p0, p2); // Yüzey yüksekliği
            newObj.transform.localScale = new Vector3(faceWidth, faceHeight, Mathf.Min(faceWidth, faceHeight)) * sizeMultiplier.magnitude;

            // Obje pozisyonunu yukarıdan aşağıya ve soldan sağa doğru ayarlamak için
            Vector3 offset = new Vector3(0, -0.5f * faceHeight, 0); // Aşağıya doğru kaydır
            newObj.transform.position += offset;
        }
    }
}