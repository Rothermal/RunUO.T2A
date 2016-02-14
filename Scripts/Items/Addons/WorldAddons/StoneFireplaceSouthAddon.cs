namespace Server.Items
{
    public class StoneFireplaceSouthAddon : BaseAddon
	{
		[Constructable]
		public StoneFireplaceSouthAddon()
		{
			AddComponent( new AddonComponent( 0x967 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 0x961 ), 0, 0, 0 );
		}

		public StoneFireplaceSouthAddon( Serial serial ) : base( serial )
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