
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // [SerializeField] Camera m_Camera = null;

    [SerializeField] Transform m_TargetToLookAt = null;

    public void SetTargetToLookAt(Transform target)
    {
        m_TargetToLookAt = target;
    }


    [SerializeField] private List<CameraPreset> m_Presets;
    [SerializeField] string m_ActivePreset = null;
    CameraPreset CurrentPreset => m_Presets.Find(f => f.name == m_ActivePreset);

    public void ApplyPreset(string preset)
    {
        ApplyPreset(preset, false);
    }

    public void ApplyPreset(string preset, bool immediate)
    {
        m_ActivePreset = preset;
        StartMoveCamera(immediate);
    }

    void StartMoveCamera(bool immediate)
    {
        m_TargetPosition = CurrentPreset.Position;
        m_CameraMoveInProgress = true;

        if (immediate)
        {
            EndUpdatePosition();
        }
    }

    private void LateUpdate()
    {
        if (m_CameraMoveInProgress)
            UpdatePosition();

        if (m_TargetToLookAt != null)
            UpdateLookAt();
    }

    #region Move camera

    bool m_CameraMoveInProgress = false;
    private Vector3 m_TargetPosition = new Vector3();

    [Header("Position SmoothDamp params")]
    [SerializeField] private float m_CameraSmoothMargin = 0.1f;
    [SerializeField] private float m_SmoothTime = 0.2f;
    private Vector3 m_SmoothVelocity = Vector3.zero;

    void UpdatePosition()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, m_TargetPosition, ref m_SmoothVelocity, m_SmoothTime);
        if (Vector3.Magnitude(transform.localPosition - m_TargetPosition) < m_CameraSmoothMargin)
            EndUpdatePosition();
    }

    void EndUpdatePosition()
    {
        transform.localPosition = m_TargetPosition;
        m_CameraMoveInProgress = false;
    }

    Vector3 m_PreviousLookAtVector = Vector3.zero;

    void UpdateLookAt()
    {
        Vector3 lookAtPosition = m_TargetToLookAt.position - transform.position;

        if (lookAtPosition == m_PreviousLookAtVector)
            return;

        m_PreviousLookAtVector = lookAtPosition;
        lookAtPosition.y = 0;
        var rotDir = Quaternion.Euler(0, -90, 0) * lookAtPosition;
        var upDir = Quaternion.AngleAxis(90, rotDir) * (m_TargetToLookAt.position - transform.position);
        var rotation = Quaternion.LookRotation(m_TargetToLookAt.position - transform.position, upDir);
        transform.rotation = rotation;
    }

    #endregion Move camera
}

[System.Serializable]
public class CameraPreset
{
    public string name = "New preset";
    public Vector3 Position = new Vector3();
    public float fieldOfView = 79f;
}
