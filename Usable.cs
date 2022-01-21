using UnityEngine;

namespace WrightWay.VR
{
	/// <summary>
	/// Something that can be used.
	/// </summary>
	public class Usable : MonoBehaviour, IUsable
	{
		/// <summary>
		/// Event for starting using the object.
		/// </summary>
		public UsableUseEvent OnUsed;
		/// <summary>
		/// Event for stoping using the object.
		/// </summary>
		public UsableUseEvent OnUnused;

		private void Start()
		{

		}

		private void Update()
		{

		}

		/// <summary>
		/// Start using the object.
		/// </summary>
		public void Use(Hand hand)
		{
			OnUsed.Invoke();
		}

		/// <summary>
		/// Stop using the object.
		/// </summary>
		public void Unuse(Hand hand)
		{
			OnUnused.Invoke();
		}
	}
}