using System;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public enum MoonstoneType
	{
		Felucca
	}

	public class Moonstone : Item
	{
		private MoonstoneType m_Type;

		[CommandProperty( AccessLevel.GameMaster )]
		public MoonstoneType Type
		{
			get
			{
				return m_Type;
			}
			set
			{
				m_Type = value;
				InvalidateProperties();
			}
		}

		public override int LabelNumber{ get{ return 1041490 + (int)m_Type; } }

		[Constructable]
		public Moonstone( MoonstoneType type ) : base( 0xF8B )
		{
			Weight = 1.0;
			m_Type = type;
		}

		public Moonstone( Serial serial ) : base( serial )
		{
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				Hue = Utility.RandomBirdHue();
				ProcessDelta();
				from.SendLocalizedMessage( 1005398 ); // The stone's substance shifts as you examine it.
			}

			base.OnSingleClick( from );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_Type );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Type = (MoonstoneType)reader.ReadInt();

					break;
				}
			}
		}
	}
}