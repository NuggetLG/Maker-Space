using System;
using UnityEngine;

public class AttachBoat : MonoBehaviour
{
    private ClawController clawController;
    private bool isAttached = false; // Indica si el objeto está adjunto al barco
    [SerializeField] private float maxDepth = -40; // Profundidad máxima permitida

    private void Start()
    {
        clawController = FindObjectOfType<ClawController>();
    }

    private void Update()
    {
        // Verificar si la profundidad del objeto supera el valor máximo
        if (transform.position.y < maxDepth)
        {
            Destroy(gameObject); // Destruir el objeto
            clawController.OnObjectfailed();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isAttached) return; // Evitar múltiples llamadas

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

                isAttached = true; // Marcar como adjuntado
                clawController.OnObjectAttached();
            }
        }
    }
}