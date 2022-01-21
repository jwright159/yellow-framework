using System;
using System.Collections;
using UnityEngine;
using Valve.VR;

namespace WrightWay.VR
{
	/// <summary>
	/// One of the two hands used in VR.
	/// </summary>
	[RequireComponent(typeof(SteamVR_Behaviour_Pose))]
	public class Hand : MonoBehaviour
	{
		/// <summary>
		/// The center point with which to query usable <see cref="Usable"/>s.
		/// </summary>
		public Transform useCollisionPoint;
		/// <summary>
		/// The radius with which to query usable <see cref="Usable"/>s.
		/// </summary>
		public float useCollisionRadius;

		/// <summary>
		/// Action for using things. Default GrabPinch.
		/// </summary>
		public SteamVR_Action_Boolean useAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");
		/// <summary>
		/// Action for grabbing things. Default GrabGrip.
		/// </summary>
		public SteamVR_Action_Boolean grabAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");
		// Perhaps move these somewhere else? I don't like hardcoding in these values, at least in Hand.

		/// <summary>
		/// The mask from which to find usable <see cref="Usable"/>s.
		/// </summary>
		public LayerMask useLayerMask = -1;

		/// <summary>
		/// The actual SteamVR interface.
		/// </summary>
		private SteamVR_Behaviour_Pose behaviourPose;

		/// <summary>
		/// The maximum number of colliders to query for.
		/// </summary>
		private const int MaxOverlappingColliders = 32;
		/// <summary>
		/// A list of colliders colliding with this hand based on the <see cref="useCollisionPoint"/> and <see cref="useCollisionRadius"/>.
		/// </summary>
		private Collider[] overlappingColliders = new Collider[MaxOverlappingColliders];

		/// <summary>
		/// The <see cref="Usable"/>s currently being used. <Type, IUsable>
		/// </summary>
		// Perhaps make this a dict of lists if need be later
		private Hashtable usingUsables = new Hashtable();

		protected virtual void Awake()
		{
			behaviourPose = GetComponent<SteamVR_Behaviour_Pose>();
		}

		protected virtual void Start()
		{

		}

		protected virtual void Update()
		{
			UpdateUseState<Usable>();
			UpdateUseState<Grabbable>();
		}

		/// <summary>
		/// Fire use and unuse events to the nearest usable.
		/// </summary>
		private void UpdateUseState<T>() where T : MonoBehaviour, IUsable
		{
			T usingUsable = (T)usingUsables[typeof(T)];

			bool used = GetUse<T>();
			bool unused = GetUnuse<T>();

			if (usingUsable == null && used)
			{
				T usable = GetClosestComponent<T>();
				if (usable)
				{
					usingUsables.Add(typeof(T), usable);
					usable.Use(this);
				}
			}

			if (usingUsable != null && (unused || Vector3.Distance(transform.position, usingUsable.transform.position) > useCollisionRadius))
			{
				usingUsable.Unuse(this);
				usingUsables.Remove(typeof(T));
			}

			// Now what kind of wacky interaction would happen if both of these fired at the same time? Hmm...
		}

		/// <summary>
		/// Get the closest object with a component to the <see cref="useCollisionPoint"/> within the <see cref="useCollisionRadius"/>.
		/// </summary>
		private T GetClosestComponent<T>() where T : MonoBehaviour
		{
			return GetClosestComponent<T>(useCollisionPoint.position, useCollisionRadius);
		}

		/// <summary>
		/// Get the closest object with a component to the <paramref name="position"/> within the <paramref name="radius"/>.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="radius"></param>
		private T GetClosestComponent<T>(Vector3 position, float radius) where T : MonoBehaviour
		{
			int colliderAmount = Physics.OverlapSphereNonAlloc(position, radius, overlappingColliders, useLayerMask);

			if (colliderAmount >= MaxOverlappingColliders)
				Debug.LogWarning("Hand collider amount limit reached, might lose some results");

			T closestComponent = default;
			float closestDistance = float.MaxValue; // Can't just use radius bc it's touching by faces not by centers lmao

			for (int i = 0; i < colliderAmount; i++)
			{
				Collider collider = overlappingColliders[i];
				overlappingColliders[i] = null;

				T component = collider.GetComponentInParent<T>();
				if (component == null)
					continue;

				float distance = Vector3.Distance(position, collider.transform.position); // Something something sqrMagnitude

				if (distance < closestDistance)
				{
					closestComponent = component;
					closestDistance = distance;
				}
			}

			return closestComponent;
		}

		protected virtual bool GetUse<T>() where T : IUsable
		{
			return GetAction<T>().GetStateDown(behaviourPose.inputSource);
		}

		protected virtual bool GetUnuse<T>() where T : IUsable
		{
			return GetAction<T>().GetStateUp(behaviourPose.inputSource);
		}

		private SteamVR_Action_Boolean GetAction<T>() where T : IUsable
		{
			// HACK: This sucks.
			if (typeof(T).IsAssignableFrom(typeof(Usable)))
				return useAction;
			else if (typeof(T).IsAssignableFrom(typeof(Grabbable)))
				return grabAction;
			else
				return null;
		}
	}
}