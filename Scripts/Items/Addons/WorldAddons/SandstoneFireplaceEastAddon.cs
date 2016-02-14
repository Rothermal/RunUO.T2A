namespace Server.Items
{
    public class SandstoneFireplaceEastAddon : BaseAddon
	{
		[Constructable]
		public SandstoneFireplaceEastAddon()
		{
			AddComponent( new AddonComponent( 0x489 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x475 ), 0, 1, 0 );
		}

		public SandstoneFireplaceEastAddon( Serial serial ) : base( serial )
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