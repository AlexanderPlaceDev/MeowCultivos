using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC", menuName = "NPC/Comportamiento")]
public class NPC_comportamiento : ScriptableObject
{
    public string DiaActual = "LUN"; // Día inicial del juego
    public int HoraActual = 10; // Hora inicial, por ejemplo, 6:00 AM
    public float MinutoActual = 0;
}
