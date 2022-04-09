using UnityEngine;

public class CharColliderController : MonoBehaviour
{
    TestController testController;
    UnitManager um;
    void Start()
    {
        testController = GetComponentInParent<TestController>();
        um = GetComponentInParent<UnitManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (um == null || testController == null) return;
        GameObject parent = transform.parent.gameObject;
        if (other.transform.parent == null) return;
        GameObject otherParent = other.transform.parent.gameObject;
        if (other.gameObject.name == "ViewRange")
        {
            return;
        }
        if (um.target == null && !testController.targetArrived)
        {
            if (um.gridMap.GetMapValue(testController.targetPos.x, testController.targetPos.z) == otherParent.GetInstanceID() &&
                !parent.GetComponent<UnitManager>().selected &&
                (otherParent.tag == "Building" || (otherParent.tag == "Unit" && otherParent.GetComponent<TestController>().targetArrived)))
            {

                testController.SetTargetPos(um.gridMap.GetNearestLoc(testController.targetPos.x, testController.targetPos.z));
            }
        }
        if (um.target != null && !testController.targetArrived)
        {
            if (um.target != other.gameObject &&
                (otherParent.tag == "Building" || (otherParent.tag == "Unit" && otherParent.GetComponent<TestController>().targetArrived)))
            {
                Vector3 origin = parent.transform.position;
                origin.y = 4;
                Vector3 end = testController.targetPos;
                end.y = 4;
                Ray ray = new Ray(origin, Vector3.Normalize(end - origin));
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 100.0f, (1 << 3)))
                {
                    if (hitInfo.transform.parent != null && hitInfo.transform.parent.gameObject != um.target)
                    {
                        Vector3 dir = Vector3.Normalize(otherParent.transform.position - parent.transform.position);
                        Vector3 newPos = parent.transform.position - dir * 1.0f;
                        dir = new Vector3(-dir.z, dir.y, dir.x);
                        if (um.avoidDir % 2 == 1) dir = -dir;
                        testController.SetTargetPos(newPos + dir * 5.0f);
                    }
                }
            }
        }
        if (um.target != null && um.target.tag == "Building" && other.gameObject == um.target)
        {
            um.touchTarget = true;
        }
        else
        {
            um.touchTarget = false;
        }
    }
}
