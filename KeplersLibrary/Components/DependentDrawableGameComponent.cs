using System;
using System.Reflection;

namespace Apollo.Components
{
	/// <summary>
	/// Defines a drawable game component that uses dependency injection through the service provider.
	/// </summary>
	/// <remarks>
	/// Components deriving from this type should use the RequiredServiceAttribute and
	/// OptionalServiceAttribute to define the services that are to be injected.
	/// </remarks>
	public abstract class DependentBasicDrawableGameComponent : BasicDrawableGameComponent
	{
		/// <summary>
		/// Initializes a new DependentBasicDrawableGameComponent.
		/// </summary>
		/// <param name="services">The service provider containing the required and optional services.</param>
		protected DependentBasicDrawableGameComponent(IServiceProvider services)
			: base(services)
		{
			PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic);

			foreach (PropertyInfo property in properties)
			{
				object[] attributes = property.GetCustomAttributes(true);
				foreach (object attr in attributes)
				{
					RequiredServiceAttribute reqService = attr as RequiredServiceAttribute;
					OptionalServiceAttribute optService = attr as OptionalServiceAttribute;

					if (reqService != null)
					{
						Type serviceType = reqService.ServiceType ?? property.PropertyType;
						object service = services.GetService(serviceType);

						if (service == null)
							throw new Exception(string.Format(DependentBasicGameComponent.serviceNotFoundExceptionFormat, serviceType));

						property.SetValue(this, service, null);
					}
					else if (optService != null)
					{
						Type serviceType = optService.ServiceType ?? property.PropertyType;
						object service = services.GetService(serviceType);

						if (service != null)
							property.SetValue(this, service, null);
					}
				}
			}
		}
	}
}