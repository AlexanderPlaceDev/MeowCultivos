using UnityEngine;
using TMPro;

public class Scr_CurvarTexto : MonoBehaviour
{
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public float curveScale = 1.0f;

    private TextMeshProUGUI textMeshPro;
    private Vector3[] originalVertices;

    void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        RecordOriginalVertices();
    }

    void RecordOriginalVertices()
    {
        // Guarda los vértices originales del texto
        textMeshPro.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMeshPro.textInfo;
        originalVertices = new Vector3[textInfo.meshInfo[0].vertices.Length];
        System.Array.Copy(textInfo.meshInfo[0].vertices, originalVertices, originalVertices.Length);
    }

    void Update()
    {
        CurvarTexto();
    }

    void CurvarTexto()
    {
        textMeshPro.ForceMeshUpdate(); // Asegura que la malla esté actualizada

        TMP_TextInfo textInfo = textMeshPro.textInfo;
        int characterCount = textInfo.characterCount;

        Vector3[] vertices;

        float boundsMinX = textMeshPro.bounds.min.x;
        float boundsMaxX = textMeshPro.bounds.max.x;

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                Vector3 orig = originalVertices[charInfo.vertexIndex + j];

                float x0 = (orig.x - boundsMinX) / (boundsMaxX - boundsMinX);
                float y0 = curve.Evaluate(x0) * curveScale;

                vertices[charInfo.vertexIndex + j] = new Vector3(orig.x, orig.y + y0, orig.z);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textMeshPro.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}
