using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Apollo.Components
{
	/// <summary>
	/// A basic implementation of the IComparer interface.
	/// </summary>
	public class DrawOrderComparer : IComparer<IDrawable>
	{
		/// <summary>
		/// Compares two IDrawable objects for equality.
		/// </summary>
		/// <param name="x">The first IDrawable object.</param>
		/// <param name="y">The second IDrawable object.</param>
		/// <returns>A value indicating the objects equality based on the DrawOrder property:
		/// 0 - The two DrawOrders are the same.
		/// 1 - The first IDrawable object's DrawOrder is larger than the second object.
		/// -1 - The first IDrawable object's DrawOrder is smaller than the second object.
		/// </returns>
		public int Compare(IDrawable x, IDrawable y)
		{
			return x.DrawOrder.CompareTo(y.DrawOrder);
		}
	}
}