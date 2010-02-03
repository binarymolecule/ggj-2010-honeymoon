using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Apollo.Components
{
	/// <summary>
	/// A basic implementation of the IComparer interface.
	/// </summary>
	public class UpdateOrderComparer : IComparer<IUpdateable>
	{
		/// <summary>
		/// Compares two IUpdateable objects for equality.
		/// </summary>
		/// <param name="x">The first IUpdateable object.</param>
		/// <param name="y">The second IUpdateable object.</param>
		/// <returns>A value indicating the objects equality based on the UpdateOrder property:
		/// 0 - The two UpdateOrders are the same.
		/// 1 - The first IUpdateable object's UpdateOrder is larger than the second object.
		/// -1 - The first IUpdateable object's UpdateOrder is smaller than the second object.
		/// </returns>
		public int Compare(IUpdateable x, IUpdateable y)
		{
			return x.UpdateOrder.CompareTo(y.UpdateOrder);
		}
	}
}