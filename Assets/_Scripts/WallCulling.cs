using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCulling : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }


    //Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Group");
    //// CullingMask�� "Group" Layer�� �����մϴ�.
    //Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Group"));
    //// Nothing ������ CullingMask���� Group Layer�� �߰��մϴ�.

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "MainFloor":
                
                break;

            case "Stairs":
                break;

            case "Elevator":
                break;

            default:
                break;
        }
        Debug.Log(collision.transform.parent.parent.gameObject.name + " -> " + collision.transform.parent.gameObject.name);
    }
}
