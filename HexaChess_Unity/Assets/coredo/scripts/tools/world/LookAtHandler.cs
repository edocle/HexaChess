using UnityEngine;

public class LookAtHandler : MonoBehaviour
{
    [SerializeField] Transform m_TargetToLookAt = null;

    public void SetTargetToLookAt(Transform target)
    {
        m_TargetToLookAt = target;
    }

    private void LateUpdate()
    {
        if (m_TargetToLookAt != null)
            UpdateLookAt();
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
}
