using UnityEngine;

public class Boat2d : MonoBehaviour
{
    public float area; // Área del barco
    public float weight; // Peso inicial del barco
    private Rigidbody rb;

    private Vector3 weightedPositionSum = Vector3.zero; // Suma ponderada de posiciones
    private float totalMass; // Masa total del barco
    
    public float horizontalCenterOfMass; // Centro de masa horizontal (-1 a 1)
    private float boatLength; // Longitud del barco
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        totalMass = weight; // Inicializar con el peso inicial del barco
        rb.mass = totalMass / area;

        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        // Calcular la longitud del barco
        boatLength = GetComponent<Collider>().bounds.size.x;
    }

    public void AddMassAndRecalculateCenterOfMass(float addedMass, Vector3 addedPosition)
    {
        // Convertir la posición global a local
        Vector3 localPosition = transform.InverseTransformPoint(addedPosition);

        // Actualizar la suma ponderada de posiciones
        weightedPositionSum += localPosition * addedMass;

        // Actualizar la masa total
        totalMass += addedMass;
        rb.mass = totalMass / area;

        // Recalcular el centro de masa
        rb.centerOfMass = weightedPositionSum / totalMass;

        // Recalcular el centro de masa horizontal normalizado
        horizontalCenterOfMass = (CalculateHorizontalCenterOfMass())*10f;
    }

    private float CalculateHorizontalCenterOfMass()
    {
        // Obtener la posición horizontal promedio ponderada
        float weightedHorizontalPosition = weightedPositionSum.x / totalMass;

        // Normalizar la posición horizontal a un rango de -1 a 1
        float normalizedPosition = Mathf.Clamp(weightedHorizontalPosition / (boatLength / 2), -1f, 1f);

        return normalizedPosition;
    }

    public float GetHorizontalCenterOfMass()
    {
        return horizontalCenterOfMass;
    }
    
}