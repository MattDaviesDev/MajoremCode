using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHarvestable 
{

    void SpawnCritSpot();

    void OnHit(bool isCrit);

    void OnDepleated();

    void OnReplenish();

    sHarvesting.ToolType GetTool();

    bool GetDepleated();

    sResourceNode GetNode();

    Mesh GetMesh();

    float GetVFXSize();

    Texture2D GetTexture();

}
