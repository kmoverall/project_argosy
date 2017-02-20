using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SkewCameraProjection : MonoBehaviour {

    Camera camera;
	
	void OnPreCull ()
    {
        Camera camera = GetComponent<Camera>();
        camera.ResetProjectionMatrix();
        camera.orthographic = true;
        Matrix4x4 skewedProjMatrix = camera.projectionMatrix;
        skewedProjMatrix[1, 2] = 1 / camera.orthographicSize;
        skewedProjMatrix[1, 3] = (camera.farClipPlane + camera.nearClipPlane) / (2 * camera.orthographicSize);
        camera.projectionMatrix = skewedProjMatrix;
	}
}
