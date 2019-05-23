using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Racetracks
{
    class Body : RotatingSpriteGameObject
    {
        protected float radius;
        private Vector2 acceleration = Vector2.Zero;
		protected Vector2 force = Vector2.Zero;
        private float invMass = 1.0f; //set indirectly by setting 'mass'
		protected const float STEERING = 0.04f;
		private const int MAX_SPEED = 800;
		private const float FRICTION = 0.96f;
                
        /// <summary>Creates a physics body</summary>
        public Body(Vector2 position, string assetName) : base(assetName)
        {
            Vector2 size = new Vector2(Width, Height); //circle collider will fit either width or height
            radius = Math.Max(Width, Height) / 2.0f;
            Mass = (radius * radius); //mass approx.
            origin = Center;
            this.position = position;
        }

        /// <summary>Updates this Body</summary>        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
			acceleration = force / Mass;
			if (velocity.Length() < MAX_SPEED) {
				velocity += acceleration;
			}
			velocity *= FRICTION;
			force = Vector2.Zero;
        }

		public void Collision(Body other)
		{
			Vector2 distance = position - other.position;

			if(distance.Length() < radius + other.radius)
			{
				distance.Normalize();
				distance *= (position - other.position).Length() - radius - other.radius;
				position -= distance / 2;
				other.position += distance / 2;

				Vector2 newVel = ((Mass - other.Mass) * velocity + (2 * other.Mass * other.velocity)) / (Mass + other.Mass);
				other.velocity = ((other.Mass - Mass) * other.velocity + (2 * Mass * velocity)) / (other.Mass + Mass);

				velocity = newVel;
			}
		}

        /// <summary>Returns closest point on this shape</summary>        
        public Vector2 GetClosestPoint(Vector2 point)
        {
            Vector2 delta = point - position;
            if (delta.LengthSquared() > radius * radius)
            {
                delta.Normalize();
                delta *= radius;
            }
            return position + delta;
        }

        /// <summary>Sets the invMass to 0.0</summary>        
        protected void MakeStatic()
        {
            invMass = 0.0f; //making mass infinite
        }

        /// <summary>Get or set the invMass through setting the mass</summary>        
        private float _mass = 1.0f;
        public float Mass
        {
            get
            {
                return _mass;
            }
            set
            {
                _mass = value;
                invMass = 1.0f / value; //note: float div. by zero will give infinite invMass
            }
        }                
        
        /// <summary>Get or set the angle by getting/setting the forward Vec2</summary>
        public Vector2 Forward
        {
            get
            {
                return new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle)); //angle to polar
            }

            set
            {
                Angle = (float)Math.Atan2(value.Y, value.X); //polar to angle
            }
        }

        /// <summary>Get or set the angle by getting/setting the right Vec2</summary>
        public Vector2 Right
        {
            get
            {
                return new Vector2((float)-Math.Sin(Angle), (float)Math.Cos(Angle)); //angle to polar
            }

            set
            {
                Angle = (float)Math.Atan2(-value.X, value.Y); //polar to angle
            }
        }
        
    }
}
