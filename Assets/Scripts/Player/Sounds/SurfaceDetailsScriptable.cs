using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SurfaceDetails", menuName = "Gus/Surface Details", order = 0)]
public class SurfaceDetailsScriptable : ScriptableObject
{
    [SerializeField] private List<SurfaceDetails> surfaceDetails = new();
    
    public SurfaceDetails SurfaceFootsteps(string tagToSearch)
    {
        SurfaceDetails surface = surfaceDetails.FirstOrDefault(x => x.surfaceTag.Equals(tagToSearch));
        if (surface != null)
        {
            if (surface.allowFootsteps) return surface;
            else return default;
        }
        return tagToSearch != "Default" ? SurfaceFootsteps("Default") : default;
    }
}

[Serializable] public class SurfaceDetails
{
    public string surfaceTag;
    public bool allowFootsteps = true;
    [Space(1)]
    public CustomAudioClip[] walkFootsteps;
    public CustomAudioClip[] sprintFootsteps;
    public CustomAudioClip[] sheepFootsteps;
}