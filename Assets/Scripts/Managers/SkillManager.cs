using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public UnitManager um;

    public int skillId = 0;
    [SerializeField] private bool moveCancel = true;
    [SerializeField] private bool haltOnStart = true;
    [SerializeField] private bool onRotToMousePos = true;
    [SerializeField] private bool hasTarget = true;

    void Start()
    {
        um = GetComponentInParent<UnitManager>();
    }
    public bool OnSetTarget()
    {
        if (!moveCancel) return false;
        else
        {
            Debug.Log("Cancelling skill");
            gameObject.SetActive(false);
            return true;
        }
    }
    public Command GetSkillCommand()
    {
        Command cmd = new Command();
        if(onRotToMousePos)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 50000.0f, (1 << 6)))
            {
                cmd.vec3 = Vector3.Normalize(hitInfo.point - transform.parent.position);
            }
        }
        if (hasTarget && um.target != null)
        {
            if (um.target.tag == "Unit") cmd.id = um.target.GetComponent<UnitManager>().netId;
            else if (um.target.tag == "Building") cmd.id = um.target.GetComponent<BuildingManager>().netId;
        }
        else cmd.id = -1;
        cmd.cnt = 1;
        cmd.ids = new int[] { um.netId };
        return cmd;
    }
    public bool OnSkillTriggered(GameObject target, Vector3 vec3)
    {
        if(haltOnStart)
        {
            um.Halt();
        }
        if (onRotToMousePos)
        {

            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hitInfo;
            //if (Physics.Raycast(ray, out hitInfo, 50000.0f, (1 << 6)))
            //{
            //    um.RotateTo(Vector3.Normalize(hitInfo.point - transform.parent.position));
            //}
            um.RotateTo(vec3);
        }
        return true;
    }
}
