using UnityEngine;
using System.Collections;

public interface Vision
{
    GameActor[] runVisionDetection(float fov, float viewDistance);
}
