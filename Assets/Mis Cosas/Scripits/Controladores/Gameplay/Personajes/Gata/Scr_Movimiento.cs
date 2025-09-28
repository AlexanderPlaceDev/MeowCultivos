using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Scr_Movimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public bool UsaEjeHorizontal;
    public bool PuedeRetroceder;
    public bool CambiaVelocidadAlRetroceder;
    public float Velocidad;
    public float VelocidadRetroceder;
    public float VelCaminar;
    public float VelCorrer;
    public float AumentoDeFov;
    public float VelocidadFov;
    public float Arrastre;
    [SerializeField] bool GuardaPosicion;

    [Header("Checar Suelo")]
    public bool EstaEnElSuelo;
    public float AlturaPersonaje;
    public float OffsetY;
    public LayerMask Suelo;

    [Header("Salto")]
    public float FuerzaSalto;
    public float SaltoCoolDown;
    public float MultiplicadorDeAire;
    public float Gravedad;
    private bool ListoParaSaltar = true;

    [Header("Tiempo en el aire")]
    public float TiempoEnAireMaximo = 5f; // Tiempo máximo en el aire antes de desactivar el collider
    private float tiempoEnAire = 0f;
    private bool colisionDesactivada = false;
    private Collider colisionador;

    [Header("Agachar")]
    public float VelAgachado;
    public float EscalaAgachadoY;
    private float EscalaInicialY;

    [Header("Rampa")]
    public float AnguloMaximo;
    private RaycastHit RampaRayo;

    [Header("Teclas")]
    public KeyCode SaltoTecla = KeyCode.Space;
    public KeyCode CorrerTecla = KeyCode.LeftShift;
    public KeyCode AgacharTecla = KeyCode.LeftControl;

    public Estados Estado;
    public enum Estados
    {
        Quieto,
        Caminar,
        Retroceder,
        Correr,
        Agachado,
        Aire
    }

    public float InputHor;
    public float InputVer;
    private Vector3 Direccion;
    private Transform Origen;
    private Rigidbody RB;
    private float TiempoGuardado = 0;
    public bool PuedeGuardarPosicion = true;
    float FovOriginal;
    float NFov = 0;


    public GameObject Controlador;
    Scr_ControladorBatalla batalla;
    private void Start()
    {
        if (Controlador != null)
        {
            Controlador = GameObject.Find("Controlador");
            batalla = Controlador.GetComponent<Scr_ControladorBatalla>();
        }
        Origen = GetComponent<Transform>();
        RB = GetComponent<Rigidbody>();
        RB.freezeRotation = true;
        colisionador = GetComponent<Collider>();
        FovOriginal = Camera.main.fieldOfView;
        EscalaInicialY = transform.localScale.y;
    }

    private void Update()
    {
        VerificarSuelo();
        ControlarColisionAerea();
        CapturarInputs();
        ActualizarEstado();
        GuardarPosicionSiEsNecesario();
        AplicarArrastre();
        AplicarFovAlCorrer();
    }

    private void AplicarFovAlCorrer()
    {
        if (Estado == Estados.Correr || (Estado == Estados.Aire && Input.GetKey(CorrerTecla)))
        {
            if (NFov < AumentoDeFov)
            {
                NFov += Time.deltaTime * VelocidadFov;
            }
            else
            {
                NFov = AumentoDeFov;
            }
        }
        else
        {
            if (NFov > 0)
            {
                NFov -= Time.deltaTime * VelocidadFov;
            }
            else
            {
                NFov = 0;
            }
        }
        Camera.main.fieldOfView = FovOriginal + NFov;
    }

    private void FixedUpdate()
    {
        Mover();
        AplicarGravedad();
        ControlarVelocidad();
    }

    private void AplicarGravedad()
    {
        // Si el personaje no está en el suelo, aplicar gravedad
        if (!EstaEnElSuelo)  // Solo aplica gravedad si no está en el suelo
        {
            RB.drag = 0;
            RB.velocity = new Vector3(RB.velocity.x, RB.velocity.y - Gravedad * Time.deltaTime, RB.velocity.z);
        }
        else
        {
            RB.drag = Arrastre;
            // Si está en el suelo, aseguramos que no siga cayendo
            if (RB.velocity.y < 0)
            {
                RB.velocity = new Vector3(RB.velocity.x, 0, RB.velocity.z);
            }
        }
    }

    private void VerificarSuelo()
    {
        float alturaVerificacion = AlturaPersonaje * 0.5f * transform.localScale.y + 0.2f;
        EstaEnElSuelo = Physics.Raycast(transform.position + new Vector3(0, OffsetY, 0), Vector3.down, alturaVerificacion, Suelo);
        Debug.DrawRay(transform.position + new Vector3(0, OffsetY, 0), Vector3.down * alturaVerificacion, Color.red);

        // Si está en el suelo, reinicia el tiempo en el aire y activa el collider si estaba desactivado
        if (EstaEnElSuelo)
        {
            tiempoEnAire = 0f;

            if (colisionDesactivada)
            {
                //colisionador.enabled = true; // Reactivamos la colisión
                colisionDesactivada = false;
            }
        }
    }

    private void ControlarColisionAerea()
    {
        // Si el personaje no está en el suelo, aumenta el tiempo en el aire
        if (!EstaEnElSuelo)
        {
            tiempoEnAire += Time.deltaTime;

            // Si ha estado en el aire más del tiempo máximo, desactiva el collider
            if (tiempoEnAire >= TiempoEnAireMaximo && !colisionDesactivada)
            {
                //colisionador.enabled = false; // Desactivar colisión
                colisionDesactivada = true;
            }
        }
    }

    private void CapturarInputs()
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

        if (Input.GetKeyDown(SaltoTecla) && ListoParaSaltar && EstaEnElSuelo)
        {
            ListoParaSaltar = false;
            Saltar();
            Invoke(nameof(ReiniciarSalto), SaltoCoolDown);
        }

        if (Input.GetKeyDown(AgacharTecla))
        {
            Agachar();
        }
        if (Input.GetKeyUp(AgacharTecla))
        {
            Levantarse();
        }
    }

    private void ActualizarEstado()
    {
        if (Input.GetKey(AgacharTecla))
        {
            Estado = Estados.Agachado;
            Velocidad = VelAgachado;
        }
        else if ((EstaEnElSuelo || Subiendo()) && InputVer > 0 && Input.GetKey(CorrerTecla))
        {
            Estado = Estados.Correr;
            Velocidad = VelCorrer;
            if (Camera.main.fieldOfView != Camera.main.fieldOfView + AumentoDeFov)
            {
                Camera.main.fieldOfView += AumentoDeFov;
            }
        }
        else if (EstaEnElSuelo || Subiendo())
        {
            if (Camera.main.fieldOfView == Camera.main.fieldOfView + AumentoDeFov)
            {
                Camera.main.fieldOfView -= AumentoDeFov;
            }
            if (InputHor == 0 && InputVer == 0)
            {
                Estado = Estados.Quieto;
            }
            else
            {
                if (InputVer > 0)
                {
                    Estado = Estados.Caminar;
                    Velocidad = VelCaminar;

                }
                if (InputVer < 0)
                {
                    Estado = Estados.Retroceder;
                    if (CambiaVelocidadAlRetroceder)
                    {
                        Velocidad = VelocidadRetroceder;
                    }
                    else
                    {

                        Velocidad = VelCaminar;
                    }


                }
            }
        }
        else
        {
            Estado = Estados.Aire;
        }
    }

    private void Mover()
    {
        if (batalla != null)
        {
            if (batalla.Stuneado) return;
            if (batalla.Congelado) return;
            if (batalla.Stuneado) return;
        }
        // Dirección de movimiento basada en la entrada
        Direccion = transform.forward * InputVer + transform.right * InputHor;

        if (EstaEnElSuelo)
        {
            // Movimiento en el suelo
            Vector3 velocidadDeseada = Direccion.normalized * Velocidad;
            RB.velocity = new Vector3(velocidadDeseada.x, RB.velocity.y, velocidadDeseada.z);
        }
        else
        {
            // Movimiento en el aire (menos controlado)
            Vector3 velocidadAerea = Direccion.normalized * Velocidad * MultiplicadorDeAire;
            RB.velocity += new Vector3(velocidadAerea.x, 0, velocidadAerea.z) * Time.deltaTime;
        }

        // Si estás en una rampa, ajustar la dirección del movimiento
        if (Subiendo())
        {
            Vector3 direccionRampa = DireccionRampa();
            RB.velocity += direccionRampa * Velocidad * Time.deltaTime;
        }
    }

    private void ControlarVelocidad()
    {
        Vector3 velocidadHorizontal = new Vector3(RB.velocity.x, 0, RB.velocity.z);

        if (velocidadHorizontal.magnitude > Velocidad)
        {
            Vector3 velocidadLimite = velocidadHorizontal.normalized * Velocidad;
            RB.velocity = new Vector3(velocidadLimite.x, RB.velocity.y, velocidadLimite.z);
        }

        if (FuerzaSalto != 0)
        {
            if (RB.velocity.y < -FuerzaSalto)
            {
                RB.velocity = new Vector3(RB.velocity.x, -FuerzaSalto, RB.velocity.z);
            }
            if (RB.velocity.y > FuerzaSalto)
            {
                RB.velocity = new Vector3(RB.velocity.x, FuerzaSalto, RB.velocity.z);
            }
        }

    }

    private void Saltar()
    {
        RB.velocity = new Vector3(RB.velocity.x, 0, RB.velocity.z);
        RB.AddForce(transform.up * FuerzaSalto, ForceMode.Impulse);
    }

    private void ReiniciarSalto()
    {
        ListoParaSaltar = true;
    }

    private bool Subiendo()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RampaRayo, AlturaPersonaje * 0.5f + 0.3f))
        {
            float angulo = Vector3.Angle(Vector3.up, RampaRayo.normal);
            return angulo < AnguloMaximo && angulo != 0;
        }
        return false;
    }

    private Vector3 DireccionRampa()
    {
        return Vector3.ProjectOnPlane(Direccion, RampaRayo.normal).normalized;
    }

    private void Agachar()
    {
        transform.localScale = new Vector3(transform.localScale.x, EscalaAgachadoY, transform.localScale.z);
        RB.AddForce(Vector3.down * 5f, ForceMode.Force);
    }

    private void Levantarse()
    {
        transform.localScale = new Vector3(transform.localScale.x, EscalaInicialY, transform.localScale.z);
    }

    private void GuardarPosicionSiEsNecesario()
    {
        if (PuedeGuardarPosicion)
        {
            TiempoGuardado += Time.deltaTime;
            if (TiempoGuardado >= 5)
            {
                TiempoGuardado = 0;
                GuardaPosYRot();
            }
        }
    }

    private void GuardaPosYRot()
    {
        if (GuardaPosicion)
        {
            GetComponent<Scr_EventosGuardado>().GuardarPosicion(transform);
        }
    }

    private void AplicarArrastre()
    {
        if (EstaEnElSuelo)
        {
            RB.drag = Arrastre;
        }
        else
        {
            RB.drag = 0;
        }
    }
}