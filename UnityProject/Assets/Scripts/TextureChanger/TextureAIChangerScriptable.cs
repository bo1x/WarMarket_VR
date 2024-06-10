using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAIChangerScriptable : MonoBehaviour
{
    public List<Texture> TextureList;
    public Material NPCMaterial;

    // Start is called before the first frame update
    void Start()
    {
        NPCMaterial.mainTexture = TextureList[(int)Random.Range(0, TextureList.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
