using UnityEngine;

public class AttachBoat : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat") || other.transform.parent?.CompareTag("Boat") == true)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Desactivar físicas
            }

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true; // Asegurar que el collider sea un trigger
            }

            Transform boatTransform = other.transform.root;
            Boat2d boat = boatTransform.GetComponent<Boat2d>();
            if (boat != null)
            {
                // Adherir el objeto al barco
                transform.SetParent(boatTransform, true);

                
                // Cambiar el tag a "Carga"
                gameObject.tag = "Carga";
                
                // Actualizar la masa y el centro de masa del barco
                boat.AddMassAndRecalculateCenterOfMass(rb.mass, transform.position);
            }
        }
    }
}