using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Scr_ObjetoEnMano : MonoBehaviour
{
    public string Nombre;
    public float Cantidad;

    //Forma del objeto en mano
    public bool[] Forma = new bool[18];

    //Cantidad de casillas que ocupa la forma
    int CasillasActivas;

    //Cantidad de casillas activas y que encuentran espacio
    int CasillasQueEncontraronEspacio;

    //Iconos del objeto en mano
    public Sprite[] Iconos;

    //Casillas que ocupa el objeto en el inventario
    Image[] CasillasOcupadas;

    //Sus propias Casillas
    Scr_CasillaObjetoEnMano[] Casillas = new Scr_CasillaObjetoEnMano[18];

    //En casi de encontrar un espacio para colocarse
    public bool EncontroEspacio;

    Scr_ControladorInventario Inventario;


    private void Start()
    {
        //Agrega sus propias casillas
        for (int i = 0; i < 18; i++)
        {
            Casillas[i] = transform.GetChild(0).GetChild(i).GetComponent<Scr_CasillaObjetoEnMano>();
        }

        Inventario=GameObject.Find("Gata").transform.GetChild(3).GetComponent<Scr_ControladorInventario>();
    }

    void Update()
    {
        //Actualiza su posicion a la del mouse
        transform.position = Input.mousePosition;

        //Vemos Cuantas estan activas, actualiza iconos, actualiza cantidades y si no lo estan se desactivan
        ActualizarObjetoEnMano();


        //creamos el espacio para las casillas que ocuparemos
        CasillasOcupadas = new Image[CasillasActivas];

        //De las que esten activas se busca cuantas encontraron espacio
        ContarCuantasEncontraronEspacio();

        //si las que estan activas encontraron espacio y hay una o mas activas
        if (CasillasQueEncontraronEspacio == CasillasActivas && CasillasActivas > 0)
        {
            EncontroEspacio = true;

            //si oprime el mouse
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ColocarObjeto();
                

        	    ReiniciarObjetoEnMano();
            }

        }
        else
        {
            EncontroEspacio = false;
        }

    }

    void ActualizarObjetoEnMano()
    {
        int NCasilla = 0;
        CasillasActivas = 0;

        foreach (bool Casilla in Forma)
        {
            if (Casilla)
            {
                CasillasActivas++;
                transform.GetChild(0).GetChild(NCasilla).GetComponent<Image>().enabled = true;
                transform.GetChild(0).GetChild(NCasilla).GetComponent<BoxCollider2D>().enabled = true;
                transform.GetChild(0).GetChild(NCasilla).GetComponent<Image>().sprite = Iconos[NCasilla];
                transform.GetChild(0).GetChild(NCasilla).GetChild(0).GetComponent<TextMeshProUGUI>().enabled = true;
                transform.GetChild(0).GetChild(NCasilla).GetChild(0).GetComponent<TextMeshProUGUI>().text = Cantidad.ToString();

            }
            else
            {
                transform.GetChild(0).GetChild(NCasilla).GetComponent<Image>().enabled = false;
                transform.GetChild(0).GetChild(NCasilla).GetComponent<BoxCollider2D>().enabled = false;
                transform.GetChild(0).GetChild(NCasilla).GetChild(0).GetComponent<TextMeshProUGUI>().enabled = false;
            }
            NCasilla++;
        }
    }

    void ContarCuantasEncontraronEspacio()
    {

        int NCasilla = 0;
        CasillasQueEncontraronEspacio = 0;
        foreach (Scr_CasillaObjetoEnMano Casilla in Casillas)
        {
            if (Casilla.EncontroEspacio && (Casilla.CasillaEncontrada.sprite==Casilla.Vacio ||Casilla.CasillaEncontrada.sprite == Casilla.gameObject.GetComponent<Image>().sprite))
            {
                CasillasOcupadas[CasillasQueEncontraronEspacio] = Casilla.CasillaEncontrada;
                CasillasQueEncontraronEspacio++;

            }
            NCasilla++;
        }
    }

    void ReiniciarObjetoEnMano()
    {
        Nombre = "";
        Cantidad = 0;
        CasillasActivas = 0;
        CasillasQueEncontraronEspacio = 0;
        EncontroEspacio=false;

        Forma=new bool[18];
        CasillasOcupadas = new Image[18];
        Iconos= new Sprite[18];
    }


    void ColocarObjeto()
    {   
        
        int Parte = 1;
        foreach (Image Casilla in CasillasOcupadas)
        {
            Casilla.GetComponent<Scr_CasillaInventario>().PuedeAgarrar=false;
            StartCoroutine(EsperarCasilla(Casilla));
            if (CasillasOcupadas.Length > 1)
            {
                Inventario.CasillasContenido[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] = Nombre + Parte;
                Parte++;

                Casilla.GetComponent<Scr_CasillaInventario>().CasillasHermanas=new List<Image>();
                for (int i = 0; i < CasillasOcupadas.Length; i++)
                {
                    Casilla.GetComponent<Scr_CasillaInventario>().CasillasHermanas.Add(CasillasOcupadas[i]);
                }

            }
            else
            {
                Inventario.CasillasContenido[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] = Nombre;
                if(Casilla.GetComponent<Scr_CasillaInventario>().CasillasHermanas.ToArray().Length==0)
                {
                    Casilla.GetComponent<Scr_CasillaInventario>().CasillasHermanas.Add(CasillasOcupadas[0]);
                }
            }


            Casilla.GetComponent<Scr_CasillaInventario>().FormaConHermanas = Forma;

            Inventario.Cantidades[(int)Casilla.GetComponent<Scr_CasillaInventario>().Numero] += Cantidad;
        }
    }

    IEnumerator EsperarCasilla(Image Casilla)
    {
        yield return new WaitForSeconds(0.1f);
        Casilla.GetComponent<Scr_CasillaInventario>().PuedeAgarrar=true;
    }
}
