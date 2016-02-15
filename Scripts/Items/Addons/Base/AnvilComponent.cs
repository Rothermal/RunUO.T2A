using System;
using Server;

namespace Server.Items
{
	[Server.Engines.Craft.Anvil]
	public class AnvilComponent : AddonComponent
	{
		[Constructable]
		public AnvilComponent( int itemID ) : base( itemID )
		{
		}

		public AnvilComponent( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}