using System.Collections.Generic;
using UnityEngine;

public class TeamData : MonoBehaviour
{
    public List<TeamDetails> teamDetails;
}

[System.Serializable]
public class TeamDetails
{
    public List<TeamPlayer> Teams;
}

[System.Serializable]
public class TeamPlayer
{
    public Vector3 PlayerPosition;

    public Vector3 PlayerRotation;

    public Vector3 PlayerlocalScale;
}