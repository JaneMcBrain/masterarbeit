using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaneDetectionController : MonoBehaviour
{
    [SerializeField]
    ARSession m_ARSession;

    [SerializeField]
    GameObject m_PlanePrefab; // Ein Prefab, das zur Darstellung von erkannten Ebenen verwendet wird.

    private List<ARPlane> m_DetectedPlanes = new List<ARPlane>(); // Eine Liste erkannter Ebenen.

    void Start()
    {
        Debug.Log("HELLO PLANE DETECTION");
        // Stellen Sie sicher, dass die AR-Session aktiviert ist.
        if (m_ARSession != null)
        {
            m_ARSession.enabled = true;

            // Registrieren Sie einen Handler für das Ereignis, wenn eine Ebene erkannt wird.
            ARPlaneManager planeManager = m_ARSession.GetComponent<ARPlaneManager>();
            if (planeManager != null)
            {
                planeManager.planesChanged += OnPlanesChanged;
            }
        }
    }

    // Handler für das Ereignis, wenn sich die erkannten Ebenen ändern.
    void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
    {
        // Überprüfen Sie, ob Ebenen hinzugefügt wurden.
        foreach (var plane in eventArgs.added)
        {
            m_DetectedPlanes.Add(plane);
            Debug.Log("Ebene erkannt: " + plane);
            // Hier könnten Sie das Prefab m_PlanePrefab instanziieren und positionieren, um die erkannte Ebene darzustellen.
        }

        // Überprüfen Sie, ob Ebenen entfernt wurden.
        foreach (var plane in eventArgs.removed)
        {
            m_DetectedPlanes.Remove(plane);
            Debug.Log("Ebene entfernt: " + plane);
            // Hier könnten Sie die Darstellung der entfernten Ebene aufräumen.
        }
    }

    void Update()
    {
        // Überprüfen Sie, ob Ebenen erkannt werden.
        if (m_DetectedPlanes.Count > 0)
        {
            // Planes wurden erkannt, führen Sie hier entsprechende Aktionen aus.
        }
        else
        {
            // Keine Ebenen erkannt, führen Sie hier ggf. andere Aktionen aus.
        }
    }
}
