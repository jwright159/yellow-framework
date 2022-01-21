using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace WrightWay.VR
{
	/// <summary>
	/// The player of the game.
	/// </summary>
	public class Player : MonoBehaviour
	{
		public SteamVR_Behaviour_Pose leftHand;
		public SteamVR_Behaviour_Pose rightHand;

		private IEnumerator Start()
		{
			// This just makes the object wait to start until SteamVR gets some sort of initialized
			// It's a coroutine btw
			while (SteamVR.initializedState == SteamVR.InitializedStates.None || SteamVR.initializedState == SteamVR.InitializedStates.Initializing)
				yield return null;

			if (SteamVR.instance != null)
			{
				// use vr stuffs
			}
			else
			{
				// use flatscreen stuffs
			}
		}

		private void Update()
		{
			if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
				return;
		}
	}
}