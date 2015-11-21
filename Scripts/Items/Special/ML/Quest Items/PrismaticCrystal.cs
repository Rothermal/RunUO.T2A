using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class PrismaticCrystal : Item
	{
		public override int LabelNumber{ get{ return 1074269; } } // prismatic crystal

		[Constructable]
		public PrismaticCrystal() : base( 0x2DA )
		{
			Movable = false;
			Hue = 0x32;
		}

		public PrismaticCrystal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
