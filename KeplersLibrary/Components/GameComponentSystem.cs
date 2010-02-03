using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Apollo.Components
{
	/// <summary>
	/// A self-contained system for managing a GameComponentCollection.
	/// </summary>
	public sealed class GameComponentSystem
	{
		private static readonly UpdateOrderComparer updateOrderComparer = new UpdateOrderComparer();
		private static readonly DrawOrderComparer drawOrderComparer = new DrawOrderComparer();

		private readonly List<IUpdateable> updateable = new List<IUpdateable>();
		private readonly List<IDrawable> drawable = new List<IDrawable>();

		private readonly List<IUpdateable> tempUpdateable = new List<IUpdateable>();
		private readonly List<IDrawable> tempDrawable = new List<IDrawable>();

		private bool loaded;

		/// <summary>
		/// Gets the underlying GameComponentCollection.
		/// </summary>
		public GameComponentCollection Components { get; private set; }

		/// <summary>
		/// Initializes a new GameComponentSystem.
		/// </summary>
		public GameComponentSystem()
		{
			Components = new GameComponentCollection();
			Components.ComponentAdded += componentAdded;
			Components.ComponentRemoved += componentRemoved;
		}

		private void componentAdded(object sender, GameComponentCollectionEventArgs e)
		{
			IUpdateable u = e.GameComponent as IUpdateable;
			IDrawable d = e.GameComponent as IDrawable;

			if (u != null)
			{
				updateable.Add(u);
				updateable.Sort(updateOrderComparer);
				u.UpdateOrderChanged += componentUpdateOrderChanged;
			}

			if (d != null)
			{
				drawable.Add(d);
				drawable.Sort(drawOrderComparer);
				d.DrawOrderChanged += componentDrawOrderChanged;
			}

			if (loaded)
				e.GameComponent.Initialize();
		}

		private void componentRemoved(object sender, GameComponentCollectionEventArgs e)
		{
			IUpdateable u = e.GameComponent as IUpdateable;
			IDrawable d = e.GameComponent as IDrawable;

			if (u != null)
			{
				updateable.Remove(u);
				u.UpdateOrderChanged -= componentUpdateOrderChanged;
			}

			if (d != null)
			{
				drawable.Remove(d);
				d.DrawOrderChanged -= componentDrawOrderChanged;
			}
		}

		private void componentUpdateOrderChanged(object sender, EventArgs e)
		{
			updateable.Sort(updateOrderComparer);
		}

		private void componentDrawOrderChanged(object sender, EventArgs e)
		{
			drawable.Sort(drawOrderComparer);
		}

		/// <summary>
		/// Initializes all components in the system.
		/// </summary>
		public void Initialize()
		{
			foreach (IGameComponent component in Components)
				component.Initialize();

			loaded = true;
		}

		/// <summary>
		/// Updates all IUpdateable components in the system.
		/// </summary>
		/// <param name="gameTime">The current game timestamp.</param>
		public void Update(GameTime gameTime)
		{
			tempUpdateable.Clear();
			tempUpdateable.AddRange(updateable);

			foreach (IUpdateable comp in tempUpdateable)
				if (comp.Enabled)
					comp.Update(gameTime);
		}

		/// <summary>
		/// Draws all IDrawable components in the system.
		/// </summary>
		/// <param name="gameTime">The current game timestamp.</param>
		public void Draw(GameTime gameTime)
		{
			tempDrawable.Clear();
			tempDrawable.AddRange(drawable);

			foreach (IDrawable comp in tempDrawable)
				if (comp.Visible)
					comp.Draw(gameTime);
		}
	}
}