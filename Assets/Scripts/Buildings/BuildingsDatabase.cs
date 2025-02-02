using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingsDatabase", menuName = "Buildings/BuildingsDatabase")]
public class BuildingsDatabase : ScriptableObject
{
    public List<Building> Buildings;
}   
