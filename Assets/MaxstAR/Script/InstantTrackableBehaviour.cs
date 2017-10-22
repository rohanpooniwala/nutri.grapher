using UnityEngine;
using System.IO;
using JsonFx.Json;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using UnityEngine.Rendering;

namespace maxstAR
{
    public class InstantTrackableBehaviour : AbstractInstantTrackableBehaviour
    {
		public override void OnTrackSuccess(string id, string name, Matrix4x4 poseMatrix)
        {
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

			// Enable renderers
			foreach (Renderer component in rendererComponents)
			{
				component.enabled = true;
			}

			// Enable colliders
			foreach (Collider component in colliderComponents)
			{
				component.enabled = true;
			}

			SpriteRenderer[] rendererComponents2D = GetComponentsInChildren<SpriteRenderer>(true);
			Collider2D[] colliderComponents2D = GetComponentsInChildren<Collider2D>(true);

			// Enable renderers
			foreach (SpriteRenderer component in rendererComponents2D)
			{
				component.enabled = true;
			}

			//			transform.position = MatrixUtils.PositionFromMatrix(poseMatrix);
			//			transform.rotation = MatrixUtils.QuaternionFromMatrix(poseMatrix);
			//			transform.localScale = MatrixUtils.ScaleFromMatrix(poseMatrix);
		}

        public override void OnTrackFail()
        {
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

			// Disable renderer
			foreach (Renderer component in rendererComponents)
			{
				component.enabled = false;
			}

			// Disable collider
			foreach (Collider component in colliderComponents)
			{
				component.enabled = false;
			}

			SpriteRenderer[] rendererComponents2D = GetComponentsInChildren<SpriteRenderer>(true);
			Collider2D[] colliderComponents2D = GetComponentsInChildren<Collider2D>(true);

			// Enable renderers
			foreach (SpriteRenderer component in rendererComponents2D)
			{
				component.enabled = false;
			}
        }
    }
}