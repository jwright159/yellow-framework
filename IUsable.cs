using UnityEngine;

namespace WrightWay.VR
{
	/// <summary>
	/// Something that can be used.
	/// </summary>
	public interface IUsable
	{
		/// <summary>
		/// Start using the object.
		/// </summary>
		/// <param name="hand">The hand using the object.</param>
		public void Use(Hand hand);

		/// <summary>
		/// Stop using the object.
		/// </summary>
		/// <param name="hand">The hand unusing the object.</param>
		public void Unuse(Hand hand);
	}
}