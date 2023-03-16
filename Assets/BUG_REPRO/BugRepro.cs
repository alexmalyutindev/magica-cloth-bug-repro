using System.Collections;
using System.Collections.Generic;
using MagicaCloth2;
using UnityEngine;

public class BugRepro : MonoBehaviour
{
    public GameObject DefaultHead;
    public GameObject CustomHead;

    public GameObject Body;

    public Transform RootBone;

    private Dictionary<string, Transform> _skeletonMap = new();

    private IEnumerator Start()
    {
        foreach (var bone in RootBone.GetComponentsInChildren<Transform>())
        {
            _skeletonMap.TryAdd(bone.name, bone);
        }
        
        yield return new WaitForSeconds(1f);

        var defaultHead = Instantiate(DefaultHead, transform);
        Bind(defaultHead);

        var body = Instantiate(Body, transform);
        Bind(body);

        yield return new WaitForSeconds(1f);
        defaultHead.SetActive(false);

        yield return new WaitForSeconds(1f);
        var customHead = Instantiate(CustomHead, transform);
        Bind(customHead);
    }

    private void Bind(GameObject skinPart)
    {
        var magicaCloths = skinPart.GetComponentsInChildren<MagicaCloth>();
        foreach (var magicaCloth in magicaCloths)
        {
            magicaCloth.Initialize();
            magicaCloth.ReplaceTransform(_skeletonMap);
        }

        foreach (var colliderComponent in skinPart.GetComponentsInChildren<ColliderComponent>())
        {
            var colliderComponentTransform = colliderComponent.transform;
            colliderComponentTransform.GetLocalPositionAndRotation(out Vector3 position, out Quaternion rotation);
            colliderComponentTransform.SetParent(_skeletonMap[colliderComponentTransform.parent.name]);
            colliderComponentTransform.SetLocalPositionAndRotation(position, rotation);
        }

        foreach (var skinnedMeshRenderer in skinPart.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            var bones = new Transform[skinnedMeshRenderer.bones.Length];
            for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
            {
                if (_skeletonMap.TryGetValue(skinnedMeshRenderer.bones[i].name, out var bone))
                {
                    bones[i] = bone;
                }
                else if (skinnedMeshRenderer.bones[i] == skinnedMeshRenderer.transform)
                {
                    bones[i] = skinnedMeshRenderer.transform;
                }
                else
                {
                    Debug.LogError("Can't map SkinnedMesh bones!");
                }
            }

            skinnedMeshRenderer.bones = bones;
            skinnedMeshRenderer.rootBone = _skeletonMap[skinnedMeshRenderer.rootBone.name];
        }

        foreach (var magicaCloth in magicaCloths)
        {
            magicaCloth.BuildAndRun();
        }
    }
}
