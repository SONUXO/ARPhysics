using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlenderShapeLoop : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    int  blendShapeCount;
    int index=0;
    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        blendShapeCount = skinnedMesh.blendShapeCount;       
    }

    // Update is called once per frame
    void Update()
    {
        if(index > 0) skinnedMeshRenderer.SetBlendShapeWeight(index-1,0f);
        if(index == 0) skinnedMeshRenderer.SetBlendShapeWeight(blendShapeCount-1, 0f);

       skinnedMeshRenderer.SetBlendShapeWeight (index, 100f); 
       index++;

       if(index > blendShapeCount - 1 ) index=0;
    }
}
