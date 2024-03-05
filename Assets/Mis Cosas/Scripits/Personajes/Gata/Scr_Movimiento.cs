using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Movimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public bool UsaEjeHorizontal;
    public bool PuedeRetroceder;
    public float Velocidad;
    public float VelCaminar;
    public float VelCorrer;
    public float arrastre;
    [SerializeField] bool GuardaPosicion;



    [Header("Checar Suelo")]
    public bool EstaEnElSuelo;
    public float AlturaPersonaje;
    public LayerMask Suelo;

    [Header("Salto")]
    public float FuerzaSalto;
    public float SaltoCoolDown;
    public float MultiplicadorDeAire;
    public float Gravedad;
    bool ListoParaSaltar = true;

    [Header("Agachar")]
    public float VelAgachado;
    public float EscalaAgachadoY;
    public float EscalaInicialY;

    [Header("Rampa")]
    public float AnguloMaximo;
    public RaycastHit RampaRayo;
    private bool SalirRampa;

    [Header("Teclas")]
    public KeyCode Salto = KeyCode.Space;
    public KeyCode Correr = KeyCode.LeftShift;
    public KeyCode Agachar = KeyCode.LeftControl;

    public Estados Estado;
    public enum Estados
    {
        Quieto,
        Caminar,
        Correr,
        Agachado,
        Aire
    }


    public float InputHor;
    public float InputVer;

    Vector3 Direccion;
    Transform Origen;
    Rigidbody RB;
    float TiempoGuardado = 0;
    private void Start()
    {
        Origen = GetComponent<Transform>();
        RB = GetComponent<Rigidbody>();
        RB.freezeRotation = true;

        EscalaInicialY = transform.localScale.y;
    }
    void Update()
    {
        EstaEnElSuelo = Physics.Raycast(transform.position, Vector3.down, AlturaPersonaje * 0.5f + 0.2f, Suelo);

        MisInputs();
        ControlarVelocidad();
        MaquinaDeEstados();
        TiempoGuardado += Time.deltaTime;
        if (TiempoGuardado >= 5)
        {
            TiempoGuardado = 0;
            GuardaPosYRot();

        }

        if (EstaEnElSuelo)
        {
            RB.drag = arrastre;
        }
        else
        {
            RB.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        Mover();
    }

    private void MisInputs()
    {
        if (UsaEjeHorizontal)
        {
            InputHor = Input.GetAxisRaw("Horizontal");
        }
        if (PuedeRetroceder)
        {
            InputVer = Input.GetAxisRaw("Vertical");
        }
        else
        {
            if (Input.GetAxisRaw("Vertical") >= 0)
            {
                InputVer = Input.GetAxisRaw("Vertical");
            }
        }


        if (Input.GetKeyDown(Salto) && ListoParaSaltar && EstaEnElSuelo)
        {
            ListoParaSaltar = false;
            Saltar();
            Invoke(nameof(ReiniciarSalto), SaltoCoolDown);
        }

        if (Input.GetKeyDown(Agachar))
        {
            transform.localScale = new Vector3(transform.localScale.x, EscalaAgachadoY, transform.localScale.z);
            RB.AddForce(Vector3.down * 5f, ForceMode.Force);
        }
        if (Input.GetKeyUp(Agachar))
        {
            transform.localScale = new Vector3(transform.localScale.x, EscalaInicialY, transform.localScale.z);
        }

    }

    private void MaquinaDeEstados()
    {
        if (Input.GetKey(Agachar))
        {
            Estado = Estados.Agachado;
            Velocidad = VelAgachado;
        }


        if ((EstaEnElSuelo || Subiendo()) && InputVer > 0 && Input.GetKey(Correr))
        {
            Estado = Estados.Correr;
            Velocidad = VelCorrer;
        }
        else
        {
            if (EstaEnElSuelo || Subiendo())
            {
                if (InputHor == 0 && InputVer == 0)
                {
                    Estado = Estados.Quieto;
                }
                else
                {
                    Estado = Estados.Caminar;
                }
                Velocidad = VelCaminar;
            }
            else
            {
                Estado = Estados.Aire;
            }

        }
    }

    private void Mover()
    {
        Direccion = Origen.forward * InputVer + Origen.right * InputHor;

        if (Subiendo())
        {
            RB.AddForce(DireccionRampa() * Velocidad * 20f, ForceMode.Force);
            if (RB.velocity.y > 0)
            {
                //Debug.Log("Subiendo");
                RB.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else
        {
            if (EstaEnElSuelo)
            {
                RB.AddForce(Direccion.normalized * Velocidad * 10f, ForceMode.Force);
            }
            else
            {
                //Debug.Log("Esta en el aire");
                RB.AddForce((Direccion.normalized + new Vector3(0, -Gravedad, 0)) * Velocidad * 10f, ForceMode.Force);
            }
        }

    }

    private void ControlarVelocidad()
    {
        if (Subiendo() && !SalirRampa)
        {
            if (RB.velocity.magnitude > Velocidad)
            {
                RB.velocity = RB.velocity.normalized * Velocidad;
            }
        }
        else
        {
            Vector3 VelocidadActual = new Vector3(RB.velocity.x, 0, RB.velocity.z);

            if (VelocidadActual.magnitude > Velocidad)
            {
                Vector3 VelocidadLimite = VelocidadActual.normalized * Velocidad;
                RB.velocity = new Vector3(VelocidadLimite.x, RB.velocity.y, VelocidadLimite.z);
            }
        }


    }

    private void Saltar()
    {
        SalirRampa = true;
        RB.velocity = new Vector3(RB.velocity.x, 0, RB.velocity.z);
        RB.AddForce(transform.up * FuerzaSalto, ForceMode.Impulse);
    }
    private void ReiniciarSalto()
    {
        SalirRampa = false;
        ListoParaSaltar = true;
    }

    private bool Subiendo()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RampaRayo, AlturaPersonaje * 0.5f + 0.3f))
        {
            float Angulo = Vector3.Angle(Vector3.up, RampaRayo.normal);
            return Angulo < AnguloMaximo && Angulo != 0;
        }

        return false;
    }

    private Vector3 DireccionRampa()
    {
        return Vector3.ProjectOnPlane(Direccion, RampaRayo.normal).normalized;
    }

    private void GuardaPosYRot()
    {
        if (GuardaPosicion)
        {
            GetComponent<Scr_EventosGuardado>().GuardarPosicion(transform);
        }
    }

}
