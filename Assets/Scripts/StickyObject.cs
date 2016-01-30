using UnityEngine;
using System.Collections;

public class StickyObject : MonoBehaviour {

    public bool IsPartOfGroup { get { return group != null; } }
    StickyGroup group = null;

    public void OnCollisionStay(Collision collision)
    {
        if (GetComponent<Pickup>() != null && GetComponent<Pickup>().IsPickedUp)
            return;

        if (collision.gameObject.GetComponent<StickyGroup>() != null)
        {
            group = collision.gameObject.GetComponent<StickyGroup>();
            AddToOwnGroup(this);
        }

        if (collision.gameObject.GetComponent<StickyObject>() != null)
        {
            if (!IsPartOfGroup && !collision.gameObject.GetComponent<StickyObject>().IsPartOfGroup)
            {
                CreateStickyGroup();
            }
            else if (!IsPartOfGroup)
            {
                collision.gameObject.GetComponent<StickyObject>().AddToOwnGroup(this);
            }
            else if (!collision.gameObject.GetComponent<StickyObject>().IsPartOfGroup)
            {
                AddToOwnGroup(collision.gameObject.GetComponent<StickyObject>());
            }
            else
            {
            }
        }
    }

    void MergeGroups(StickyObject other)
    {
        Debug.Log("Merge groups");
        other.transform.parent = null;
        if (other.group.transform.childCount == 0)
        {
            Destroy(other.group.gameObject);
        }
        other.group = null;
        AddToOwnGroup(other);
    }

    void AddToOwnGroup(StickyObject obj)
    {
        StartCoroutine(DelaySticky(obj));
    }

    IEnumerator DelaySticky(StickyObject obj)
    {
        for (int i = 0; i < 10; ++i)
        {
            yield return null;
        }
        Destroy(obj.GetComponent<Rigidbody>());
        obj.GetComponent<StickyObject>().group = group;
        obj.transform.parent = group.transform;

    }

    void CreateStickyGroup()
    {
        Debug.Log("New sticky group from " + gameObject);
        var go = new GameObject("Sticky Group");
        group = go.AddComponent<StickyGroup>();
        go.AddComponent<Rigidbody>();

        go.transform.position = transform.position;
        transform.parent = go.transform;

        Destroy(transform.GetComponent<Rigidbody>());
    }
}
