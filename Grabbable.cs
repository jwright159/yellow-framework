using System;
using UnityEngine;

namespace WrightWay.VR
{
	/// <summary>
	/// Something that can be grabbed.
	/// </summary>
	// TODO: Make it grab
	public class Grabbable : MonoBehaviour, IUsable
	{
		/// <summary>
		/// The maximum raycast distance when in non-VR and this is grabbed.
		/// </summary>
		public float flatscreenRaycastDistance = -1;

		/// <summary>
		/// Event fired after grabbing the object.
		/// </summary>
		public GrabbableGrabEvent OnGrabbed;
		/// <summary>
		/// Event fired after releasing the object.
		/// </summary>
		public GrabbableGrabEvent OnReleased;

		/// <summary>
		/// Whether the object is currently being held.
		/// </summary>
		public bool isGrabbed { get; private set; }

		/// <summary>
		/// Whether we've initialized the grab layers below yet.
		/// </summary>
		private static bool initializedGrabLayers;
		/// <summary>
		/// The layer for unheld Grabbables.
		/// </summary>
		private static int grabbableLayer;
		/// <summary>
		/// The layer for held Grabbables.
		/// </summary>
		private static int grabbedLayer;

		private void Awake()
		{
			if (!initializedGrabLayers)
			{
				initializedGrabLayers = true;
				grabbableLayer = LayerMask.NameToLayer("Usable");
				grabbedLayer = LayerMask.NameToLayer("Grabbed Usable");
			}
		}

		private void Start()
		{
			
		}

		private void Update()
		{

		}

		public void Use(Hand hand)
		{
			if (!isGrabbed)
				Grab(hand);
			else
				Release(hand);
		}

		public void Unuse(Hand hand)
		{
			
		}

		private void Grab(Hand hand)
		{
			isGrabbed = true;

			transform.parent = hand.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;

			if (gameObject.layer != grabbableLayer)
				Debug.LogWarning($"Grabbable {this} is not on the layer {LayerMask.LayerToName(grabbableLayer)}, will be after releasing", this);
			SetChildrenLayer(grabbedLayer);

			if (hand is FlatscreenHand flatscreenHand)
				SetFlatscreenHandRaycastDistance(flatscreenHand, flatscreenRaycastDistance);

			OnGrabbed.Invoke();
		}

		private void Release(Hand hand)
		{
			isGrabbed = false;

			transform.parent = null;

			SetChildrenLayer(grabbableLayer);

			if (hand is FlatscreenHand flatscreenHand)
				SetFlatscreenHandRaycastDistance(flatscreenHand, flatscreenHand.flatscreenRaycastDistance);

			OnReleased.Invoke();
		}

		private void SetChildrenLayer(int layer)
		{
			foreach (Transform transform in GetComponentsInChildren<Transform>())
				transform.gameObject.layer = layer;
		}

		private void SetFlatscreenHandRaycastDistance(FlatscreenHand hand, float raycastDistance)
		{
			if (flatscreenRaycastDistance < 0) return;

			hand.currentRaycastDistance = raycastDistance;
		}
	}
}