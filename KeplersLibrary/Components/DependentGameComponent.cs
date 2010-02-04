using System;
using System.Reflection;

namespace Apollo.Components
{
	/// <summary>
	/// Defines a game component that uses dependency injection through the service provider.
	/// </summary>
	/// <remarks>
	/// Components deriving from this type should use the RequiredServiceAttribute and
	/// OptionalServiceAttribute to define the services that are to be injected.
	/// </remarks>
	public abstract class DependentBasicGameComponent : BasicGameComponent
	{
		internal const string serviceNotFoundExceptionFormat =
			"No service of type {0} found. Make sure the service is added to the IServiceProvider before creating the component.";

		/// <summary>
		/// Initializes a new DependentBasicGameComponent.
		/// </summary>
		/// <param name="services">The service provider containing the required and optional services.</param>
		protected DependentBasicGameComponent(IServiceProvider services)
		{
			PropertyInfo[] properties = GetType().GetProperties();

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
							throw new Exception(string.Format(serviceNotFoundExceptionFormat, serviceType));

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