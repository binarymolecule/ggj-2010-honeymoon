using System;

namespace Apollo.Components
{
	/// <summary>
	/// Defines a service that is optional for a Dependent(Drawable)BasicGameComponent's operation.
	/// The component must have a settable property for the given type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class OptionalServiceAttribute : Attribute
	{
		/// <summary>
		/// Gets the type of the service that is desired.
		/// </summary>
		public Type ServiceType { get; private set; }

		/// <summary>
		/// Initializes a new OptionalServiceAttribute.
		/// </summary>
		/// <param name="serviceType">The type of service to request.</param>
		public OptionalServiceAttribute(Type serviceType)
		{
			ServiceType = serviceType;
		}
	}
}