using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Apollo.Components
{
	/// <summary>
	/// A base class for a basic game component. Replaces the default
	/// GameComponent class by removing the requirement of a Game instance
	/// in the constructor.
	/// </summary>
	public abstract class BasicGameComponent : IGameComponent, IUpdateable, IDisposable
	{
		private bool enabled = true;
		private int updateOrder;

		/// <summary>
		/// Fired when the component is disposed.
		/// </summary>
		public event EventHandler Disposed;

		/// <summary>
		/// Fired when the Enabled property is changed.
		/// </summary>
		public event EventHandler EnabledChanged;

		/// <summary>
		/// Fired when the UpdateOrder property is changed.
		/// </summary>
		public event EventHandler UpdateOrderChanged;

		/// <summary>
		/// Gets or sets a value indicating whether to update the component.
		/// </summary>
		[XmlIgnore]
		public virtual bool Enabled
		{
			get { return enabled; }
			set
			{
				enabled = value;
				OnEnabledChanged();
			}
		}

		/// <summary>
		/// Gets or sets the order in which the component is updated. Components
		/// are updated in order of smallest to largets UpdateOrder.
		/// </summary>
		[XmlIgnore]
		public virtual int UpdateOrder
		{
			get { return updateOrder; }
			set
			{
				updateOrder = value;
				OnUpdateOrderChanged();
			}
		}

		/// <summary>
		/// Gets a value indicating whether or not the component has been disposed.
		/// </summary>
		[XmlIgnore]
		public bool IsDisposed { get; private set; }

		protected BasicGameComponent()
		{
			enabled = true;
			updateOrder = 0;
		}

		/// <summary>
		/// Allows the component to dispose itself if not already done.
		/// </summary>
		~BasicGameComponent()
		{
			if (!IsDisposed)
				Dispose(false);
		}

		/// <summary>
		/// Disposes the component, allowing it to free any resources necessary.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Allows the component to dispose any resources. This method can be
		/// overridden, but should never be called directly. Call Dispose()
		/// if you want to dispose of a component.
		/// </summary>
		/// <param name="disposing">True if the method was called from Dispose(); false if the method was called from the finalizer.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					if (Disposed != null)
						Disposed(this, EventArgs.Empty);
				}

				IsDisposed = true;
			}
		}

		/// <summary>
		/// Allows the component to initialize itself.
		/// </summary>
		public virtual void Initialize()
		{
		}

		/// <summary>
		/// Allows the component to update itself.
		/// </summary>
		/// <param name="gameTime">The current game timestamp.</param>
		public virtual void Update(GameTime gameTime)
		{
		}

		/// <summary>
		/// Triggers the EnabledChanged event. Override for custom behavior of the event.
		/// </summary>
		protected virtual void OnEnabledChanged()
		{
			if (EnabledChanged != null)
				EnabledChanged(this, EventArgs.Empty);
		}

		/// <summary>
		/// Triggers the UpdateOrderChanged event. Override for custom behavior of the event.
		/// </summary>
		protected virtual void OnUpdateOrderChanged()
		{
			if (UpdateOrderChanged != null)
				UpdateOrderChanged(this, EventArgs.Empty);
		}
	}
}