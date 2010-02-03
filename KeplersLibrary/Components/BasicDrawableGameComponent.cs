using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apollo.Components
{
	/// <summary>
	/// A base class for a basic, drawable game component. Replaces the default
	/// DrawableGameComponent by replacing the required Game parameter with an
	/// IServiceProvider, allowing the components to be re-used in both games
	/// and editors.
	/// </summary>
	public abstract class BasicDrawableGameComponent : BasicGameComponent, IDrawable
	{
		private readonly IServiceProvider services;
		private bool initialized;
		private bool visible = true;
		private int drawOrder;

		/// <summary>
		/// Fired when the DrawOrder property is changed.
		/// </summary>
		public event EventHandler DrawOrderChanged;

		/// <summary>
		/// Fired when the Visible property is changed.
		/// </summary>
		public event EventHandler VisibleChanged;

		/// <summary>
		/// Gets or sets the draw order of the component. Components are drawn in
		/// order of smallest to largest DrawOrder.
		/// </summary>
		public int DrawOrder
		{
			get { return drawOrder; }
			set
			{
				drawOrder = value;
				OnDrawOrderChanged();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the component is visible.
		/// </summary>
		public bool Visible
		{
			get { return visible; }
			set
			{
				visible = value;
				OnVisibleChanged();
			}
		}

		/// <summary>
		/// Gets the GraphicsDevice associated with the component.
		/// </summary>
		public GraphicsDevice GraphicsDevice
		{
			get { return (GraphicsDeviceService != null) ? GraphicsDeviceService.GraphicsDevice : null; }
		}

		private IGraphicsDeviceService GraphicsDeviceService { get; set; }

		/// <summary>
		/// Initializes a new BasicDrawableGameComponent.
		/// </summary>
		/// <param name="services">The services provider to use when retrieving the graphics device.</param>
		protected BasicDrawableGameComponent(IServiceProvider services)
		{
			this.services = services;
		}

		/// <summary>
		/// Allows the component to dispose any resources. This method can be
		/// overridden, but should never be called directly. Call Dispose()
		/// if you want to dispose of a component.
		/// </summary>
		/// <param name="disposing">True if the method was called from Dispose(); false if the method was called from the finalizer.</param>
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed && disposing)
			{
				UnloadContent();

				GraphicsDeviceService.DeviceCreated -= DeviceCreated;
				GraphicsDeviceService.DeviceDisposing -= DeviceDisposing;
			}

			base.Dispose(disposing);
		}

		private void DeviceCreated(object sender, EventArgs e)
		{
			LoadContent();
		}

		private void DeviceDisposing(object sender, EventArgs e)
		{
			UnloadContent();
		}

		/// <summary>
		/// Allows the component to initialize itself.
		/// </summary>
		public override void Initialize()
		{
			if (!initialized)
			{
                GraphicsDeviceService = services.GetService(typeof(IGraphicsDeviceService).GetType()) as IGraphicsDeviceService;
				if (GraphicsDeviceService == null)
					throw new InvalidOperationException("No IGraphicsDeviceService found in IServiceProvider.");

				GraphicsDeviceService.DeviceCreated += DeviceCreated;
				GraphicsDeviceService.DeviceDisposing += DeviceDisposing;
				if (GraphicsDeviceService.GraphicsDevice != null)
					LoadContent();
			}

			initialized = true;
		}

		/// <summary>
		/// Allows the component to load any content it requires.
		/// </summary>
		protected virtual void LoadContent()
		{
		}

		/// <summary>
		/// Allows the component to unload any content.
		/// </summary>
		protected virtual void UnloadContent()
		{
		}

		/// <summary>
		/// Allows the component to Draw itself.
		/// </summary>
		/// <param name="gameTime">The current game timestamp.</param>
		public virtual void Draw(GameTime gameTime)
		{
		}

		/// <summary>
		/// Triggers the VisibleChanged event. Override for custom behavior of the event.
		/// </summary>
		protected virtual void OnVisibleChanged()
		{
			if (VisibleChanged != null)
				VisibleChanged(this, null);
		}

		/// <summary>
		/// Triggers the DrawOrderChanged event. Override for custom behavior of the event.
		/// </summary>
		protected virtual void OnDrawOrderChanged()
		{
			if (DrawOrderChanged != null)
				DrawOrderChanged(this, null);
		}
	}
}