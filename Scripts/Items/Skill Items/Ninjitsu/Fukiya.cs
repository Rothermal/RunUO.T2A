using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Mobiles;

namespace Server.Items
{
    [FlipableAttribute( 0x27AA, 0x27F5 )]
	public class Fukiya : Item
	{
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}