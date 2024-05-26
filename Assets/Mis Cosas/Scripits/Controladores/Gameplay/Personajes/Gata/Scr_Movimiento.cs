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
    public float Arrastre;
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
    private bool ListoParaSaltar = true;

    [Header("Agachar")]
    public float VelAgachado;
    public float EscalaAgachadoY;
    private float EscalaInicialY;

    [Header("Rampa")]
    public float AnguloMaximo;
    private RaycastHit RampaRayo;
    private bool SalirRampa;

    [Header("Teclas")]
    public KeyCode SaltoTecla = KeyCode.Space;
    public KeyCode CorrerTecla = KeyCode.LeftShift;
    public KeyCode AgacharTecla = KeyCode.LeftControl;

    public Estados Estado;
    public enum Estados
    {
        Quieto,
        Caminar,
        Correr,
        Agachado,
        Aire
    }

    private float InputHor;
    private float InputVer;
    private Vector3 Direccion;
    private Transform Origen;
    private Rigidbody RB;
    private float TiempoGuardado = 0;

    private void Start()
    {
        Origen = GetComponent<Transform>();
        RB = GetComponent<Rigidbody>();
        RB.freezeRotation = true;

        EscalaInicialY = transform.localScale.y;
    }

    private void Update()
    {
        VerificarSuelo();
        CapturarInputs();
        ControlarVelocidad();
        ActualizarEstado();
        GuardarPosicionSiEsNecesario();
        AplicarArrastre();
    }

    private void FixedUpdate()
    {
        Mover();
    }

    private void VerificarSuelo()
    {
        float alturaVerificacion = AlturaPersonaje * 0.5f * transform.localScale.y + 0.2f;
        EstaEnElSuelo = Physics.Raycast(transform.position, Vector3.down, alturaVerificacion, Suelo);
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
        }
        else if (EstaEnElSuelo || Subiendo())
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

    private void Mover()
    {
        Direccion = Origen.forward * InputVer + Origen.right * InputHor;

        // Proyectar la dirección en el plano horizontal
        Direccion = Vector3.ProjectOnPlane(Direccion, Vector3.up).normalized;

        if (Subiendo())
        {
            Vector3 direccionRampa = DireccionRampa();
            RB.AddForce(direccionRampa * Velocidad * 20f, ForceMode.Force);
            if (RB.velocity.y > 0)
            {
                RB.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else
        {
            if (EstaEnElSuelo)
            {
                RB.AddForce(Direccion * Velocidad * 10f, ForceMode.Force);
            }
            else
            {
                RB.AddForce((Direccion + Vector3.down * Gravedad) * Velocidad * 10f, ForceMode.Force);
            }
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

        // Limitar la velocidad vertical para evitar que atraviese el suelo
        if (RB.velocity.y < -50f)
        {
            RB.velocity = new Vector3(RB.velocity.x, -50f, RB.velocity.z);
        }
    }

    private void Saltar()
    {
        SalirRampa = true;
        RB.velocity = new Vector3(RB.velocity.x, 0, RB.velocity.z);
        RB.AddForce(Vector3.up * FuerzaSalto, ForceMode.Impulse); // Usar Vector3.up en lugar de transform.up
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
        TiempoGuardado += Time.deltaTime;
        if (TiempoGuardado >= 5)
        {
            TiempoGuardado = 0;
            GuardaPosYRot();
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
