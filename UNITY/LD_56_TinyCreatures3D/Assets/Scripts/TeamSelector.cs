using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelector : MonoBehaviour
{
    [SerializeField] private CharacterTeam _characterTeam;

    public CharacterTeam CharacterTeam => _characterTeam;
}

public enum CharacterTeam
{
    PLAYER,
    ENEMY,
    NEUTRAL
}
