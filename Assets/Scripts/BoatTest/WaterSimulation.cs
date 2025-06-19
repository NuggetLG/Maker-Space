using UnityEngine;

public class WaterSimulation : MonoBehaviour
{
    public float baseWaterDensity = 1f; // Densidad base del agua
    
    [SerializeField]
    private float buoyantForce; // Fuerza total aplicada (para monitoreo)
    [SerializeField]
    private float DragCoeficient; // Coeficiente de resistencia
    [SerializeField]
    private float rotationForce; // Fuerza de rotación aplicada (para monitoreo)
    [SerializeField]
    private float angularVelocityThreshold = 0.1f; // Umbral para detener el torque
    [SerializeField]
    private float torqueMultiplier = 10f; // Multiplicador para aumentar la fuerza de torque
    [SerializeField]
    private float angularDragCoefficient = 0.5f; // Coeficiente de resistencia angular
    [SerializeField]
    private float depth; // Profundidad umbral
    [SerializeField]
    private float depthThreshold = 0.5f; // Profundidad umbral
    [SerializeField]
    private GameManager gameManager; // Profundidad umbral


    private void OnTriggerStay(Collider other)
{
    Boat2d boat = other.GetComponent<Boat2d>();
    Rigidbody rb = other.GetComponent<Rigidbody>();

    if (boat != null && rb != null)
    {
        // Obtener el volumen total del barco
        float boatVolume = boat.area;
        // Obtener el ancho del objeto
        float objectWidth = other.bounds.size.x;

        // Calcular puntos en el espacio local y transformarlos al espacio global
        Vector3 leftPoint = other.transform.TransformPoint(new Vector3(-objectWidth / 2, 0, 0));
        Vector3 rightPoint = other.transform.TransformPoint(new Vector3(objectWidth / 2, 0, 0));
        Vector3 centerPoint = other.bounds.center;
        
        // Calcular vectores desde el centro hacia los puntos
        Vector3 leftVector = leftPoint - centerPoint;
        Vector3 rightVector = rightPoint - centerPoint;

        // Calcular ángulos con respecto a Vector3.up (global)
        float leftAngle = Vector3.SignedAngle(leftVector, Vector3.up, Vector3.forward);
        float rightAngle = Vector3.SignedAngle(rightVector, Vector3.up, Vector3.forward);

        
        // Calcular el porcentaje sumergido usando Physics.ComputePenetration
        Collider waterCollider = GetComponent<Collider>();
        Collider boatCollider = other;

        Vector3 direction;
        float distance;

        if (Physics.ComputePenetration(
            boatCollider, boatCollider.transform.position, boatCollider.transform.rotation,
            waterCollider, waterCollider.transform.position, waterCollider.transform.rotation,
            out direction, out distance))
        {
            // Calcular la profundidad relativa del objeto en el agua
            float waterTop = waterCollider.bounds.max.y; // Borde superior del agua
            float objectBottom = boatCollider.bounds.min.y; // Borde inferior del objeto
            depth = Mathf.Clamp01((waterTop - objectBottom) / waterCollider.bounds.size.y);
            
            // Activar el bool si la profundidad supera el umbral

            if (depth >= depthThreshold)
            {
                gameManager.GameOver = true;
                baseWaterDensity = 0.2f;
            }
            
            
            // Rotación adicional basada en horizontalCenterOfMass
            float centerOfMassTorque = -(boat.horizontalCenterOfMass * 90f); // Torque proporcional a -90° a 90°
            
            // Calcular la fuerza de rotación total
            rotationForce = (leftAngle + rightAngle)+ centerOfMassTorque;
            Debug.Log("Rotation Force: " + rotationForce);

            // Aplicar el torque al barco
            rb.AddTorque(Vector3.forward * rotationForce, ForceMode.Force);

            // Calcular la densidad del agua basada en la profundidad
            float adjustedWaterDensity = baseWaterDensity * depth;

            // Calcular el porcentaje sumergido basado en la penetración
            float submergedPercentage = Mathf.Clamp01(distance / boatCollider.bounds.size.y);

            // Calcular el volumen sumergido
            float submergedVolume = boatVolume * submergedPercentage;

            // Calcular la fuerza de flotación usando el principio de Arquímedes
            buoyantForce = adjustedWaterDensity * submergedVolume;

            // Aplicar la fuerza de flotación al centro del objeto
            rb.AddForce(Vector3.up * Mathf.Min(buoyantForce, boat.weight), ForceMode.Force);
            
        }

        // Calcular y aplicar la resistencia
        Vector3 velocity = rb.velocity;
        Vector3 dragForce = -DragCoeficient * velocity;
        rb.AddForce(dragForce, ForceMode.Force);
        
        // Calcular y aplicar la resistencia angular
        Vector3 angularVelocity = rb.angularVelocity;
        Vector3 angularDrag = -angularDragCoefficient * angularVelocity;
        rb.AddTorque(angularDrag, ForceMode.Force);

        if (velocity.magnitude < 0.01f)
        {
            Debug.Log("Barco detenido");
            rb.velocity = Vector3.zero;
        }
    }
}
    
    private void OnDrawGizmos()
    {
        Collider waterCollider = GetComponent<Collider>();
        if (waterCollider == null) return;

        // Obtener el ancho del objeto
        Collider[] colliders = FindObjectsOfType<Collider>();
        foreach (var other in colliders)
        {
            if (other.GetComponent<Boat2d>() == null) continue;

            float objectWidth = other.bounds.size.x;

            // Crear puntos de referencia en los extremos del ancho
            Vector3 leftPoint = other.bounds.center - other.transform.right * (objectWidth / 2);
            Vector3 rightPoint = other.bounds.center + other.transform.right * (objectWidth / 2);

            // Dibujar Gizmos para los puntos
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(leftPoint, 0.1f); // Esfera para el punto izquierdo
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(rightPoint, 0.1f); // Esfera para el punto derecho
        }
    }
}