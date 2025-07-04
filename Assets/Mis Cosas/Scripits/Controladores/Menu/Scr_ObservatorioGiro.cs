using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_ObservatorioGiro : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [Header("Padre")]
    public Axis parentAxis = Axis.Z; // Eje configurable
    public float parentMinAngle = 200f;
    public float parentMaxAngle = 130f;

    [Header("Hijo")]
    public Transform child;
    public Axis childAxis = Axis.Y;
    public float childMinAngle = 0f;
    public float childMaxAngle = 180f;

    private Quaternion childInitialLocalRotation;

    void Start()
    {
        if (child != null)
        {
            // Guarda la rotación inicial del hijo para no perderla
            childInitialLocalRotation = child.localRotation;
        }
    }

    void Update()
    {
        // Normalizar posición del mouse (0..1)
        float mouseX = Mathf.Clamp01(Input.mousePosition.x / Screen.width);
        float mouseY = Mathf.Clamp01(Input.mousePosition.y / Screen.height);

        // Mapear a ángulos
        float parentAngle = Mathf.Lerp(parentMinAngle, parentMaxAngle, mouseX);
        float childAngle = Mathf.Lerp(childMinAngle, childMaxAngle, mouseY);

        // Rotar padre
        Vector3 parentRotation = transform.localEulerAngles;
        parentRotation = SetAxisRotation(parentRotation, parentAxis, parentAngle);
        transform.localEulerAngles = parentRotation;

        // Rotar hijo adicionalmente sobre su propio eje
        if (child != null)
        {
            Vector3 axisVector = GetAxisVector(childAxis);
            Quaternion additionalRotation = Quaternion.AngleAxis(childAngle, axisVector);

            // Aplicar rotación inicial + adicional
            child.localRotation = childInitialLocalRotation * additionalRotation;
        }
    }

    Vector3 SetAxisRotation(Vector3 euler, Axis axis, float angle)
    {
        switch (axis)
        {
            case Axis.X: euler.x = angle; break;
            case Axis.Y: euler.y = angle; break;
            case Axis.Z: euler.z = angle; break;
        }
        return euler;
    }

    Vector3 GetAxisVector(Axis axis)
    {
        switch (axis)
        {
            case Axis.X: return Vector3.right;
            case Axis.Y: return Vector3.up;
            case Axis.Z: return Vector3.forward;
            default: return Vector3.up;
        }
    }
}
