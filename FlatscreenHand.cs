using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WrightWay.VR
{
	public class FlatscreenHand : Hand
	{
		/// <summary>
		/// The camera with which to raycast in non-VR.
		/// </summary>
		public Camera flatscreenCamera;

		/// <summary>
		/// The default maximum distance to raycast at in non-VR.
		/// </summary>
		public float flatscreenRaycastDistance;

		/// <summary>
		/// The mask for raycasting the flatscreen hand.
		/// </summary>
		public LayerMask flatscreenRaycastLayerMask = -1;

		/// <summary>
		/// A debug object showing where our raycast is coming from. Should be on the Ignore Raycast layer.
		/// </summary>
		public Transform flatscreenAim;
		/// <summary>
		/// The distance from the camera to the aim object.
		/// </summary>
		public float flatscreenAimDistance;

		/// <summary>
		/// The last distance that a raycast was hit at.
		/// </summary>
		private float lastHitDistance;

		/// <summary>
		/// The maximum raycast distance depending on whether this is holding a <see cref="Grabbable"/>.
		/// </summary>
		public float currentRaycastDistance
		{
			get => _currentRaycastDistance;
			set
			{
				_currentRaycastDistance = value;

				if (lastHitDistance > currentRaycastDistance)
					lastHitDistance = currentRaycastDistance;
			}
		}
		private float _currentRaycastDistance;

		protected override void Start()
		{
			base.Start();

			currentRaycastDistance = flatscreenRaycastDistance;

			// Start this with a value so we don't have the hand in our flatscreen face
			lastHitDistance = flatscreenRaycastDistance;
		}

		protected override void Update()
		{
			UpdateFlatscreenHand();

			base.Update();
		}

		/// <summary>
		/// Update a hand based on the mouse cursor on the screen.
		/// </summary>
		public void UpdateFlatscreenHand()
		{
			Ray ray = flatscreenCamera.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, currentRaycastDistance, flatscreenRaycastLayerMask))
			{
				// Put the hand on the hit point so we might interact with it
				transform.position = hit.point;

				lastHitDistance = hit.distance;
			}
			else
			{
				// Didn't hit anything, but move the hand around in the empty air
				transform.position = ray.origin + ray.direction * lastHitDistance;
			}
			transform.rotation = Quaternion.LookRotation(transform.position - flatscreenCamera.transform.position, Vector3.up);

			// Update the aim for debugging purposes (after the raycast)
			if (flatscreenAim != null)
			{
				flatscreenAim.position = ray.origin + flatscreenAimDistance * ray.direction;
				flatscreenAim.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
			}
		}
		// TODO: Make our own flatscreen camera controller

		protected override bool GetUse<T>()
		{
			return base.GetUse<T>() || Input.GetButtonDown(GetButtonName<T>());
		}

		protected override bool GetUnuse<T>()
		{
			return base.GetUnuse<T>() || Input.GetButtonUp(GetButtonName<T>());
		}

		private string GetButtonName<T>() where T : IUsable
		{
			if (typeof(T).IsAssignableFrom(typeof(Usable)))
				return "Fire1";
			else if (typeof(T).IsAssignableFrom(typeof(Grabbable)))
				return "Fire2";
			else
				return null;
		}
	}
}
