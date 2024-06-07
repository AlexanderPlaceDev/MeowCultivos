using UnityEngine;
using UnityEngine.SceneManagement;

public class Scr_ActivadorPelea : MonoBehaviour
{
    float tiempo = 0;

    SpriteRenderer Color;

    private void Start()
    {
        Color = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (tiempo < 0.5f)
        {
            tiempo += Time.deltaTime;
        }

        GetComponent<SpriteRenderer>().color = new Color(Color.color.r, Color.color.g, Color.color.b, tiempo*2);
    }
}